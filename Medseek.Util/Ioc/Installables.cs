namespace Medseek.Util.Ioc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides functionality for identifying installable types and objects.
    /// </summary>
    public static class Installables
    {
        /// <summary>
        /// Gets the component registrations for the components in the 
        /// specified assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly to examine for component registrations.
        /// </param>
        /// <param name="includeRegistrations">
        /// A value indicating whether registrations should be included in the
        /// results.
        /// </param>
        /// <returns>
        /// The component registration descriptions.
        /// </returns>
        public static IEnumerable<IInstallable> FromAssembly(Assembly assembly, bool includeRegistrations = true)
        {
            var installables = assembly.DefinedTypes
                .SelectMany(t => t.GetCustomAttributes<InstallAttribute>()
                    .Select(a => a.ToInstallable(t.AsType())));

            foreach (var installable in installables)
            {
                Debug.WriteLine("Installable: {0}", installable);
                yield return installable;
            }

            if (includeRegistrations)
            {
                var registrations = Registrations.FromAssembly(assembly);
                foreach (var registration in registrations)
                    yield return registration;
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
        /// <param name="includeRegistrations">
        /// A value indicating whether registrations should be included in the
        /// results.
        /// </param>
        /// <returns>
        /// The component registration descriptions.
        /// </returns>
        public static IEnumerable<IInstallable> FromAssemblyContaining(Type type, bool includeRegistrations = true)
        {
            var assembly = Assembly.GetAssembly(type);
            return FromAssembly(assembly, includeRegistrations);
        }

        /// <summary>
        /// Gets the component registrations for the components in the 
        /// executing assembly.
        /// </summary>
        /// <param name="includeRegistrations">
        /// A value indicating whether registrations should be included in the
        /// results.
        /// </param>
        /// <returns>
        /// The component registration descriptions.
        /// </returns>
        public static IEnumerable<IInstallable> FromExecutingAssembly(bool includeRegistrations = true)
        {
            var assembly = Assembly.GetCallingAssembly();
            return FromAssembly(assembly, includeRegistrations);
        }
    }
}