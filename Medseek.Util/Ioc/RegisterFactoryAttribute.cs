namespace Medseek.Util.Ioc
{
    using System;

    /// <summary>
    /// Marks an interface for registration as a factory to be installed in the
    /// injection container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public class RegisterFactoryAttribute : RegisterAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="RegisterFactoryAttribute" /> class.
        /// </summary>
        public RegisterFactoryAttribute()
            : base(Type.EmptyTypes)
        {
        }

        /// <summary>
        /// Gets or sets the name of the component selector used by the 
        /// factory to identify the components to resolve.
        /// </summary>
        public string ComponentSelectorName
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
            result.AsFactory = true;
            result.ComponentSelectorName = ComponentSelectorName;
            result.Implementation = null;
            return result;
        }
    }
}