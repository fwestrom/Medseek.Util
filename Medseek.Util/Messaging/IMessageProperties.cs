namespace Medseek.Util.Messaging
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for types that provide access to the message properties.
    /// </summary>
    [Obsolete("Use MessageProperties class directly instead; this interface will be removed in the near future.")]
    public interface IMessageProperties : ICloneable
    {
        /// <summary>
        /// Gets or sets the additional properties dictionary.
        /// </summary>
        Dictionary<string, object> AdditionalProperties { get; set; }

        /// <summary>
        /// Gets or sets a value in the additional properties dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        object this[string key] { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier associated with the message.
        /// </summary>
        string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the location to which reply messages should be
        /// published.
        /// </summary>
        MqAddress ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the routing key associated with the message.
        /// </summary>
        string RoutingKey { get; set; }

        /// <summary>
        /// Gets the value of a property by its identifier, name, or key.
        /// </summary>
        /// <param name="id">
        /// The property identifier, name, or key.
        /// </param>
        /// <returns>
        /// The value of the property, or null if it was not present.
        /// </returns>
        object Get(string id);

        /// <summary>
        /// Sets the value of a property by its identifier, name, or key.
        /// </summary>
        /// <param name="id">
        /// The property identifier, name, or key.
        /// </param>
        /// <param name="value">
        /// The value to set for the property.
        /// </param>
        void Set(string id, object value);
    }
}