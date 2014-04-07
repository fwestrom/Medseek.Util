namespace Medseek.Util.MicroServices
{
    /// <summary>
    /// Interface for types that provide the ability to invoke a remote 
    /// micro-service.
    /// </summary>
    public interface IRemoteMicroServiceInvoker
    {
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