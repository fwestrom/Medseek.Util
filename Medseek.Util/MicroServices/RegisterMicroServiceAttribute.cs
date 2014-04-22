namespace Medseek.Util.MicroServices
{
    using System;
    using System.Linq;
    using Medseek.Util.Ioc;

    /// <summary>
    /// Marks a type to be registered as a Micro-Service that can process 
    /// incoming requests for a specified set of contracts.
    /// </summary>
    /// <remarks>
    /// Note that the default component lifestyle for types registered by 
    /// the marking with this <see cref="RegisterMicroServiceAttribute" /> is 
    /// <see cref="Lifestyle.Transient" />.  You may override this default 
    /// component lifestyle by specifying the desired value on the lifestyle 
    /// property <see cref="RegisterAttributeBase.Lifestyle" />.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterMicroServiceAttribute : RegisterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="RegisterMicroServiceAttribute" /> class.
        /// </summary>
        public RegisterMicroServiceAttribute(params Type[] services)
            : base(services)
        {
            Lifestyle = Lifestyle.Transient;
        }

        public override Registration ToRegistration(Type attributedType)
        {
            var registration = base.ToRegistration(attributedType);
            if (!registration.Services.Contains(attributedType))
                registration.Services = new[] { attributedType }.Concat(registration.Services);
            return registration;
        }
    }
}