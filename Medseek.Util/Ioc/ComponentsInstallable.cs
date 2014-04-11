namespace Medseek.Util.Ioc
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides shared functionality for types that describe the components in
    /// a subsystem, library, or functional grouping.
    /// </summary>
    public abstract class ComponentsInstallable : IInstallable
    {
        /// <summary>
        /// Installs the installable type into an inversion of control 
        /// container.
        /// </summary>
        /// <param name="container">
        /// The inversion of control container.
        /// </param>
        void IInstallable.Installing(IIocContainer container)
        {
            foreach (IInstallable registration in GetInstallables())
                registration.Installing(container);
        }

        /// <summary>
        /// Returns the collection of installable types associated with the 
        /// subclass assembly.
        /// </summary>
        protected virtual IEnumerable<IInstallable> GetInstallables()
        {
            var type = GetType();
            var registrations = Registrations.FromAssemblyContaining(type);
            return registrations;
        }
    }
}