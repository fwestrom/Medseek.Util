namespace Medseek.Util.Messaging.RabbitMq
{
    using Medseek.Util.Ioc;

    using RabbitMQ.Client;

    [Register(typeof(IConnectionFactoryWrapper))]
    public class ConnectionFactoryWrapper : IConnectionFactoryWrapper
    {
        public IConnection CreateConnection()
        {
            return new ConnectionFactory().CreateConnection();
        }
    }
}