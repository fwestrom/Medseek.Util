namespace Medseek.Util.Messaging.ActiveMq
{
    using System;
    using Apache.NMS;
    using Medseek.Util.Ioc;
    using Medseek.Util.Messaging;

    /// <summary>
    /// A message publisher for working with ActiveMQ.
    /// </summary>
    [Register(typeof(IMqPublisher), Lifestyle = Lifestyle.Transient)]
    public class ActiveMqPublisher : MqPublisherBase
    {
        private readonly IMessageProducer producer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveMqPublisher"/> class.
        /// </summary>
        public ActiveMqPublisher(ISession session, MqAddress address)
            : base(address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            if (session == null)
                throw new ArgumentNullException("session");

            var destination = address.ToDestination();
            producer = session.CreateProducer(destination);
        }

        /// <summary>
        /// Publishes a message to the messaging system channel.
        /// </summary>
        /// <param name="body">
        /// An array containing the raw bytes of the message body.
        /// </param>
        /// <param name="properties">
        /// The properties associated with the message.
        /// </param>
        protected override void OnPublish(byte[] body, MessageProperties properties)
        {
            var message = producer.CreateBytesMessage(body);
            message.SetProperties(properties);
            producer.Send(message);
        }

        /// <summary>
        /// Disposes the messaging system publisher.
        /// </summary>
        protected override void OnDisposingPublisher()
        {
            producer.Dispose();
        }
    }
}