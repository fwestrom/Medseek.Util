namespace Medseek.Util.Ioc
{
    using System;

    /// <summary>
    /// Describes a component that is registered with the container.
    /// </summary>
    public class ComponentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentInfo"/> class.
        /// </summary>
        public ComponentInfo(Type implementation, Type[] services)
        {
            if (implementation == null)
                throw new ArgumentNullException("implementation");
            if (services == null)
                throw new ArgumentNullException("services");

            Implementation = implementation;
            Services = services;
        }

        /// <summary>
        /// Gets the implementation type of the component.
        /// </summary>
        public Type Implementation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the set of services that are provided by the component.
        /// </summary>
        public Type[] Services
        {
            get;
            private set;
        }
    }
}