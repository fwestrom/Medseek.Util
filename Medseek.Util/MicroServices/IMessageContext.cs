namespace Medseek.Util.MicroServices
{
    using System;
    using System.IO;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Interface for types that provide information about the current context 
    /// for executing micro-service operations.
    /// </summary>
    public interface IMessageContext
    {
        /// <summary>
        /// Raised to indicate that the message acknowledgement is desired.
        /// </summary>
        event EventHandler Acknowledged;

        /// <summary>
        /// Gets the size in bytes of the message body.
        /// </summary>
        int BodyLength
        {
            get;
        }

        /// <summary>
        /// Gets the message properties.
        /// </summary>
        MessageProperties Properties
        {
            get;
        }

        /// <summary>
        /// Gets the routing key associated with the message context.
        /// </summary>
        string RoutingKey
        {
            get;
        }

        /// <summary>
        /// Causes the message acknowledgement to be send for the context.
        /// </summary>
        void Ack();

        /// <summary>
        /// Returns a new cloned copy of the message context (see remarks 
        /// about the body data and context properties).
        /// </summary>
        /// <remarks>
        /// The routing key and body oriented data is not necessarily cloned 
        /// by this operation or even present on the resulting message context.
        /// </remarks>
        IMessageContext Clone(bool includeAllProperties = false);

        /// <summary>
        /// Gets an array containing the message body data.
        /// </summary>
        byte[] GetBodyArray();

        /// <summary>
        /// Gets a stream that can be used to read the message body data.
        /// </summary>
        Stream GetBodyStream();
    }
}