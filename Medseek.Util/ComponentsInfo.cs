namespace Medseek.Util
{
    using System.Collections.Generic;
    using Medseek.Util.Ioc;

    /// <summary>
    /// Provides shared functionality for types that describe the components in
    /// a subsystem, library, or functional grouping.
    /// </summary>
    public abstract class ComponentsInfo
    {
        /// <summary>
        /// Gets the collection of registrations associated with the subsystem,
        /// library, or grouping of components.
        /// </summary>
        public virtual IEnumerable<Registration> Components
        {
            get
            {
                var type = GetType();
                return Registrations.FromAssemblyContaining(type);
            }
        }
    }
}