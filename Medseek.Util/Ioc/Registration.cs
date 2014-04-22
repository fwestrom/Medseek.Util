namespace Medseek.Util.Ioc
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Medseek.Util.Ioc.ComponentRegistration;

    /// <summary>
    /// Describes a component registration.
    /// </summary>
    public class Registration : IInstallable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the component is to be
        /// registered as a factory.
        /// </summary>
        /// <seealso cref="ComponentSelectorName" />
        public bool AsFactory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the component selector used if the 
        /// registration is for a factory to configure how the factory to 
        /// identifies the components that it resolves.
        /// </summary>
        /// <seealso cref="AsFactory" />
        public string ComponentSelectorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of dependency specification objects 
        /// associated with the component registration.
        /// </summary>
        public IEnumerable<Dependency> Dependencies
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
        /// Gets or sets the component instance to use for the registration.
        /// </summary>
        public object Instance
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
        /// Gets or sets a value indicating whether the registration describes 
        /// the default component for the services that it provides.
        /// </summary>
        public bool IsDefault
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
        /// Gets or sets a value indicating whether previously registered services will be registered for this component.
        /// </summary>
        public bool OnlyNewServices
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
        /// Installs the registration into an inversion of control container.
        /// </summary>
        void IInstallable.Installing(IIocContainer container)
        {
            container.Register(this);
        }
    }
}