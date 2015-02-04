namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using Medseek.Util.Interactive;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using Medseek.Util.Messaging;
    using Medseek.Util.MicroServices.Lookup;
    using Medseek.Util.Objects;
    using Medseek.Util.Threading;

    /// <summary>
    /// Provides a micro-service dispatcher using a messaging system connection.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "MQ->MessageQueue is not hungarian notation.")]
    [Register(typeof(IMicroServiceDispatcher), Lifestyle = Lifestyle.Singleton, Start = true)]
    public class MicroServiceDispatcher : Disposable, IMicroServiceDispatcher, IStartable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<IMqConsumer, List<MicroServiceBinding>> bindingMap = new Dictionary<IMqConsumer, List<MicroServiceBinding>>(); 
        private readonly List<MicroServiceBinding> bindings = new List<MicroServiceBinding>(); 
        private readonly List<IMqConsumer> consumers = new List<IMqConsumer>();
        private readonly IMqChannel channel;
        private readonly IEnvironment environment;
        private readonly IMicroServiceLookup lookup;
        private readonly IMessageContextAccess messageContextAccess;
        private readonly IMicroServiceLocator microServiceLocator;
        private readonly IMicroServiceSerializer microServiceSerializer;
        private readonly IMicroServicesFactory microServicesFactory;
        private readonly IMqPlugin mqPlugin;
        private readonly IDispatchedThread thread;
        private TimeSpan timeout = TimeSpan.FromSeconds(15);
        private long dispatcherDepth;
        private bool started;
        private bool threadStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceDispatcher" /> 
        /// class.
        /// </summary>
        public MicroServiceDispatcher(
            IMqConnection connection, 
            IEnvironment environment,
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
            lookup = microServicesFactory.GetLookup(channel);
            this.environment = environment;
            this.messageContextAccess = messageContextAccess;
            this.microServiceLocator = microServiceLocator;
            this.microServiceSerializer = microServiceSerializer;
            this.microServicesFactory = microServicesFactory;
            this.mqPlugin = mqPlugin;
            this.thread = thread;
            thread.Name = "MicroServices.Dispatch";
        }

        /// <summary>
        /// Raised to indicate that a message was returned as undeliverable.
        /// </summary>
        public event EventHandler<ReturnedEventArgs> Returned;

        /// <summary>
        /// Raised to indicate that an unhandled exception was encountered by 
        /// the micro-service dispatcher.
        /// </summary>
        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        /// <summary>
        /// Gets the remote micro-service invoker.
        /// </summary>
        [Obsolete("Use IMicroServiceInvoker.Send instead of the methods on IRemoteMicroServiceInvoker.")]
        public IRemoteMicroServiceInvoker RemoteMicroServiceInvoker
        {
            get
            {
                return microServicesFactory.GetRemoteMicroServiceInvoker(channel);
            }
        }

        /// <summary>
        /// Gets or sets the timeout used by remote micro-service invocation 
        /// message transmission operations.
        /// </summary>
        public TimeSpan Timeout
        {
            get
            {
                return timeout > TimeSpan.Zero ? timeout : TimeSpan.Zero;
            }
            set
            {
                timeout = value;
            }
        }

        /// <summary>
        /// Sends a message to a remote micro-service.
        /// </summary>
        /// <param name="address">
        /// The address of the micro-service to which the message should be 
        /// sent.
        /// </param>
        /// <param name="body">
        /// The message body.
        /// </param>
        /// <param name="properties">
        /// The message properties.
        /// </param>
        /// <param name="enableLookup">
        /// A value indicating whether micro-service lookup should be enabled 
        /// for the send operation.
        /// </param>
        public void Send(MqAddress address, byte[] body, MessageProperties properties, bool enableLookup)
        {
            ThrowIfDisposed();
            if (address == null)
                throw new ArgumentNullException("address");
            if (body == null)
                throw new ArgumentNullException("body");
            if (properties == null)
                throw new ArgumentNullException("properties");

            enableLookup = enableLookup && (environment.GetCommandLineArgs() ?? new string[0]).Contains("/enableMicroServiceDispatcherLookup");

            var resolvedAddress = enableLookup ? lookup.Resolve(address, Timeout) : null;
            var publishAddress = mqPlugin.ToPublisherAddress(resolvedAddress ?? address);
            using (var publisher = channel.CreatePublisher(publishAddress))
            {
                if (Log.IsDebugEnabled)
                {
                    var sb = new StringBuilder("Publishing message; Address = " + address);
                    if (!address.Equals(resolvedAddress))
                        sb.AppendFormat(", Resolved = {0}", resolvedAddress != null ? (object)resolvedAddress : "(null)");
                    if (!address.Equals(publishAddress))
                        sb.AppendFormat(", Publish = {0}", publishAddress);
                    Log.Debug(sb
                        .AppendFormat(", Timeout = {0}", Timeout)
                        .AppendFormat(", Body.Length = {0}", body.Length)
                        .Append("."));
                }

                publisher.Publish(body, properties);
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();
            if (started)
                throw new InvalidOperationException();

            Log.DebugFormat("{0}", MethodBase.GetCurrentMethod().Name);
            started = true;
            if (!threadStarted)
            {
                threadStarted = true;
                thread.Start();
            }

            channel.Returned += OnChannelReturned;

            microServiceLocator.Initialize();
            var consumerGroups = microServiceLocator.Bindings
                .Do(bindings.Add)
                .Select(binding => new { binding, address = mqPlugin.ToConsumerAddress(binding.Address) })
                .GroupBy(x => string.Join("|", x.binding.AutoAckDisabled, x.address.SourceKey))
                .ToArray();

            thread.Invoke(() =>
            {
                foreach (var consumerGroup in consumerGroups)
                {
                    var autoAckDisabled = consumerGroup.Select(x => x.binding.AutoAckDisabled).Distinct().SingleOrDefault();
                    var autoDelete = consumerGroup.Select(x => x.binding.AutoDelete).Distinct().SingleOrDefault();
                    Log.InfoFormat("Starting message consumer; SourceKey = {0}, AutoAckDisabled = {1}, Bindings = {2}.", consumerGroup.Key, autoAckDisabled, string.Join(", ", consumerGroup.Select(x => x.binding).Select(x => string.Format("(Addresses = {0}, Service = {1}, Method = {2})", x.Address, x.Service, x.Method))));
                    var addresses = consumerGroup.Select(x => x.address).ToArray();
                    var createdConsumers = channel.CreateConsumers(addresses, autoAckDisabled, autoDelete);
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
            ThrowIfDisposed();
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
        /// Sets up a micro-service operation for handling reply messages.
        /// </summary>
        public IDisposable SetupReply<T>(Action<T> onReceived)
        {
            var messageContext = messageContextAccess.Current;
            var correlationId = messageContext.Properties.CorrelationId;
            var originalCorrelationId = correlationId;
            if (!string.IsNullOrEmpty(correlationId))
                correlationId += ".";
            correlationId += Guid.NewGuid().ToString("n");
            messageContext.Properties.CorrelationId = correlationId;

            var disposable = new Disposable();
            disposable.Disposing += (sender, e) => 
                messageContext.Properties.CorrelationId = originalCorrelationId;
            return disposable;
        }

        /// <summary>
        /// Disposes the dispatcher.
        /// </summary>
        protected override void OnDisposing()
        {
            channel.Returned -= OnChannelReturned;
            thread.Dispose();
            channel.Dispose();
        }

        private void OnChannelReturned(object sender, ReturnedEventArgs e)
        {
            var returned = Returned;
            if (returned != null)
                returned(this, e);
        }

        private void OnReceived(object sender, ReceivedEventArgs e)
        {
            Log.DebugFormat("Read {0} bytes from queue; CorrelationId = {1}.", e.MessageContext.BodyLength, e.MessageContext.Properties.CorrelationId);

            Interlocked.Increment(ref dispatcherDepth);
            thread.InvokeAsync(() =>
            {
                MicroServiceBinding binding = null;
                Log.DebugFormat("Identifying micro-service invocation details; CorrelationId = {0}.", e.MessageContext.Properties.CorrelationId);
                if (channel.CanPause)
                    channel.IsPaused = Interlocked.Read(ref dispatcherDepth) > 10;

                var messageContextExit = messageContextAccess.Enter(e.MessageContext);
                try
                {
                    var consumer = (IMqConsumer)sender;
                    var bindingsFromMap = bindingMap[consumer];
                    binding = bindingsFromMap.First(x => mqPlugin.IsMatch(e.MessageContext, x.Address));

                    using (var instance = microServiceLocator.Get(binding))
                    {
                        var parameterTypes = binding.Method.GetParameters().Select(x => x.ParameterType).ToArray();
                        object[] parameterValues;
                        using (var bodyStream = e.MessageContext.GetBodyStream())
                            parameterValues =
                                microServiceSerializer.Deserialize(
                                    e.MessageContext.Properties.ContentType,
                                    parameterTypes,
                                    bodyStream);

                        Log.DebugFormat(
                            "Invoking micro-service; Address = {0}, IsOneWay = {1}, Method = {2}, Parameters = {3}.",
                            binding.Address,
                            binding.IsOneWay,
                            binding.Method,
                            string.Join(", ", parameterValues));

                        var returnValue = instance.Invoke(parameterValues);
                        if (e.MessageContext.Properties.ReplyTo != null && !binding.IsOneWay)
                        {
                            var body = microServiceSerializer.Serialize(
                                e.MessageContext.Properties.ContentType,
                                binding.Method.ReturnType,
                                returnValue);
                            var replyToAddress =
                                channel.Plugin.ToPublisherAddress(e.MessageContext.Properties.ReplyTo);

                            Log.DebugFormat(
                                "Sending reply message; ReplyTo = {0}, ContentType = {1}, CorrelationId = {2}, Body.Length = {3}, Value = {4}.",
                                e.MessageContext.Properties.ReplyTo,
                                e.MessageContext.Properties.ContentType,
                                e.MessageContext.Properties.CorrelationId,
                                body.Length,
                                returnValue);
                            using (messageContextAccess.Enter())
                            using (var publisher = channel.CreatePublisher(replyToAddress))
                            {
                                var properties = messageContextAccess.Current.Properties;
                                properties.ReplyTo = null;
                                publisher.Publish(body, properties);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    var isHandled = false;
                    var unhandledException = UnhandledException;
                    if (unhandledException != null)
                    {
                        var unhandledExceptionArgs = new UnhandledExceptionEventArgs(ex);
                        unhandledException(this, unhandledExceptionArgs);
                        isHandled = unhandledExceptionArgs.IsHandled;
                    }

                    if (!isHandled)
                    {
                        var logBuilder = new StringBuilder().AppendFormat("Unexpected failure during micro-service operation; Cause = {0}: {1}.", ex.GetType().Name, ex.Message.TrimEnd('.'));
                        if (e.MessageContext.Properties.ReplyTo != null && binding != null && !binding.IsOneWay)
                        {
                            Log.Warn(logBuilder, ex);

                            var replyToAddress = channel.Plugin.ToPublisherAddress(e.MessageContext.Properties.ReplyTo);
                            using (var publisher = channel.CreatePublisher(replyToAddress))
                            {
                                var properties = messageContextAccess.Current.Properties;
                                properties.ReplyTo = null;
                                var body = microServiceSerializer.Serialize(properties.ContentType, ex.GetType(), ex);
                                publisher.Publish(body, properties);
                            }
                        }
                        else
                        {
                            logBuilder.Replace(
                                "; Cause = ",
                                string.Format(
                                    "; ReplyTo = {0}, IsOneWay = {1}, Cause =",
                                    e.MessageContext.Properties.ReplyToString ?? "(null)",
                                    binding != null ? binding.IsOneWay.ToString() : "Unknown"));
                            Log.Error(logBuilder, ex);
                        }
                    }
                }
                finally
                {
                    var depth = Interlocked.Decrement(ref dispatcherDepth);
                    if (channel.CanPause)
                        channel.IsPaused = 10 < depth;

                    messageContextExit.Dispose();
                    // messageContext.ExitAll();
                }
            });
        }
    }
}