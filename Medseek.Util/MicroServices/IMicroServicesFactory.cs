namespace Medseek.Util.MicroServices
{
    using Medseek.Util.Ioc;
    using Medseek.Util.Messaging;
    using Medseek.Util.MicroServices.BindingProviders;

    /// <summary>
    /// Interface for types that can provide instances of the micro-services
    /// components.
    /// </summary>
    [RegisterFactory(Lifestyle = Lifestyle.Transient)]
    public interface IMicroServicesFactory
    {
        /// <summary>
        /// Returns the set of available micro-service binding providers.
        /// </summary>
        IMicroServiceBindingProvider[] GetBindingProviders();

        /// <summary>
        /// Returns a remote micro-service invoker.
        /// </summary>
        IRemoteMicroServiceInvoker GetRemoteMicroServiceInvoker(IMqChannel channel);
    }
}