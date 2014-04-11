namespace Medseek.Util.MicroServices
{
    using System;

    /// <summary>
    /// Interface for types that can invoke bound methods on components that 
    /// provide micro-service implementations.
    /// </summary>
    public interface IMicroServiceInvoker : IDisposable
    {
        /// <summary>
        /// Gets the micro-service binding description associated with the 
        /// invoker.
        /// </summary>
        MicroServiceBinding Binding
        {
            get;
        }

        /// <summary>
        /// Invokes the bound method that provides the micro-service operation.
        /// </summary>
        /// <param name="parameters">
        /// The values to pass as the method parameters.
        /// </param>
        /// <returns>
        /// The return value produced by the method, or null if the method 
        /// has a void return type.
        /// </returns>
        object Invoke(object[] parameters);
    }
}