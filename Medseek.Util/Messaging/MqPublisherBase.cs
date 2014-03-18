namespace Medseek.Util.Messaging
{
    using System;
    using Medseek.Util.Logging;

    /// <summary>
    /// Provides common functionality to derived message publisher types.
    /// </summary>
    public abstract class MqPublisherBase : MqSynchronizedDisposable, IMqPublisher
    {
        private readonly MqAddress address;
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqPublisherBase" /> 
        /// class.
        /// </summary>
        protected MqPublisherBase(MqAddress address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            this.address = address;
            log = LogManager.GetLogger(GetType());
            log.DebugFormat("Creating the publisher; Address = {0}.", address);
        }

        /// <summary>
        /// Gets the address to which messages are published.
        /// </summary>
        public MqAddress Address
        {
            get
            {
                return address;
            }
        }

        /// <summary>
        /// Publishes a message to the messaging system channel.
        /// </summary>
        /// <param name="body">
        /// An array containing the raw bytes of the message body.
        /// </param>
        /// <param name="correlationId">
        /// An optional correlation identifier to associate with the message.
        /// </param>
        /// <param name="replyTo">
        /// An optional description of the location to which reply messages 
        /// should be published.
        /// </param>
        public void Publish(byte[] body, string correlationId = null, MqAddress replyTo = null)
        {
            using (EnterDisposeLock())
            {
                log.DebugFormat("Publishing a message of {0} bytes; Address = {1}, CorrelationId = {2}, ReplyTo = {3}.", body.Length, address, correlationId, replyTo);
                OnPublish(body, new MessageProperties
                {
                    CorrelationId = correlationId,
                    ReplyTo = replyTo,
                });
            }
        }

        /// <summary>
        /// Disposes the messaging system disposable component.
        /// </summary>
        protected override void OnDisposingMqDisposable()
        {
            log.DebugFormat("Disposing the publisher; Address = {0}.", address);
            OnDisposingPublisher();
        }

        /// <summary>
        /// Disposes the messaging system publisher.
        /// </summary>
        protected abstract void OnDisposingPublisher();

        /// <summary>
        /// Publishes a message to the messaging system channel.
        /// </summary>
        /// <param name="body">
        /// An array containing the raw bytes of the message body.
        /// </param>
        /// <param name="properties">
        /// The properties associated with the message.
        /// </param>
        protected abstract void OnPublish(byte[] body, MessageProperties properties);
    }
}