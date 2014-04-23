namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using System.Reflection;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using RabbitMQ.Client;

    /// <summary>
    /// Provides an AMQP connection by wrapping an instance of 
    /// RabbitMQ.Client.IConnection that is obtained from the connection 
    /// factory.
    /// </summary>
    [Register(typeof(IRabbitMqConnection), typeof(IMqConnection), Lifestyle = Lifestyle.Transient)]
    public class RabbitMqConnection : MqConnectionBase, IRabbitMqConnection
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IConnection connection;
        private readonly IRabbitMqFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConnection"/> class.
        /// </summary>
        public RabbitMqConnection(IRabbitMqConnectionFactory connectionFactory, IRabbitMqFactory factory, IMqPlugin plugin)
            : base(plugin)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException("connectionFactory");
            if (factory == null)
                throw new ArgumentNullException("factory");

            Log.InfoFormat("Creating connection; Factory = {0}.", connectionFactory);
            connection = connectionFactory.CreateConnection();
            this.factory = factory;
        }

        /// <summary>
        /// Creates a new RabbitMQ client library model object.
        /// </summary>
        public IModel CreateModel()
        {
            return connection.CreateModel();
        }

        /// <summary>
        /// Creates a new channel within the connection that can be used to 
        /// interact with the messaging system.
        /// </summary>
        protected override IMqChannel OnCreateChannel()
        {
            var channel = factory.GetRabbitMqChannel(this);
            channel.Disposed += (sender, e) => factory.Release(channel);
            return channel;
        }

        /// <summary>
        /// Disposes the connection resources used by the connection.
        /// </summary>
        protected override void OnDisposingConnection()
        {
            connection.Dispose();
        }
    }
}