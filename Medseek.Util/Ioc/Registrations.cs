namespace Medseek.Util.Ioc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides functionality for identifying component registrations.
    /// </summary>
    public static class Registrations
    {
        /// <summary>
        /// Gets the component registrations for the components in the 
        /// executing assembly.
        /// </summary>
        /// <returns>
        /// The component registration descriptions.
        /// </returns>
        public static IEnumerable<Registration> FromAssemblyContaining(Type type)
        {
            var results = Assembly.GetAssembly(type).DefinedTypes
                .SelectMany(t => t.GetCustomAttributes<RegisterAttributeBase>()
                    .Select(a => a.ToRegistration(t.AsType())));

            foreach (var result in results)
            {
                Debug.WriteLine("Registration: Services = {0}, Implementation = {1}", string.Join(",", result.Services), result.Implementation);
                yield return result;
            }
        }
    }
}