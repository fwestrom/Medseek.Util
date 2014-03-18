namespace Medseek.Util.Messaging
{
    /// <summary>
    /// Describes additional properties associated with a message.
    /// </summary>
    public class MessageProperties
    {
        /// <summary>
        /// Gets or sets the correlation identifier associated with the message.
        /// </summary>
        public string CorrelationId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the location to which reply messages should be
        /// published.
        /// </summary>
        public MqAddress ReplyTo
        {
            get;
            set;
        }
    }
}