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
        /// specified assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly to examine for component registrations.
        /// </param>
        /// <returns>
        /// The component registration descriptions.
        /// </returns>
        public static IEnumerable<Registration> FromAssembly(Assembly assembly)
        {
            var results = assembly.DefinedTypes
                .SelectMany(t => t.GetCustomAttributes<RegisterAttributeBase>()
                    .Select(a => a.ToRegistration(t.AsType())));

            foreach (var result in results)
            {
                Debug.WriteLine("Registration: Services = {0}, Implementation = {1}", string.Join(",", result.Services), result.Implementation);
                yield return result;
            }
        }

        /// <summary>
        /// Gets the component registrations for the components in the 
        /// assembly in which the specified type is defined.
        /// </summary>
        /// <param name="type">
        /// The type for which the defining assembly is to be examined for 
        /// component registrations.
        /// </param>
        /// <returns>
        /// The component registration descriptions.
        /// </returns>
        public static IEnumerable<Registration> FromAssemblyContaining(Type type)
        {
            var assembly = Assembly.GetAssembly(type);
            return FromAssembly(assembly);
        }

        /// <summary>
        /// Gets the component registrations for the components in the 
        /// executing assembly.
        /// </summary>
        /// <returns>
        /// The component registration descriptions.
        /// </returns>
        public static IEnumerable<Registration> FromExecutingAssembly()
        {
            var assembly = Assembly.GetCallingAssembly();
            return FromAssembly(assembly);
        }
    }
}