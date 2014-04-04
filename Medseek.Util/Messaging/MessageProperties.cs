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
                string value;
                return properties.TryGetValue("correlation_id", out value)
                    ? value
                    : null;
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
                string value;
                return replyTo ?? (properties.TryGetValue("reply_to", out value) 
                    ? replyTo = new MqAddress(value) 
                    : null);
            }
            set
            {
                replyTo = value;
                properties["reply_to"] = value != null ? value.ToString() : null;
            }
        }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        public string ContentType
        {
            get
            {
                string value;
                return properties.TryGetValue("content_type", out value)
                    ? value
                    : null;
            }
            set
            {
                properties["content_type"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the routing key associated with the message.
        /// </summary>
        public string RoutingKey
        {
            get; 
            set;
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

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            var result = (MessageProperties)MemberwiseClone();
            result.properties.Clear();
            foreach (var entry in properties)
                properties[entry.Key] = entry.Value;
            return result;
        }
    }
}