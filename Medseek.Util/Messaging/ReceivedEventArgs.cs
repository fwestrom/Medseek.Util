namespace Medseek.Util.Messaging
{
    using System;
    using System.IO;

    /// <summary>
    /// Event data describing a message received notification.
    /// </summary>
    public class ReceivedEventArgs : EventArgs
    {
        private readonly Stream body;
        private readonly MessageProperties properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedEventArgs" /> 
        /// class.
        /// </summary>
        public ReceivedEventArgs(Stream body, MessageProperties properties)
        {
            if (body == null)
                throw new ArgumentNullException("body");
            if (properties == null)
                throw new ArgumentNullException("properties");

            this.body = body;
            this.properties = properties;
        }

        /// <summary>
        /// Gets a stream that can be used to read the message body.
        /// </summary>
        public Stream Body
        {
            get
            {
                return body;
            }
        }

        /// <summary>
        /// Gets the additional properties associated with the message.
        /// </summary>
        public MessageProperties Properties
        {
            get
            {
                return properties;
            }
        }
    }
}