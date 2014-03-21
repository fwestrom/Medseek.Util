namespace Medseek.Util.Ioc
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Describes a component registration.
    /// </summary>
    public class Registration
    {
        /// <summary>
        /// Gets or sets a value indicating whether the component is to be
        /// registered as a factory.
        /// </summary>
        public bool AsFactory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a comma separated list of the interceptor component 
        /// names for the component.
        /// </summary>
        public string Interceptors
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the implementation type of the component.
        /// </summary>
        public Type Implementation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the lifestyle for the component.
        /// </summary>
        public Lifestyle Lifestyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the service types that the component exposes to 
        /// consumer types
        /// through the injection container.
        /// </summary>
        public IEnumerable<Type> Services
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the method to use for starting the component.
        /// </summary>
        public MethodInfo StartMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether previously registered services will be registered for this component.
        /// </summary>
        public bool OnlyNewServices
        {
            get;
            set;
        }
    }
}