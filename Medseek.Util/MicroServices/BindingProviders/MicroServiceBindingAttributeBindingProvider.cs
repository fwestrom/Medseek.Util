namespace Medseek.Util.MicroServices.BindingProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Medseek.Util.Ioc;

    /// <summary>
    /// Provides information about micro-service bindings by looking for 
    /// micro-service binding attributes on prospective types.
    /// </summary>
    /// <seealso cref="MicroServiceBindingAttribute" />
    [Register(typeof(IMicroServiceBindingProvider), OnlyNewServices = false)]
    public class MicroServiceBindingAttributeBindingProvider : IMicroServiceBindingProvider
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
        public IEnumerable<T> GetBindings<T>(Type type)
            where T : MicroServiceBinding, new()
        {
            var registerAttribute = type.GetCustomAttribute<RegisterMicroServiceAttribute>();
            return registerAttribute != null
                ? type.GetMethods()
                    .SelectMany(method => method.GetCustomAttributes<MicroServiceBindingAttribute>()
                        .Select(attribute => attribute.ToBinding<T>(method, type)))
                : Enumerable.Empty<T>();
        }
    }
}