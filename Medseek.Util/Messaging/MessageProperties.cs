namespace Medseek.Util.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Describes additional properties associated with a message.
    /// </summary>
    [DataContract(Namespace = "")]
    public class MessageProperties : ICloneable
    {
        private Dictionary<string, object> additionalProperties;

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        [DataMember(Name = "content-type", Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the correlation identifier associated with the message.
        /// </summary>
        [DataMember(Name = "correlation-id", Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string CorrelationId
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the location to which reply messages should be
        /// published.
        /// </summary>
        /// <seealso cref="ReplyToString" />
        public MqAddress ReplyTo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the string value of the reply-to address.
        /// </summary>
        /// <seealso cref="ReplyTo" />
        [DataMember(Name = "reply-to", Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string ReplyToString
        {
            get
            {
                var replyTo = ReplyTo;
                return replyTo != null 
                    ? replyTo.Value 
                    : null;
            }
            set
            {
                ReplyTo = value != null
                    ? new MqAddress(value) 
                    : null;
            }
        }

        /// <summary>
        /// Gets or sets the additional properties dictionary.
        /// </summary>
        [DataMember(Name = "extended", Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public Dictionary<string, object> AdditionalProperties 
        {
            get
            {
                return additionalProperties != null && additionalProperties.Count > 0
                    ? additionalProperties
                    : null;
            }
            set
            {
                additionalProperties = value;
            }
        }

        /// <summary>
        /// Gets the collection of the extended property keys for which a value
        /// is set in the extended properties.
        /// </summary>
        public IEnumerable<string> ExtendedKeys
        {
            get
            {
                return additionalProperties != null
                    ? additionalProperties.Where(x => x.Value != null).Select(x => x.Key)
                    : Enumerable.Empty<string>();
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
        /// Creates a new copy of message properties instance.
        /// </summary>
        /// <returns>
        /// A new message properties object copied from the original.
        /// </returns>
        public MessageProperties Clone()
        {
            var result = new MessageProperties
            {
                ContentType = ContentType,
                CorrelationId = CorrelationId,
                ReplyTo = ReplyTo,
            };

            result.additionalProperties = additionalProperties != null
                ? new Dictionary<string, object>(additionalProperties)
                : null;

            return result;
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
            return additionalProperties != null && additionalProperties.TryGetValue(id, out value) 
                ? value 
                : null;
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
            if (value != null)
            {
                if (additionalProperties == null)
                    additionalProperties = new Dictionary<string, object>();
                additionalProperties[id] = value;
            }
            else if (additionalProperties != null)
            {
                additionalProperties.Remove(id);
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}