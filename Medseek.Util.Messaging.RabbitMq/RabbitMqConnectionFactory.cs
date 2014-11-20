namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Medseek.Util.Logging;
    using RabbitMQ.Client;

    /// <summary>
    /// Provides instances of components that correspond to connections using 
    /// the RabbitMQ client plugin.
    /// </summary>
    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ConnectionFactory factory = new ConnectionFactory();
        private readonly IEnvironment environment;
        private bool configureFromEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConnectionFactory"/> class.
        /// </summary>
        public RabbitMqConnectionFactory(IEnvironment environment, bool configureFromEnvironment = false)
        {
            if (environment == null)
                throw new ArgumentNullException("environment");

            this.configureFromEnvironment = configureFromEnvironment;
            this.environment = environment;
        }

        /// <summary>
        /// Gets or sets the username 
        /// </summary>
        public string UserName
        {
            get
            {
                return factory.UserName;
            }
            set
            {
                factory.UserName = value;
            }
        }

        /// <summary>
        /// Creates an object that can be used to interact with a new RabbitMQ 
        /// connection.
        /// </summary>
        /// <returns>
        /// An object that can be used to interact with the connection.
        /// </returns>
        public IConnection CreateConnection()
        {
            if (configureFromEnvironment)
            {
                configureFromEnvironment = false;

                var args = environment.GetCommandLineArgs();
                var brokerArgument = args.FirstOrDefault(c => c.StartsWith("/broker="));
                if (brokerArgument != null)
                {
                    var brokerSettings = brokerArgument.Split('=');
                    if (brokerSettings[1].StartsWith("amqp://"))
                    {
                        factory.Uri = brokerSettings[1];
                    }
                    else if (brokerSettings[1].Contains('/'))
                    {
                        var brokerSpec = brokerSettings[1].Split('/');
                        factory.HostName = brokerSpec[0];
                        factory.VirtualHost = brokerSpec[1];
                    }
                    else
                    {
                        factory.HostName = brokerSettings[1];
                    }
                }

                Log.InfoFormat("Configured from command line arguments; BrokerArgument = {0}, HostName = {1}, VirtualHost = {2}, UserName = {3}.", brokerArgument, factory.HostName, factory.VirtualHost, factory.UserName);
            }

            var connection = factory.CreateConnection();
            return connection;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return new StringBuilder(GetType().Name)
                .AppendFormat("{{ Ep = {0}", factory.Endpoint)
                .AppendFormat(", Vhost = {0}", factory.VirtualHost)
                .AppendFormat(", User = {0}", factory.UserName)
                .Append(" ... }")
                .ToString();
        }
    }
}