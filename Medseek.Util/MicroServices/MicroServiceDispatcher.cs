namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Medseek.Util.Interactive;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using Medseek.Util.Messaging;
    using Medseek.Util.Threading;

    /// <summary>
    /// Provides a micro-service dispatcher using a messaging system connection.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "MQ->MessageQueue is not hungarian notation.")]
    [Register(typeof(IMicroServiceDispatcher), Lifestyle = Lifestyle.Singleton, Start = true)]
    public class MicroServiceDispatcher : IMicroServiceDispatcher, IStartable, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<IMqConsumer, List<MicroServiceBinding>> bindingMap = new Dictionary<IMqConsumer, List<MicroServiceBinding>>(); 
        private readonly List<MicroServiceBinding> bindings = new List<MicroServiceBinding>(); 
        private readonly List<IMqConsumer> consumers = new List<IMqConsumer>();
        private readonly IMqChannel channel;
        private readonly IMessageContextAccess messageContextAccess;
        private readonly IMicroServiceLocator microServiceLocator;
        private readonly IMicroServiceSerializer microServiceSerializer;
        private readonly IMqPlugin mqPlugin;
        private readonly IRemoteMicroServiceInvoker remoteMicroServiceInvoker;
        private readonly IDispatchedThread thread;
        private long dispatcherDepth;
        private bool disposed;
        private bool started;
        private bool threadStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceDispatcher" /> 
        /// class.
        /// </summary>
        public MicroServiceDispatcher(
            IMqConnection connection, 
            IMessageContextAccess messageContextAccess,
            IMicroServiceLocator microServiceLocator,
            IMicroServiceSerializer microServiceSerializer,
            IMicroServicesFactory microServicesFactory,
            IMqPlugin mqPlugin,
            IDispatchedThread thread)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (messageContextAccess == null)
                throw new ArgumentNullException("messageContextAccess");
            if (microServiceLocator == null)
                throw new ArgumentNullException("microServiceLocator");
            if (microServiceSerializer == null)
                throw new ArgumentNullException("microServiceSerializer");
            if (microServicesFactory == null)
                throw new ArgumentNullException("microServicesFactory");
            if (mqPlugin == null)
                throw new ArgumentNullException("mqPlugin");
            if (thread == null)
                throw new ArgumentNullException("thread");

            channel = connection.CreateChannnel();
            remoteMicroServiceInvoker = microServicesFactory.GetRemoteMicroServiceInvoker(channel);
            this.messageContextAccess = messageContextAccess;
            this.microServiceLocator = microServiceLocator;
            this.microServiceSerializer = microServiceSerializer;
            this.mqPlugin = mqPlugin;
            this.thread = thread;
            thread.Name = "MicroServices.Dispatch";
        }

        /// <summary>
        /// Gets the remote micro-service invoker.
        /// </summary>
        public IRemoteMicroServiceInvoker RemoteMicroServiceInvoker
        {
            get
            {
                return remoteMicroServiceInvoker;
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);
            if (started)
                throw new InvalidOperationException();

            Log.DebugFormat("{0}", MethodBase.GetCurrentMethod().Name);
            started = true;
            if (!threadStarted)
            {
                threadStarted = true;
                thread.Start();
            }

            microServiceLocator.Initialize();
            var consumerGroups = microServiceLocator.Bindings
                .Do(bindings.Add)
                .Select(binding => new { binding, address = mqPlugin.ToConsumerAddress(binding.Address) })
                .GroupBy(x => x.address.SourceKey)
                .ToArray();

            thread.Invoke(() =>
            {
                foreach (var consumerGroup in consumerGroups)
                {
                    Log.InfoFormat("Starting message consumer; SourceKey = {0}, Bindings = {1}.", consumerGroup.Key, string.Join(", ", consumerGroup.Select(x => x.binding).Select(x => string.Format("(Addresses = {0}, Service = {1}, Method = {2})", x.Address, x.Service, x.Method))));
                    var addresses = consumerGroup.Select(x => x.address).ToArray();
                    var createdConsumers = channel.CreateConsumers(addresses, true);
                    foreach (var consumer in createdConsumers)
                    {
                        bindingMap[consumer] = consumerGroup.Select(x => x.binding).ToList();
                        consumer.Received += OnReceived;
                        consumers.Add(consumer);
                    }
                }
            });
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);
            if (!started)
                throw new InvalidOperationException();

            Log.DebugFormat("{0}", MethodBase.GetCurrentMethod().Name);
            started = false;

            thread.Invoke(() =>
            {
                bindingMap.Clear();
                bindings.Clear();
                foreach (var consumer in consumers.ToArray())
                {
                    consumer.Dispose();
                    consumers.Remove(consumer);
                }
            });
        }

        /// <summary>
        /// Disposes the dispatcher.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                thread.Dispose();
                channel.Dispose();
            }
        }

        private void OnReceived(object sender, ReceivedEventArgs e)
        {
            Log.DebugFormat("Read {0} bytes from queue; CorrelationId = {1}.", e.GetBodyArray().Length, e.Properties.CorrelationId);

            Interlocked.Increment(ref dispatcherDepth);
            thread.InvokeAsync(() =>
            {
                Log.DebugFormat("Identifying micro-service invocation details; CorrelationId = {0}.", e.Properties.CorrelationId);
                if (channel.CanPause)
                    channel.IsPaused = Interlocked.Read(ref dispatcherDepth) > 10;
                try
                {
                    var consumer = (IMqConsumer)sender;
                    var bindingsFromMap = bindingMap[consumer];
                    var binding = bindingsFromMap.First(x => mqPlugin.IsMatch(e.Properties, x.Address));
                    var messageContext = new MessageContext(e.Properties);

                    using (messageContextAccess.Enter(messageContext))
                    using (var instance = microServiceLocator.Get(binding))
                    {
                        object serializerState = null;
                        var parameterTypes = binding.Method.GetParameters().Select(x => x.ParameterType).ToArray();
                        var parameterValues = microServiceSerializer.Deserialize(messageContext, parameterTypes, e.GetBodyStream(), ref serializerState);

                        Log.DebugFormat("Invoking micro-service; Address = {0}, IsOneWay = {1}, Method = {2}, Parameters = {3}.", binding.Address, binding.IsOneWay, binding.Method, string.Join(", ", parameterValues));
                        var returnValue = instance.Invoke(parameterValues);
                        if (e.Properties.ReplyTo != null && !binding.IsOneWay)
                        {
                            byte[] body;
                            using (var ms = new MemoryStream())
                            {
                                var returnType = binding.Method.ReturnType;
                                microServiceSerializer.Serialize(messageContext, returnType, returnValue, ms, ref serializerState);
                                body = ms.ToArray();
                            }

                            var publishAddress = channel.Plugin.ToPublisherAddress(e.Properties.ReplyTo);
                            Log.DebugFormat("Sending reply message; ReplyTo = {0}, ContentType = {1}, CorrelationId = {2}, Body.Length = {3}, Value = {4}.", e.Properties.ReplyTo, e.Properties.ContentType, e.Properties.CorrelationId, body.Length, returnValue);
                            using (messageContextAccess.Enter())
                            using (var publisher = channel.CreatePublisher(publishAddress))
                            {
                                var properties = messageContextAccess.Current.Properties;
                                properties.ReplyTo = null;
                                publisher.Publish(body, properties);
                            }
                        }

                        //channel.BasicAck(e.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    var message = string.Format("Unexpected failure while dispatching message; Cause = {0}: {1}.", ex.GetType().Name, ex.Message.TrimEnd('.'));
                    Log.Warn(message, ex);
                }
                finally
                {
                    var depth = Interlocked.Decrement(ref dispatcherDepth);
                    if (channel.CanPause)
                        channel.IsPaused = 10 < depth;
                }
            });
        }
    }
}