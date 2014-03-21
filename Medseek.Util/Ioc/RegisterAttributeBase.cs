namespace Medseek.Util.Ioc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Base type for attributes that identify components to be registered with
    /// the injection container.
    /// </summary>
    public abstract class RegisterAttributeBase : Attribute
    {
        private readonly Type[] services;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterAttributeBase"
        /// /> class.
        /// </summary>
        protected RegisterAttributeBase(IEnumerable<Type> services)
        {
            if (services == null)
                throw new ArgumentNullException("services");

            this.services = services.ToArray();
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
        /// Gets the service types that the component exposes to consumer types
        /// through the injection container.
        /// </summary>
        public IEnumerable<Type> Services
        {
            get
            {
                return services;
            }
        }

        /// <summary>
        /// Converts the attribute to a registration description.
        /// </summary>
        /// <param name="attributedType">
        /// The type onto which the registration attribute is applied.
        /// </param>
        /// <returns>
        /// An object describing the registration.
        /// </returns>
        public virtual Registration ToRegistration(Type attributedType)
        {
            if (attributedType == null)
                throw new ArgumentNullException("attributedType");
            
            var registration = new Registration
            {
                Name = Name,
                Services = Services != null && Services.Any() ? Services : new[] { attributedType },
                Interceptors = Interceptors,
                Implementation = attributedType,
                Lifestyle = Lifestyle,
                AsFactory = false,
                OnlyNewServices = OnlyNewServices
            };

            return registration;
        }
    }
}