namespace Medseek.Util.Messaging
{
    using Medseek.Util.MicroServices;

    /// <summary>
    /// Event data describing a message that was returned as undeliverable.
    /// </summary>
    public class ReturnedEventArgs : ReceivedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnedEventArgs" /> 
        /// class.
        /// </summary>
        public ReturnedEventArgs(IMessageContext messageContext, MqAddress address, ushort replyCode, string replyText)
            : base(messageContext)
        {
            Address = address;
            ReplyCode = replyCode;
            ReplyText = replyText;
        }

        internal ReturnedEventArgs()
        {
        }

        /// <summary>
        /// Gets the address to which the message was originally published.
        /// </summary>
        public MqAddress Address
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the AMQP code identifying the reason for the return.
        /// </summary>
        public ushort ReplyCode
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the text description the reason for the return.
        /// </summary>
        public string ReplyText
        {
            get;
            internal set;
        }
    }
}