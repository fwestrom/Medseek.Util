namespace Medseek.Util.MicroServices
{
    using System;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Provides information about the current context for executing 
    /// micro-service operations.
    /// </summary>
    public class MessageContext : IMessageContext
    {
        private readonly IMessageProperties properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageContext"/> class.
        /// </summary>
        public MessageContext(IMessageProperties properties)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            this.properties = properties;
        }

        /// <summary>
        /// Gets the message properties.
        /// </summary>
        public IMessageProperties Properties
        {
            get
            {
                return properties;
            }
        }

        /// <summary>
        /// Creates an independent copy of the message context.
        /// </summary>
        /// <returns>
        /// The new message context that was created from the original.
        /// </returns>
        public object Clone()
        {
            var cloneProperties = (IMessageProperties)properties.Clone();
            var result = new MessageContext(cloneProperties);
            return result;
        }
    }
}