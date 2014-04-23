namespace Medseek.Util.Messaging.RabbitMq
{
    using RabbitMQ.Client;

    /// <summary>
    /// Interface for RabbitMQ client connection components.
    /// </summary>
    public interface IRabbitMqConnection : IMqConnection
    {
        /// <summary>
        /// Creates a new RabbitMQ client library model object.
        /// </summary>
        IModel CreateModel();
    }
}