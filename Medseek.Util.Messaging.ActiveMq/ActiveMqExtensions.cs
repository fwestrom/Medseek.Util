namespace Medseek.Util.Messaging.ActiveMq
{
    using Apache.NMS;
    using Apache.NMS.ActiveMQ.Commands;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Provides extension methods for working with ActiveMQ messaging 
    /// components.
    /// </summary>
    public static class ActiveMqExtensions
    {
        /// <summary>
        /// Gets the message properties.
        /// </summary>
        public static MessageProperties GetProperties(this IMessage message)
        {
            return new MessageProperties
            {
                CorrelationId = message.NMSCorrelationID,
                ReplyTo = message.NMSReplyTo.ToAddress(),
            };
        }

        /// <summary>
        /// Sets the message properties.
        /// </summary>
        public static IMessage SetProperties(this IMessage message, MessageProperties properties)
        {
            message.NMSCorrelationID = properties.CorrelationId;
            message.NMSReplyTo = properties.ReplyTo.ToDestination();
            return message;
        }

        /// <summary>
        /// Converts an ActiveMQ destination description object to a messaging 
        /// system address.
        /// </summary>
        public static MqAddress ToAddress(this IDestination destination)
        {
            var destinationQueue = (ActiveMQQueue)destination;
            var address = new MqAddress(destinationQueue.QueueName);
            return address;
        }

        /// <summary>
        /// Converts a messaging system address to an ActiveMQ destination 
        /// description object.
        /// </summary>
        public static IDestination ToDestination(this MqAddress address)
        {
            var value = new ActiveMQQueue(address.Value);
            return value;
        }
    }
}