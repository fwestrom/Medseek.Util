namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;
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
        private readonly Type[] contracts;

        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="RegisterMicroServiceAttribute" /> class.
        /// </summary>
        /// <param name="contracts">
        /// The service contracts for which the type is a micro-service 
        /// implementation.
        /// </param>
        public RegisterMicroServiceAttribute(params Type[] contracts)
            : base(Type.EmptyTypes)
        {
            if (contracts == null)
                throw new ArgumentNullException("contracts");

            this.contracts = contracts;
            Lifestyle = Lifestyle.Transient;
        }

        /// <summary>
        /// Gets the set of contracts for which the type is a micro-service 
        /// implementation.
        /// </summary>
        public IEnumerable<Type> MicroServiceContracts
        {
            get
            {
                return contracts;
            }
        }

        public override Registration ToRegistration(Type attributedType)
        {
            var registration = base.ToRegistration(attributedType);
            return registration;
        }
    }
}