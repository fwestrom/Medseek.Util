namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using Medseek.Util.Messaging;
    using Medseek.Util.Serialization;
    using Medseek.Util.Threading;

    /// <summary>
    /// Provides a micro-service dispatcher using a messaging system connection.
    /// </summary>
    [Register(Lifestyle = Lifestyle.Singleton, Start = true)]
    public class MicroServiceDispatcher : IStartable, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<IMqConsumer, MicroServiceDescriptor> descriptorMap = new Dictionary<IMqConsumer, MicroServiceDescriptor>();
        private readonly IMqChannel channel;
        private readonly IMicroServiceLocator microServiceLocator;
        private readonly ISerializer[] serializers;
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
            IMicroServiceLocator microServiceLocator,
            ISerializerFactory serializerFactory,
            IDispatchedThread thread)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (microServiceLocator == null)
                throw new ArgumentNullException("microServiceLocator");
            if (serializerFactory == null)
                throw new ArgumentNullException("serializerFactory");
            if (thread == null)
                throw new ArgumentNullException("thread");

            channel = connection.CreateChannnel();
            this.microServiceLocator = microServiceLocator;
            serializers = serializerFactory.GetAllSerializers();
            this.thread = thread;
            thread.Name = "MicroServices.Dispatch";
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

            thread.Invoke(() =>
            {
                foreach (var descriptor in microServiceLocator.Descriptors)
                {
                    Log.InfoFormat("Starting message consumer for {0}.", descriptor.Implementation);

                    var from = new MqAddress { Value = descriptor.RequestQueue };
                    var consumer = channel.CreateConsumer(from, false);
                    consumer.Received += OnReceived;
                    descriptorMap[consumer] = descriptor;
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
                foreach (var consumer in descriptorMap.Keys.ToArray())
                {
                    consumer.Dispose();
                    descriptorMap.Remove(consumer);
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
            Log.DebugFormat("Read {0} bytes from queue; CorrelationId = {1}.", e.Body.Length, e.Properties.CorrelationId);

            Interlocked.Increment(ref dispatcherDepth);
            thread.InvokeAsync(() =>
            {
                Log.DebugFormat("Identifying micro-service invocation details; CorrelationId = {0}.", e.Properties.CorrelationId);
                if (channel.CanPause)
                    channel.IsPaused = Interlocked.Read(ref dispatcherDepth) > 10;
                try
                {
                    var descriptor = descriptorMap[(IMqConsumer)sender];
                    var instance = microServiceLocator.Get(descriptor.Contract);
                    var method = descriptor.DefaultMethod;
                    var parameterType = method.GetParameters().Single().ParameterType;
                    var parameterValue = Deserialize(parameterType, e.Body);

                    Log.DebugFormat(
                        "Invoking micro-service; Instance = {0}, Method = {1}, Parameter = {2}.",
                        instance.Instance,
                        method,
                        parameterValue);
                    var invokeResult = instance.InvokeDefault(parameterValue);
                    if (e.Properties.ReplyTo != null)
                    {
                        Log.DebugFormat(
                            "Sending reply message; CorrelationId = {0}, ReplyTo = {1}, Response = {2}.",
                            e.Properties.CorrelationId,
                            e.Properties.ReplyTo,
                            invokeResult);
                        var body = Serialize(method.ReturnType, invokeResult);
                        using (var publisher = channel.CreatePublisher(e.Properties.ReplyTo))
                            publisher.Publish(body, e.Properties.CorrelationId);
                    }

                    //channel.BasicAck(e.DeliveryTag, false);
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

        private object Deserialize(Type type, Stream data)
        {
            var serializer = serializers.First(s => s.CanDeserialize(type, data));
            var result = serializer.Deserialize(type, data);
            return result;
        }

        private byte[] Serialize(Type type, object obj)
        {
            if (type == typeof(void))
                return new byte[0];

            var serializer = serializers.First(x => x.CanSerialize(type));
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(type, obj, ms);
                return ms.ToArray();
            }
        }
    }
}