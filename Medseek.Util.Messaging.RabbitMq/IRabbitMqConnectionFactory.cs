namespace Medseek.Util.Messaging.RabbitMq
{
    using RabbitMQ.Client;

    /// <summary>
    /// Interface for factories that can provide instances of components that 
    /// correspond to connections using the RabbitMQ client plugin.
    /// </summary>
    public interface IRabbitMqConnectionFactory
    {
        /// <summary>
        /// Creates an object that can be used to interact with a new RabbitMQ 
        /// connection.
        /// </summary>
        /// <returns>
        /// An object that can be used to interact with the connection.
        /// </returns>
        IConnection CreateConnection();
    }
}