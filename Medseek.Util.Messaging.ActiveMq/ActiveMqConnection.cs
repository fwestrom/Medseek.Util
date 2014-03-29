namespace Medseek.Util.Messaging.ActiveMq
{
    using System;
    using Apache.NMS;
    using Medseek.Util.Ioc;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Provides an AMQP connection by wrapping an instance of <see 
    /// cref="Apache.NMS.IConnection" /> that is obtained from the connection 
    /// factory.
    /// </summary>
    [Register(typeof(IMqConnection), Lifestyle = Lifestyle.Transient)]
    public class ActiveMqConnection : MqConnectionBase
    {
        private readonly IConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveMqConnection" 
        /// /> class.
        /// </summary>
        public ActiveMqConnection(IConnectionFactory connectionFactory)
            : base(null)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException("connectionFactory");

            connection = connectionFactory.CreateConnection();
        }

        /// <summary>
        /// Creates a new channel within the connection that can be used to 
        /// interact with the messaging system.
        /// </summary>
        /// <returns>
        /// A new channel associated with the connection.
        /// </returns>
        protected override IMqChannel OnCreateChannel()
        {
            if (!connection.IsStarted)
                connection.Start();

            var channel = new ActiveMqChannel(connection);
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