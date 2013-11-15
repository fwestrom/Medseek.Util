namespace Medseek.Util.Ioc
{
    using System;

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
            return result;
        }
    }
}