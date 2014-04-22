namespace Medseek.Util.Messaging
{
    using Medseek.Util.Ioc;
    using Medseek.Util.MicroServices;

    /// <summary>
    /// Interface for types that provide the pluggable functionality for 
    /// integrating with a messaging middleware system.
    /// </summary>
    public interface IMqPlugin : IInstallable
    {
        /// <summary>
        /// Determines whether the properties of a message imply that the 
        /// message is a match for the patterns bound to a consumer address.
        /// </summary>
        /// <param name="messageContext">
        /// The message context.
        /// </param>
        /// <param name="address">
        /// The consumer address.
        /// </param>
        /// <returns>
        /// A value indicating whether the message is a match for the address.
        /// </returns>
        bool IsMatch(IMessageContext messageContext, MqAddress address);

        /// <summary>
        /// Converts an address into a detailed consumer oriented address 
        /// representing same original address.
        /// </summary>
        /// <param name="address">
        /// The address to convert.
        /// </param>
        /// <returns>
        /// A detailed consumer oriented address.
        /// </returns>
        MqConsumerAddress ToConsumerAddress(MqAddress address);

        /// <summary>
        /// Converts an address into a detailed publisher oriented address 
        /// representing same original address.
        /// </summary>
        /// <param name="address">
        /// The address to convert.
        /// </param>
        /// <returns>
        /// A detailed publisher oriented address.
        /// </returns>
        MqPublisherAddress ToPublisherAddress(MqAddress address);
    }
}