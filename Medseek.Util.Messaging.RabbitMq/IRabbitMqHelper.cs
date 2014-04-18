namespace Medseek.Util.Messaging.RabbitMq
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

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
        /// Gets a message properties object set with the values from a
        /// RabbitMQ basic properties object.
        /// </summary>
        MessageProperties ToProperties(IBasicProperties basicProperties);

        /// <summary>
        /// Gets a message properties object set with the values from a
        /// RabbitMQ basic deliver event notification data object.
        /// </summary>
        MessageProperties ToProperties(BasicDeliverEventArgs e);

        /// <summary>
        /// Gets a publication address from the messaging system address.
        /// </summary>
        PublicationAddress ToPublicationAddress(MqAddress address);

        /// <summary>
        /// Converts an address into a RabbitMQ specific address.
        /// </summary>
        RabbitMqAddress ToRabbitMqAddress(MqAddress address);
    }
}