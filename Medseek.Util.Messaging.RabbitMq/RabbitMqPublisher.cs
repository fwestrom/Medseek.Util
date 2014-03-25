namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using Medseek.Util.Ioc;
    using RabbitMQ.Client;

    /// <summary>
    /// A message publisher for interacting with RabbitMQ.
    /// </summary>
    [Register(typeof(IMqPublisher), Lifestyle = Lifestyle.Transient)]
    public class RabbitMqPublisher : MqPublisherBase
    {
        private readonly IRabbitMqHelper helper;
        private readonly IModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqPublisher"/> class.
        /// </summary>
        public RabbitMqPublisher(MqAddress address, IRabbitMqHelper helper, IModel model)
            : base(address)
        {
            if (address == null)
                throw new ArgumentNullException("address");
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (model == null)
                throw new ArgumentNullException("model");

            this.helper = helper;
            this.model = model;
        }

        /// <summary>
        /// This method is not used.
        /// </summary>
        protected override void OnDisposingPublisher()
        {
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
            var basicProperties = helper.CreateBasicProperties(model, properties);
            var pa = helper.ToPublicationAddress(Address);
            model.BasicPublish(pa.ExchangeName, pa.RoutingKey, basicProperties, body);
        }
    }
}