namespace Medseek.Util.Messaging
{
    using System.Collections.Generic;

    /// <summary>
    /// Describes additional properties associated with a message.
    /// </summary>
    public class MessageProperties : IMessageProperties
    {
        /// <summary>
        /// Gets or sets the correlation identifier associated with the message.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the location to which reply messages should be
        /// published.
        /// </summary>
        public MqAddress ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the routing key associated with the message.
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// Gets or sets the additional properties dictionary.
        /// </summary>
        public Dictionary<string, object> AdditionalProperties 
        { 
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }
        private Dictionary<string, object> _additionalProperties = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets a value in the additional properties dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        public object this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
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
        public object Get(string id)
        {
            object value;
            return AdditionalProperties.TryGetValue(id, out value) ? value : null;
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
        public void Set(string id, object value)
        {
            AdditionalProperties[id] = value;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var result = new MessageProperties();
            foreach (var entry in AdditionalProperties)
                result[entry.Key] = entry.Value;
            result.RoutingKey = RoutingKey;
            return result;
        }
    }
}