namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for types that can find and expose micro-service 
    /// implementation components.
    /// </summary>
    public interface IMicroServiceLocator
    {
        /// <summary>
        /// Gets the collection of micro-service bindings.
        /// </summary>
        IEnumerable<MicroServiceBinding> Bindings
        {
            get;
        }

        /// <summary>
        /// Retrieves a micro-service invoker for the specified binding.
        /// </summary>
        /// <remarks>
        /// Micro-service instance descriptor objects obtained using this 
        /// method must be disposed when they are no longer needed.
        /// </remarks>
        /// <param name="binding">
        /// The micro-service binding identifying the desired invoker.
        /// </param>
        /// <param name="dependencies">
        /// Additional dependencies to provide to the micro-service.
        /// </param>
        /// <returns>
        /// A micro-service invoker, which must be disposed when it is no 
        /// longer needed.
        /// </returns>
        IMicroServiceInvoker Get(MicroServiceBinding binding, params object[] dependencies);

        /// <summary>
        /// Initializes the micro-service locator so that it can fulfill requests.
        /// </summary>
        void Initialize();
    }
}