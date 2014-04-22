namespace Medseek.Util.Messaging.RabbitMq
{
    using Medseek.Util.MicroServices;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// Interface for types that provide helper functionality for working with 
    /// RabbitMQ and messaging system components.
    /// </summary>
    public interface IRabbitMqPlugin : IMqPlugin
    {
        /// <summary>
        /// Gets a basic properties object set with the values from a message
        /// properties object.
        /// </summary>
        IBasicProperties CreateBasicProperties(IModel model, MessageProperties properties);

        /// <summary>
        /// Gets a message context object set with the values from a
        /// RabbitMQ basic deliver event notification data object.
        /// </summary>
        IMessageContext ToMessageContext(BasicDeliverEventArgs e);

        /// <summary>
        /// Gets a message properties object set with the values from a
        /// RabbitMQ basic properties object.
        /// </summary>
        MessageProperties ToProperties(IBasicProperties basicProperties);

        /// <summary>
        /// Gets a publication address from the messaging system address.
        /// </summary>
        PublicationAddress ToPublicationAddress(MqAddress address);

        /// <summary>
        /// Converts an address into a RabbitMQ specific address.
        /// </summary>
        RabbitMqAddress ToRabbitMqAddress(MqAddress address);

        /// <summary>
        /// Converts the data for a RabbitMQ returned message notification into
        /// a general representation of the notification.
        /// </summary>
        ReturnedEventArgs ToReturnedEventArgs(string exchangeType, BasicReturnEventArgs e);
    }
}