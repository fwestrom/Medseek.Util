namespace Medseek.Util.Messaging.RabbitMq
{
    using RabbitMQ.Client;

    /// <summary>
    /// Interface for types that provide helper functionality for working with 
    /// RabbitMQ and messaging system components.
    /// </summary>
    public interface IRabbitMqHelper
    {
        /// <summary>
        /// Gets a basic properties object set with the values from a message
        /// properties object.
        /// </summary>
        IBasicProperties CreateBasicProperties(IModel model, MessageProperties properties);

        /// <summary>
        /// Gets the exchange associated with the messaging system address.
        /// </summary>
        string Exchange(MqAddress address);

        /// <summary>
        /// Gets the routing key associated with the messaging system address.
        /// </summary>
        string RoutingKey(MqAddress address);

        /// <summary>
        /// Gets a message properties object set with the values from a
        /// RabbitMQ basic properties object.
        /// </summary>
        MessageProperties ToProperties(IBasicProperties basicProperties);
    }
}