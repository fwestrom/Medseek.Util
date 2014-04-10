namespace Medseek.Util.MicroServices
{
    using Medseek.Util.Messaging;

    /// <summary>
    /// Interface for types that provide the ability to invoke a remote 
    /// micro-service.
    /// </summary>
    public interface IRemoteMicroServiceInvoker
    {
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
        void Send(MqAddress address, byte[] body, IMessageProperties properties);

        /// <summary>
        /// Invokes the bound method that provides the micro-service operation.
        /// </summary>
        /// <param name="binding">
        /// The micro-service binding description identifying the micro-service
        /// invocation to perform.
        /// </param>
        /// <param name="parameters">
        /// The values to pass as the parameters to the micro-service.
        /// </param>
        void Send(MicroServiceBinding binding, params object[] parameters);
    }
}