namespace Medseek.Util.Messaging
{
    using System;
    using System.IO;

    /// <summary>
    /// Event data describing a message received notification.
    /// </summary>
    public class ReceivedEventArgs : EventArgs
    {
        private readonly ArraySegment<byte> body;
        private readonly MessageProperties properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedEventArgs" /> 
        /// class.
        /// </summary>
        public ReceivedEventArgs(ArraySegment<byte> body, MessageProperties properties)
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
        [Obsolete("Use the GetBodyStream method instead.")]
        public Stream Body
        {
            get
            {
                return GetBodyStream();
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

        /// <summary>
        /// Gets an array containing the message body data.
        /// </summary>
        public byte[] GetBodyArray()
        {
            if (body.Offset == 0 && body.Count == body.Array.Length)
                return body.Array;

            using (var ms = new MemoryStream(body.Array, body.Offset, body.Count, false))
                return ms.ToArray();
        }

        /// <summary>
        /// Gets a stream that can be used to read the message body data.
        /// </summary>
        public Stream GetBodyStream()
        {
            return new MemoryStream(body.Array, body.Offset, body.Count, false);
        }
    }
}