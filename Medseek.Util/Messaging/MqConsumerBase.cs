namespace Medseek.Util.Messaging
{
    using System;
    using System.IO;
    using System.Linq;
    using Medseek.Util.Logging;

    /// <summary>
    /// Provides common functionality to derived message consumer types.
    /// </summary>
    public abstract class MqConsumerBase : MqSynchronizedDisposable, IMqConsumer
    {
        private readonly MqAddress address;
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqConsumerBase" /> 
        /// class.
        /// </summary>
        protected MqConsumerBase(MqAddress address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            this.address = address;
            log = LogManager.GetLogger(GetType());
            log.DebugFormat("Creating the consumer; Address = {0}.", address);
        }

        /// <summary>
        /// Raised to indicate that a message has been read from the channel 
        /// and is ready to be processed by the application.
        /// </summary>
        public event EventHandler<ReceivedEventArgs> Received;

        /// <summary>
        /// Gets the address from which messages are received by the consumer.
        /// </summary>
        public MqAddress Address
        {
            get
            {
                return address;
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
            log.DebugFormat("Disposing the consumer; Address = {0}.", address);
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
                log.DebugFormat("Received a message with {0} bytes; Address = {1}, CorrelationId = {2}, ReplyTo = {3}.", count, address, properties.CorrelationId, properties.ReplyTo);
                var received = Received;
                if (received != null)
                {
                    var handlers = received.GetInvocationList().Cast<EventHandler<ReceivedEventArgs>>();
                    foreach (var handler in handlers)
                    {
                        var ms = new MemoryStream(body, offset, count, false);
                        handler(this, new ReceivedEventArgs(ms, properties));
                    }
                }
            }
        }
    }
}