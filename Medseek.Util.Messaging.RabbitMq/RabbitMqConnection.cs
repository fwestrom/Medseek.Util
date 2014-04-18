namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using Medseek.Util.Ioc;
    using RabbitMQ.Client;

    /// <summary>
    /// Provides an AMQP connection by wrapping an instance of 
    /// RabbitMQ.Client.IConnection that is obtained from the connection 
    /// factory.
    /// </summary>
    [Register(typeof(IMqConnection), Lifestyle = Lifestyle.Transient)]
    public class RabbitMqConnection : MqConnectionBase
    {
        private readonly IConnection connection;
        private readonly IRabbitMqFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConnection"/> class.
        /// </summary>
        public RabbitMqConnection(ConnectionFactory connectionFactory, IRabbitMqFactory factory, IMqPlugin plugin)
            : base(plugin)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException("connectionFactory");
            if (factory == null)
                throw new ArgumentNullException("factory");
            Console.WriteLine("Connecting to RabbitMQ endpoint:[{0}] vhost:[{1}] user:[{2}]", connectionFactory.Endpoint, connectionFactory.VirtualHost, connectionFactory.UserName);
            connection = connectionFactory.CreateConnection();
            this.factory = factory;
        }

        /// <summary>
        /// Creates a new channel within the connection that can be used to 
        /// interact with the messaging system.
        /// </summary>
        protected override IMqChannel OnCreateChannel()
        {
            var channel = factory.GetRabbitMqChannel(connection);
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