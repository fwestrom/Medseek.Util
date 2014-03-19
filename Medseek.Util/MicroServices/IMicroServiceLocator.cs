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
        /// Gets the collection of micro-service descriptors.
        /// </summary>
        IEnumerable<MicroServiceDescriptor> Descriptors
        {
            get;
        }

        /// <summary>
        /// Retrieves a micro-service component instance for the specified 
        /// contract.
        /// </summary>
        /// <remarks>
        /// Micro-service instance descriptor objects obtained using this 
        /// method must be released using <see cref="Release"/> when they are 
        /// no longer needed.
        /// </remarks>
        /// <param name="contract">
        /// The contract type provided by the micro-service.
        /// </param>
        /// <param name="dependencies">
        /// Additional dependencies to provide to the micro-service.
        /// </param>
        /// <returns>
        /// A micro-service instance descriptor object.
        /// </returns>
        /// <seealso cref="Release" />
        MicroServiceInstance Get(Type contract, params object[] dependencies);

        /// <summary>
        /// Initializes the micro-service locator so that it can fulfill requests.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Releases a micro-service component instance that was previously 
        /// obtained from the locator.
        /// </summary>
        /// <param name="instance">
        /// The micro-service instance descriptor object.
        /// </param>
        /// <seealso cref="Get" />
        void Release(MicroServiceInstance instance);
    }
}