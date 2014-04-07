namespace Medseek.Util.MicroServices
{
    /// <summary>
    /// Interface for types that provide micro-service message dispatching.
    /// </summary>
    public interface IMicroServiceDispatcher
    {
        /// <summary>
        /// Gets the remote micro-service invoker.
        /// </summary>
        IRemoteMicroServiceInvoker RemoteMicroServiceInvoker
        {
            get;
        }
    }
}