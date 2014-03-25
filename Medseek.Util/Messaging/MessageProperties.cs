namespace Medseek.Util.Messaging
{
    using System.Collections.Generic;

    /// <summary>
    /// Describes additional properties associated with a message.
    /// </summary>
    public class MessageProperties : IMessageProperties
    {
        private readonly Dictionary<string, string> properties = new Dictionary<string, string>();
        private MqAddress replyTo;

        /// <summary>
        /// Gets or sets the correlation identifier associated with the message.
        /// </summary>
        public string CorrelationId
        {
            get
            {
                return properties["correlation_id"];
            }
            set
            {
                properties["correlation_id"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the location to which reply messages should be
        /// published.
        /// </summary>
        public MqAddress ReplyTo
        {
            get
            {
                return replyTo;
            }
            set
            {
                replyTo = value;
                properties["reply_to"] = value != null ? value.ToString() : null;
            }
        }

        /// <summary>
        /// Gets the value of a property by its identifier, name, or key.
        /// </summary>
        /// <param name="id">
        /// The property identifier, name, or key.
        /// </param>
        /// <returns>
        /// The value of the property, or null if it was not present.
        /// </returns>
        public string Get(string id)
        {
            string value;
            return properties.TryGetValue(id, out value) ? value : null;
        }

        /// <summary>
        /// Sets the value of a property by its identifier, name, or key.
        /// </summary>
        /// <param name="id">
        /// The property identifier, name, or key.
        /// </param>
        /// <param name="value">
        /// The value to set for the property.
        /// </param>
        public void Set(string id, string value)
        {
            properties[id] = value;
        }
    }
}