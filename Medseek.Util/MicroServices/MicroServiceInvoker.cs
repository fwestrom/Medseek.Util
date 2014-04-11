namespace Medseek.Util.MicroServices
{
    using System;
    using Medseek.Util.Objects;

    /// <summary>
    /// Provides management of an instance of a micro-service implementation 
    /// component.
    /// </summary>
    public class MicroServiceInvoker : Disposable, IMicroServiceInvoker
    {
        private readonly MicroServiceBinding binding;
        private readonly object instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceInvoker" 
        /// /> class.
        /// </summary>
        public MicroServiceInvoker(MicroServiceBinding binding, object instance)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");
            if (instance == null)
                throw new ArgumentNullException("instance");

            this.binding = binding;
            this.instance = instance;
        }

        /// <summary>
        /// Gets the micro-service binding description associated with the 
        /// invoker.
        /// </summary>
        public MicroServiceBinding Binding
        {
            get
            {
                return binding;
            }
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
        public object Invoke(object[] parameters)
        {
            ThrowIfDisposed();

            var method = binding.Method;
            var result = method.Invoke(instance, parameters);
            return result;
        }
    }
}