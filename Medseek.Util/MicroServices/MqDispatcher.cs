namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using Medseek.Util.Messaging;
    using Medseek.Util.Threading;

    /// <summary>
    /// Provides a message queue event dispatcher.
    /// </summary>
    [Register(Start = true)]
    public class MqDispatcher : IStartable, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<IMqConsumer, MicroServiceDescriptor> descriptorMap = new Dictionary<IMqConsumer, MicroServiceDescriptor>();
        private readonly Dictionary<Type, DataContractSerializer> serializers = new Dictionary<Type, DataContractSerializer>();
        private readonly IMqChannel channel;
        private readonly IMicroServiceLocator microServiceLocator;
        private readonly IDispatchedThread thread;
        private long dispatcherDepth;
        private bool disposed;
        private bool started;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqDispatcher" /> 
        /// class.
        /// </summary>
        public MqDispatcher(
            IMqConnection connection, 
            IMicroServiceLocator microServiceLocator,
            IDispatchedThread thread)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (microServiceLocator == null)
                throw new ArgumentNullException("microServiceLocator");
            if (thread == null)
                throw new ArgumentNullException("thread");

            channel = connection.CreateChannnel();
            this.microServiceLocator = microServiceLocator;
            this.thread = thread;
            thread.Name = "MicroServices.Dispatch";
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            thread.Start();
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);
            if (started)
                throw new InvalidOperationException();

            Log.DebugFormat("{0}", MethodBase.GetCurrentMethod().Name);
            started = true;
            
            microServiceLocator.Initialize();
            if (!thread.IsAlive)
                thread.Start();

            thread.Invoke(() =>
            {
                foreach (var descriptor in microServiceLocator.Descriptors)
                {
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
                if (channel.CanPause)
                    channel.IsPaused = Interlocked.Read(ref dispatcherDepth) > 10;
                try
                {
                    var descriptor = descriptorMap[(IMqConsumer)sender];
                    var instance = microServiceLocator.Get(descriptor.Contract);
                    var method = descriptor.DefaultMethod;
                    var parameterType = method.GetParameters().Single().ParameterType;
                    var parameterValue = Deserialize(parameterType, e.Body);
                    var invokeResult = instance.InvokeDefault(parameterValue);
                    if (e.Properties.ReplyTo != null)
                    {
                        var body = Serialize(method.ReturnType, invokeResult);
                        using (var publisher = channel.CreatePublisher(e.Properties.ReplyTo))
                            publisher.Publish(body, e.Properties.CorrelationId);
                    }

                    //channel.BasicAck(e.DeliveryTag, false);
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
            DataContractSerializer serializer;
            if (!serializers.TryGetValue(type, out serializer))
                serializers[type] = serializer = new DataContractSerializer(type);

            var result = serializer.ReadObject(data);
            return result;
        }

        private byte[] Serialize(Type type, object obj)
        {
            if (type == typeof(void))
                return new byte[0];

            DataContractSerializer serializer;
            if (!serializers.TryGetValue(type, out serializer))
                serializers[type] = serializer = new DataContractSerializer(type);

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return ms.ToArray();
            }
        }
    }
}