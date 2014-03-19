namespace Medseek.Util.Messaging.ActiveMq
{
    using System;
    using Apache.NMS;
    using Medseek.Util.Ioc;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Provides a channel for interacting with an ActiveMQ
    /// </summary>
    [Register(typeof(IMqChannel), Lifestyle = Lifestyle.Transient)]
    public class ActiveMqChannel : MqChannelBase
    {
        private readonly ISession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveMqChannel" /> 
        /// class.
        /// </summary>
        public ActiveMqChannel(IConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            session = connection.CreateSession();
        }

        /// <summary>
        /// Creates a consumer that can be used to receive messages from the 
        /// messaging system channel.
        /// </summary>
        protected override IMqConsumer OnCreateConsumer(MqAddress address, bool autoDelete)
        {
            var consumer = new ActiveMqConsumer(session, address, autoDelete);
            return consumer;
        }

        /// <summary>
        /// Creates a publisher that can be used to send messages over to the 
        /// messaging system channel.
        /// </summary>
        protected override IMqPublisher OnCreatePublisher(MqAddress address)
        {
            var publisher = new ActiveMqPublisher(session, address);
            return publisher;
        }

        /// <summary>
        /// Disposes the messaging system channel.
        /// </summary>
        protected override void OnDisposingChannel()
        {
            session.Dispose();
        }
    }
}