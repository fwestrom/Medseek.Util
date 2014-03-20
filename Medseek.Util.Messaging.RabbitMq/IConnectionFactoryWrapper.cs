namespace Medseek.Util.Messaging.RabbitMq
{
    using RabbitMQ.Client;

    public interface IConnectionFactoryWrapper
    {
        IConnection CreateConnection();
    }
}

