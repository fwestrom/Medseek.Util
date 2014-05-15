namespace Medseek.Util.MicroServices
{
    using System;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Interface for types that provide micro-service message dispatching.
    /// </summary>
    public interface IMicroServiceDispatcher
    {
        /// <summary>
        /// Raised to indicate that a message was returned as undeliverable.
        /// </summary>
        event EventHandler<ReturnedEventArgs> Returned;

        /// <summary>
        /// Raised to indicate that an unhandled exception was encountered by 
        /// the micro-service dispatcher.
        /// </summary>
        event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        /// <summary>
        /// Gets the remote micro-service invoker.
        /// </summary>
        /// <seealso cref="Send" />
        [Obsolete("Use IMicroServiceInvoker.Send instead of the methods on IRemoteMicroServiceInvoker.")]
        IRemoteMicroServiceInvoker RemoteMicroServiceInvoker
        {
            get;
        }

        /// <summary>
        /// Sends a message to a remote micro-service.
        /// </summary>
        /// <param name="address">
        /// The address of the micro-service to which the message should be 
        /// sent.
        /// </param>
        /// <param name="body">
        /// The message body.
        /// </param>
        /// <param name="properties">
        /// The message properties.
        /// </param>
        /// <param name="enableLookup">
        /// A value indicating whether micro-service lookup should be enabled 
        /// for the send operation.
        /// </param>
        void Send(MqAddress address, byte[] body, MessageProperties properties, bool enableLookup = true);
    }
}