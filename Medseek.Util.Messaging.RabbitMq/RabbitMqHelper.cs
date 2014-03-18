namespace Medseek.Util.Messaging.RabbitMq
{
    using Medseek.Util.Ioc;
    using RabbitMQ.Client;

    /// <summary>
    /// Provides helper functionality for working with messaging and RabbitMQ.
    /// </summary>
    [Register(typeof(IRabbitMqHelper))]
    public class RabbitMqHelper : IRabbitMqHelper
    {
        /// <summary>
        /// Gets a basic properties object set with the values from a message
        /// properties object.
        /// </summary>
        public IBasicProperties CreateBasicProperties(IModel model, MessageProperties properties)
        {
            var basicProperties = model.CreateBasicProperties();
            if (properties.CorrelationId != null)
                basicProperties.CorrelationId = properties.CorrelationId;
            if (properties.ReplyTo != null)
                basicProperties.ReplyTo = ToReplyToString(properties.ReplyTo);
            return basicProperties;
        }

        /// <summary>
        /// Gets the exchange associated with the messaging system address.
        /// </summary>
        public string Exchange(MqAddress address)
        {
            // TODO: Extract routing key from a uri-like value.
            return string.Empty;
        }

        /// <summary>
        /// Gets the routing key associated with the messaging system address.
        /// </summary>
        public string RoutingKey(MqAddress address)
        {
            // TODO: Make sure routing key is getting pulled out of a uri-like value.
            return address != null ? address.Value : null;
        }

        /// <summary>
        /// Gets a message properties object set with the values from a
        /// RabbitMQ basic properties object.
        /// </summary>
        public MessageProperties ToProperties(IBasicProperties basicProperties)
        {
            return new MessageProperties
            {
                CorrelationId = basicProperties.CorrelationId,
                ReplyTo = ToReplyToAddress(basicProperties.ReplyTo),
            };
        }

        private static MqAddress ToReplyToAddress(string value)
        {
            // TODO: Create address from a uri-like value.
            return value != null ? new MqAddress(value) : null;
        }

        private string ToReplyToString(MqAddress address)
        {
            // TODO: Return a uri-like value for reply-to.
            return RoutingKey(address);
        }
    }
}