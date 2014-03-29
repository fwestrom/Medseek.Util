namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Medseek.Util.Ioc;
    using RabbitMQ.Client;

    /// <summary>
    /// Provides information about the RabbitMQ utility components.
    /// </summary>
    public class RabbitMqComponents : ComponentsInstallable, IMqPlugin
    {
        /// <summary>
        /// The name of a default RabbitMQ connection component.
        /// </summary>
        public const string DefaultConnection = "Medseek.Util.Messaging.RabbitMq.RabbitMqConnection.Default";

        /// <summary>
        /// The name of a default RabbitMQ connection factory component.
        /// </summary>
        public const string DefaultConnectionFactoryName = "Medseek.Util.Messaging.RabbitMq.RabbitMqConnectionFactory.Default";

        private static readonly RabbitMqComponents ComponentsInstallable = new RabbitMqComponents();
        private static readonly ConnectionFactory ConnectionFactory = new ConnectionFactory();
        private readonly RabbitMqHelper helper = new RabbitMqHelper();

        /// <summary>
        /// Prevents a default instance of the <see 
        /// cref="RabbitMqComponents" /> class from being created.
        /// </summary>
        private RabbitMqComponents()
        {
        }

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
        /// Gets the plugin that provides pluggable functionality using the 
        /// Castle project integration components.
        /// </summary>
        public static RabbitMqComponents Plugin
        {
            get
            {
                return ComponentsInstallable;
            }
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
            var ra = address as RabbitMqAddress ?? helper.ToRabbitMqAddress(address);
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
            var result = helper.ToRabbitMqAddress(address);
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
                Services = new[] { typeof(IMqPlugin) },
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