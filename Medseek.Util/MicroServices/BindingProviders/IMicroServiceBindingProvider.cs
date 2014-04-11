namespace Medseek.Util.MicroServices.BindingProviders
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for types that can identify micro-service bindings on types.
    /// </summary>
    public interface IMicroServiceBindingProvider
    {
        /// <summary>
        /// Identifies the micro-service bindings associated with the specified
        /// type, which may or may not be expected to be bound.
        /// </summary>
        /// <typeparam name="T">
        /// The type of binding objects to return.
        /// </typeparam>
        /// <param name="type">
        /// The type to analyze for micro-service bindings.
        /// </param>
        /// <returns>
        /// The micro-service bindings that were found.
        /// </returns>
        IEnumerable<T> GetBindings<T>(Type type) where T : MicroServiceBinding, new();
    }
}