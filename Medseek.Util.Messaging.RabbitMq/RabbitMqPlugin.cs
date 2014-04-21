namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Medseek.Util.Interactive;
    using Medseek.Util.Ioc;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// Provides helper functionality for working with messaging and RabbitMQ.
    /// </summary>
    public class RabbitMqPlugin : ComponentsInstallable, IRabbitMqPlugin
    {
        /// <summary>
        /// The name of a default RabbitMQ connection component.
        /// </summary>
        public const string DefaultConnection = "Medseek.Util.Messaging.RabbitMq.RabbitMqConnection.Default";

        /// <summary>
        /// The name of a default RabbitMQ connection factory component.
        /// </summary>
        public const string DefaultConnectionFactoryName = "Medseek.Util.Messaging.RabbitMq.RabbitMqConnectionFactory.Default";

        private static readonly ConnectionFactory ConnectionFactory = new ConnectionFactory();

        /// <summary>
        /// Gets the default connection factory used to create connections.
        /// </summary>
        public static ConnectionFactory DefaultConnectionFactory
        {
            get
            {
                return ConnectionFactory;
            }
        }

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

            if (properties.AdditionalProperties != null)
            {
                if (basicProperties.Headers == null)
                    basicProperties.Headers = new Dictionary<string, object>();
                properties.AdditionalProperties
                    .ForEach(p => basicProperties.Headers[p.Key] = p.Value);
            }

            return basicProperties;
        }

        /// <summary>
        /// Determines whether the properties of a message imply that the 
        /// message is a match for the patterns bound to a consumer address.
        /// </summary>
        /// <param name="properties">
        /// The properties of the message.
        /// </param>
        /// <param name="address">
        /// The consumer address.
        /// </param>
        /// <returns>
        /// A value indicating whether the message is a match for the address.
        /// </returns>
        public bool IsMatch(MessageProperties properties, MqAddress address)
        {
            var ra = address as RabbitMqAddress ?? ToRabbitMqAddress(address);
            switch (ra.ExchangeType)
            {
                case "direct":
                    return properties.RoutingKey == ra.RoutingKey;

                case "topic":
                    var regex = new StringBuilder(ra.RoutingKey.Length + 16)
                        .Append("^")
                        .Append(ra.RoutingKey)
                        .Replace(".", @"\.")
                        .Replace("*", @"[^\.]*")
                        .Replace("#", @".*")
                        .Append("$")
                        .ToString();

                    var result = Regex.IsMatch(properties.RoutingKey, regex);
                    return result;

                default:
                    throw new ArgumentException("Unsupported exchange type " + ra.ExchangeType + ".");
            }
        }

        /// <summary>
        /// Converts an address into a detailed consumer oriented address 
        /// representing same original address.
        /// </summary>
        /// <param name="address">
        /// The address to convert.
        /// </param>
        /// <returns>
        /// A detailed consumer oriented address.
        /// </returns>
        public MqConsumerAddress ToConsumerAddress(MqAddress address)
        {
            var result = ToRabbitMqAddress(address);
            return result;
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

        /// <summary>
        /// Converts an address into a detailed publisher oriented address 
        /// representing same original address.
        /// </summary>
        /// <param name="address">
        /// The address to convert.
        /// </param>
        /// <returns>
        /// A detailed publisher oriented address.
        /// </returns>
        public MqPublisherAddress ToPublisherAddress(MqAddress address)
        {
            var ra = ToRabbitMqAddress(address);
            var result = new MqPublisherAddress(ra.Value, ra.RoutingKey);
            return result;
        }

        /// <summary>
        /// Converts an address into a RabbitMQ specific address.
        /// </summary>
        public RabbitMqAddress ToRabbitMqAddress(MqAddress address)
        {
            var ra = address as RabbitMqAddress;
            var result = ra ?? RabbitMqAddress.Parse(address.Value);
            return result;
        }

        /// <summary>
        /// Converts the data for a RabbitMQ returned message notification into
        /// a general representation of the notification.
        /// </summary>
        public ReturnedEventArgs ToReturnedEventArgs(string exchangeType, BasicReturnEventArgs e)
        {
            var address = new RabbitMqAddress(exchangeType, e.Exchange, e.RoutingKey);
            var body = new ArraySegment<byte>(e.Body);
            var properties = ToProperties(e.BasicProperties);
            var result = new ReturnedEventArgs(address, body, properties, e.ReplyCode, e.ReplyText);
            return result;
        }

        /// <summary>
        /// Returns the collection of installable types associated with the 
        /// subclass assembly.
        /// </summary>
        protected override IEnumerable<IInstallable> GetInstallables()
        {
            var installables = base.GetInstallables().ToList();
            installables.Add(new Registration
            {
                Services = new[] { typeof(IRabbitMqPlugin), typeof(IMqPlugin) },
                Instance = this,
            });
            installables.Add(new Registration
            {
                Services = new[] { typeof(ConnectionFactory) },
                Name = DefaultConnectionFactoryName,
                Instance = DefaultConnectionFactory,
                Lifestyle = Lifestyle.Singleton,
                IsDefault = true,
            });
            installables.Add(new Registration
            {
                Services = new[] { typeof(IMqConnection) },
                Name = DefaultConnection,
                Implementation = typeof(RabbitMqConnection),
                Lifestyle = Lifestyle.Singleton,
                IsDefault = true,
            });

            return installables;
        }
    }
}