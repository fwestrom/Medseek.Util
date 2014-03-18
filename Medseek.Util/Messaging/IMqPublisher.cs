namespace Medseek.Util.Messaging
{
    /// <summary>
    /// Interface for types that can be used to publish messages to a messaging
    /// system channel.
    /// </summary>
    public interface IMqPublisher : IMqDisposable
    {
        /// <summary>
        /// Publishes a message to the messaging system channel.
        /// </summary>
        /// <param name="body">
        /// An array containing the raw bytes of the message body.
        /// </param>
        /// <param name="correlationId">
        /// An optional correlation identifier to associate with the message.
        /// </param>
        /// <param name="replyTo">
        /// An optional description of the location to which reply messages 
        /// should be published.
        /// </param>
        void Publish(byte[] body, string correlationId = null, MqAddress replyTo = null);
    }
}