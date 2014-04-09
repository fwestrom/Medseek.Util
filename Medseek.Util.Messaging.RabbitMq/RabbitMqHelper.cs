namespace Medseek.Util.Messaging.RabbitMq
{
    using System.Diagnostics;
    using System.Text;
    using Medseek.Util.Interactive;
    using Medseek.Util.Ioc;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.Collections.Generic;

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
        public IBasicProperties CreateBasicProperties(IBasicProperties basicProperties, IMessageProperties properties)
        {
            if (properties.ContentType != null)
                basicProperties.ContentType = properties.ContentType;
            if (properties.CorrelationId != null)
                basicProperties.CorrelationId = properties.CorrelationId; 
            if (properties.ReplyTo != null)
                basicProperties.ReplyTo = ToPublicationAddress(properties.ReplyTo).ToString();
            if (basicProperties.Headers == null)
                basicProperties.Headers = new Dictionary<string, object>();

            // Add AdditionalProperties to the BasicProperties headers.
            properties.AdditionalProperties
                .ForEach(p => basicProperties.Headers[p.Key] = p.Value);
            return basicProperties; 
        }

        /// <summary>
        /// Gets a message properties object set with the values from a
        /// RabbitMQ basic properties object.
        /// </summary>
        public MessageProperties ToProperties(IBasicProperties basicProperties)
        {
            var properties = new MessageProperties
            {
                CorrelationId = basicProperties.CorrelationId,
                ReplyTo = basicProperties.ReplyTo != null ? new MqAddress(basicProperties.ReplyTo) : null,
                ContentType = basicProperties.ContentType,
            };

            if (basicProperties.Headers != null)
            {
                // Add Headers to the MessageProperties dictionary.
                basicProperties.Headers
                    .ForEach(h => properties[h.Key] = h.Value);
            }

            return properties;
        }

        /// <summary>
        /// Gets a message properties object set with the values from a
        /// RabbitMQ basic deliver event notification data object.
        /// </summary>
        public MessageProperties ToProperties(BasicDeliverEventArgs e)
        {
            var properties = ToProperties(e.BasicProperties);
            properties.RoutingKey = e.RoutingKey;
            return properties;
        }

        /// <summary>
        /// Gets a publication address from the messaging system address.
        /// </summary>
        public PublicationAddress ToPublicationAddress(MqAddress address)
        {
            var ra = ToRabbitMqAddress(address);
            var result = new PublicationAddress(ra.ExchangeType, ra.ExchangeName, ra.RoutingKey);
            return result;
        }

        public RabbitMqAddress ToRabbitMqAddress(MqAddress address)
        {
            var ra = address as RabbitMqAddress;
            var result = ra ?? RabbitMqAddress.Parse(address.Value);
            return result;
        }
    }
}