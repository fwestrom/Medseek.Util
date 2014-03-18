namespace Medseek.Util.Messaging.RabbitMq
{
    using Medseek.Util.Ioc;
    using RabbitMQ.Client;

    /// <summary>
    /// Interface for types that can provide instances of the RabbitMQ 
    /// messaging components.
    /// </summary>
    [RegisterFactory]
    public interface IRabbitMqFactory
    {
        /// <summary>
        /// Returns an instance of the messaging system channel component.
        /// </summary>
        IMqChannel GetRabbitMqChannel(IConnection connection);

        /// <summary>
        /// Returns an instance of the message consumer component.
        /// </summary>
        IMqConsumer GetRabbitMqConsumer(IModel model, MqAddress address, bool autoDelete);

        /// <summary>
        /// Returns an instance of the message publisher component.
        /// </summary>
        IMqPublisher GetRabbitMqPublisher(IModel model, MqAddress address);

        /// <summary>
        /// Releases a component that was previously obtained from the factory.
        /// </summary>
        void Release(IMqChannel component);

        /// <summary>
        /// Releases a component that was previously obtained from the factory.
        /// </summary>
        void Release(IMqConsumer component);

        /// <summary>
        /// Releases a component that was previously obtained from the factory.
        /// </summary>
        void Release(IMqPublisher component);
    }
}