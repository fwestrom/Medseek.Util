namespace Medseek.Util.Messaging
{
    /// <summary>
    /// Interface for types that provide access to the message properties.
    /// </summary>
    public interface IMessageProperties
    {
        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the correlation identifier associated with the message.
        /// </summary>
        string CorrelationId
        {
            get;
        }

        /// <summary>
        /// Gets the location to which reply messages should be
        /// published.
        /// </summary>
        MqAddress ReplyTo
        {
            get;
        }

        /// <summary>
        /// Gets or sets the routing key associated with the message.
        /// </summary>
        string RoutingKey
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
        string Get(string id);

        /// <summary>
        /// Sets the value of a property by its identifier, name, or key.
        /// </summary>
        /// <param name="id">
        /// The property identifier, name, or key.
        /// </param>
        /// <param name="value">
        /// The value to set for the property.
        /// </param>
        void Set(string id, string value);
    }
}