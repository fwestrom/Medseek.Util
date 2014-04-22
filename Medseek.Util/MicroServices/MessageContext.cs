namespace Medseek.Util.MicroServices
{
    using System;
    using System.IO;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Provides information about the current context for executing 
    /// micro-service operations.
    /// </summary>
    public class MessageContext : IMessageContext, ICloneable
    {
        private readonly byte[] body;
        private readonly MessageProperties properties;
        private readonly string routingKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageContext"/> class.
        /// </summary>
        public MessageContext(byte[] body, string routingKey, MessageProperties properties)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            this.body = body;
            this.properties = properties;
            this.routingKey = routingKey;
        }

        /// <summary>
        /// Gets the size in bytes of the message body.
        /// </summary>
        public virtual int BodyLength
        {
            get
            {
                return body.Length;
            }
        }

        /// <summary>
        /// Gets the message properties.
        /// </summary>
        public virtual MessageProperties Properties
        {
            get
            {
                return properties;
            }
        }

        /// <summary>
        /// Gets the routing key associated with the message context.
        /// </summary>
        public virtual string RoutingKey
        {
            get
            {
                return routingKey;
            }
        }

        /// <summary>
        /// Creates an independent copy of the message context.
        /// </summary>
        /// <returns>
        /// The new message context that was created from the original.
        /// </returns>
        public virtual IMessageContext Clone(bool includeAllProperties)
        {
            var cloneProperties = properties.Clone();
            var cloneBody = includeAllProperties ? body : null;
            var cloneRoutingKey = includeAllProperties ? routingKey : null;
            var result = new MessageContext(cloneBody, cloneRoutingKey, cloneProperties);
            return result;
        }

        /// <summary>
        /// Gets an array containing the message body data.
        /// </summary>
        public virtual byte[] GetBodyArray()
        {
            return body;
        }

        /// <summary>
        /// Gets a stream that can be used to read the message body data.
        /// </summary>
        public virtual Stream GetBodyStream()
        {
            return new MemoryStream(body, false);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        object ICloneable.Clone()
        {
            return Clone(false);
        }
    }
}