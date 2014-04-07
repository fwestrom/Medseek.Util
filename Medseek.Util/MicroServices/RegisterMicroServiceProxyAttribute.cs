namespace Medseek.Util.MicroServices
{
    using System;
    using Medseek.Util.Ioc;

    /// <summary>
    /// Marks an interface to be registered as a remote micro-service for which
    /// a proxy implementation can be used to send messages.
    /// </summary>
    /// <remarks>
    /// Note that the default component lifestyle for types registered by 
    /// the marking with this <see cref="RegisterMicroServiceProxyAttribute" />
    /// is <see cref="Lifestyle.Transient" />.  You may override this default 
    /// component lifestyle by specifying the desired value on the lifestyle 
    /// property <see cref="RegisterAttributeBase.Lifestyle" />.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface)]
    public class RegisterMicroServiceProxyAttribute : RegisterAttribute
    {
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
            var value = base.ToRegistration(attributedType);
            value.Interceptors = UtilComponents.MicroServiceProxyInterceptor;
            value.Implementation = null;
            return value;
        }
    }
}