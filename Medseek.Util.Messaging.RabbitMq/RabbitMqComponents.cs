namespace Medseek.Util.Messaging.RabbitMq
{
    using System.Collections.Generic;
    using System.Linq;
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
        public const string DefaultConnectionFactoryName = "Medseek.Util.Messaging.RabbitMq.RabbitMqConnectionFactory.Default";

        private static readonly RabbitMqComponents ComponentsInstallable = new RabbitMqComponents();
        private static readonly ConnectionFactory ConnectionFactory = new ConnectionFactory();

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
        /// Returns the collection of installable types associated with the 
        /// subclass assembly.
        /// </summary>
        protected override IEnumerable<IInstallable> GetInstallables()
        {
            var installables = base.GetInstallables().ToList();
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