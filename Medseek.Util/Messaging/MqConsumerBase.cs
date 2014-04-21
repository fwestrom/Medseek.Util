namespace Medseek.Util.Messaging
{
    using System;
    using System.Linq;
    using Medseek.Util.Logging;

    /// <summary>
    /// Provides common functionality to derived message consumer types.
    /// </summary>
    public abstract class MqConsumerBase : MqSynchronizedDisposable, IMqConsumer
    {
        private readonly MqAddress[] addresses;
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqConsumerBase" /> 
        /// class.
        /// </summary>
        protected MqConsumerBase(MqAddress[] addresses)
        {
            if (addresses == null)
                throw new ArgumentNullException("addresses");

            this.addresses = addresses;
            log = LogManager.GetLogger(GetType());
            log.DebugFormat("Creating the consumer; Addresses = {0}.", string.Join(", ", addresses.Select(x => x.ToString())));
        }

        /// <summary>
        /// Raised to indicate that a message has been read from the channel 
        /// and is ready to be processed by the application.
        /// </summary>
        public event EventHandler<ReceivedEventArgs> Received;

        /// <summary>
        /// Gets the address from which messages are received by the consumer.
        /// </summary>
        public MqAddress[] Addresses
        {
            get
            {
                return addresses;
            }
        }

        /// <summary>
        /// Disposes the messaging system consumer.
        /// </summary>
        protected abstract void OnDisposingConsumer();

        /// <summary>
        /// Disposes the messaging system disposable component.
        /// </summary>
        protected override void OnDisposingMqDisposable()
        {
            log.DebugFormat("Disposing the consumer; Addresses = {0}.", string.Join(", ", addresses.Select(x => x.ToString())));
            OnDisposingConsumer();
        }

        /// <summary>
        /// Raises the message received notification.
        /// </summary>
        /// <seealso cref="Received" />
        protected void RaiseReceived(byte[] body, int offset, int count, MessageProperties properties)
        {
            using (EnterDisposeLock(false))
            {
                log.DebugFormat("Received a message with {0} bytes; RoutingKey = {1}, CorrelationId = {2}, ReplyTo = {3}.", count, properties.RoutingKey, properties.CorrelationId, properties.ReplyTo);
                var received = Received;
                if (received != null)
                {
                    var handlers = received.GetInvocationList().Cast<EventHandler<ReceivedEventArgs>>();
                    foreach (var handler in handlers)
                    {
                        var bodySegment = new ArraySegment<byte>(body, offset, count);
                        var e = new ReceivedEventArgs(bodySegment, properties);
                        handler(this, e);
                    }
                }
            }
        }
    }
}