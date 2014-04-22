namespace Medseek.Util.MicroServices.Lookup
{
    using System;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Interface for types that provide micro-service lookup functionality 
    /// for translating addressing information at runtime to the actual values 
    /// that should be used to communicate with active services.
    /// </summary>
    public interface IMicroServiceLookup
    {
        /// <summary>
        /// Resolves a micro-service using the lookup functionality and the 
        /// specified original address.
        /// </summary>
        /// <param name="address">
        /// The original address corresponding to the event, request, service, 
        /// or other relevant messaging primitive. 
        /// </param>
        /// <param name="timeout">
        /// The maximum amount of time to wait for a query to complete before 
        /// it will be aborted.
        /// </param>
        /// <returns>
        /// The resolved address, or null if no reply was returned by the 
        /// query.
        /// </returns>
        MqAddress Resolve(MqAddress address, TimeSpan timeout);
    }
}