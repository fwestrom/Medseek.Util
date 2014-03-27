namespace Medseek.Util.Messaging.RabbitMq
{
    using System.Linq;
    using System.Text;

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
            if (properties.ContentType != null)
                basicProperties.ContentType = properties.ContentType;
            if (properties.CorrelationId != null)
                basicProperties.CorrelationId = properties.CorrelationId;
            if (properties.ReplyTo != null)
                basicProperties.ReplyTo = ToPublicationAddress(properties.ReplyTo).ToString();
            return basicProperties;
        }

        /// <summary>
        /// Gets the name of the queue associated with the address.
        /// </summary>
        public string Queue(MqAddress address)
        {
            var parts = address.Value.Split('/');
            return parts.Length > 4 ? parts[4] : string.Empty;
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
                ReplyTo = new MqAddress(basicProperties.ReplyTo),
                ContentType = basicProperties.ContentType,
            };
        }

        /// <summary>
        /// Gets a publication address from the messaging system address.
        /// </summary>
        public PublicationAddress ToPublicationAddress(MqAddress address)
        {
            var parts = address.Value.Split('/');
            var text = string.Join("/", parts.Take(4));
            var result = PublicationAddress.Parse(text);
            return result;
        }
    }
}