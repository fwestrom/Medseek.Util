namespace Medseek.Util.Messaging
{
    using System;

    /// <summary>
    /// Event data describing a message that was returned as undeliverable.
    /// </summary>
    public class ReturnedEventArgs : ReceivedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnedEventArgs" /> 
        /// class.
        /// </summary>
        public ReturnedEventArgs(MqAddress address, ArraySegment<byte> body, MessageProperties properties, ushort replyCode, string replyText)
            : base(body, properties)
        {
            Address = address;
            ReplyCode = replyCode;
            ReplyText = replyText;
        }

        /// <summary>
        /// Gets the address to which the message was originally published.
        /// </summary>
        public MqAddress Address
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the AMQP code identifying the reason for the return.
        /// </summary>
        public ushort ReplyCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the text description the reason for the return.
        /// </summary>
        public string ReplyText
        {
            get;
            private set;
        }
    }
}