namespace Medseek.Util.Messaging.RabbitMq
{
    using System;

    /// <summary>
    /// Describes a RabbitMQ specific messaging system address.
    /// </summary>
    public class RabbitMqAddress : MqConsumerAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqAddress" /> 
        /// class.
        /// </summary>
        public RabbitMqAddress(string exchangeType, string exchangeName, string routingKey, string queueName = null)
            : base(ToString(exchangeType, exchangeName, routingKey, queueName), queueName)
        {
            ExchangeType = exchangeType;
            ExchangeName = exchangeName;
            RoutingKey = routingKey;
        }

        public string ExchangeType
        {
            get;
            set;
        }

        public string ExchangeName
        {
            get;
            set;
        }

        public string RoutingKey
        {
            get;
            set;
        }

        public string QueueName
        {
            get
            {
                return (string)SourceKey;
            }
        }

        /// <summary>
        /// Parses a text string into a RabbitMQ messaging system address.
        /// </summary>
        public static RabbitMqAddress Parse(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            var index = text.IndexOf("://", StringComparison.Ordinal);
            if (index < 0)
                return new RabbitMqAddress("direct", string.Empty, text, text);

            var remain = text.Substring(index + 3);
            var parts = remain.Split('/');
            if (parts.Length < 2)
                throw new ArgumentException("Unexpected address format.", "text");

            return new RabbitMqAddress(text.Substring(0, index), parts[0], parts[1], parts.Length > 2 ? parts[2] : string.Empty);
        }

        private static string ToString(string exchangeType, string exchangeName, string routingKey, string queueName)
        {
            var result = string.Format("{0}://{1}/{2}", exchangeType, exchangeName, routingKey);
            if (!string.IsNullOrWhiteSpace(queueName))
                result += "/" + queueName;
            return result;
        }
    }
}