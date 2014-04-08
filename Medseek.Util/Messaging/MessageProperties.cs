namespace Medseek.Util.Messaging
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Describes additional properties associated with a message.
    /// </summary>
    [DataContract(Namespace = "")]
    public class MessageProperties : IMessageProperties
    {
        private readonly Dictionary<string, object> additionalProperties = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        [DataMember]
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the correlation identifier associated with the message.
        /// </summary>
        [DataMember]
        public string CorrelationId
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the location to which reply messages should be
        /// published.
        /// </summary>
        [DataMember]
        public MqAddress ReplyTo
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the routing key associated with the message.
        /// </summary>
        [DataMember]
        public string RoutingKey
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the additional properties dictionary.
        /// </summary>
        [DataMember]
        public Dictionary<string, object> AdditionalProperties 
        {
            get
            {
                return additionalProperties;
            }
            set
            {
                additionalProperties.Clear();
                foreach (var item in value)
                    additionalProperties[item.Key] = item.Value;
            }
        }

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
			result.ContentType = ContentType;
			result.CorrelationId = CorrelationId;
			result.ReplyTo = ReplyTo;
			result.RoutingKey = RoutingKey;
            return result;
        }
    }
}