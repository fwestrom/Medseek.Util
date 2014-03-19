namespace Medseek.Util.Ioc
{
    using System;
    using System.Linq;

    /// <summary>
    /// Marks a type for registration as a component to be installed in the 
    /// injection container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class RegisterAttribute : RegisterAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterAttribute" /> 
        /// class.
        /// </summary>
        public RegisterAttribute(params Type[] services)
            : base(services)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the component should be 
        /// started by the container (after its installation phase has 
        /// completed) via an invocation of <see cref="IStartable.Start" />.
        /// </summary>
        /// <remarks>
        /// A component can only be started if it sets this property and 
        /// implements the <see cref="IStartable" /> interface so the container
        /// can start the component instance.
        /// </remarks>
        /// <seealso cref="IStartable" />
        public bool Start
        {
            get;
            set;
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
        public override Registration ToRegistration(Type attributedType)
        {
            var result = base.ToRegistration(attributedType);
            result.Implementation = attributedType;
            if (Start)
            {
                var startableType = typeof(IStartable);
                if (!attributedType.GetInterfaces().Contains(startableType))
                    throw new InvalidCastException(startableType.FullName + " does not implement the required IStartable interface.");
                result.StartMethod = startableType.GetMethod("Start");
            }

            return result;
        }
    }
}