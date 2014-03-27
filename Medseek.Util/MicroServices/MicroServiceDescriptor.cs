namespace Medseek.Util.MicroServices
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Describes a micro-service implementation and the resources that it uses
    /// to fulfil the associated contract.
    /// </summary>
    public class MicroServiceDescriptor
    {
        private readonly Type contract;
        private readonly Type implementation;
        private readonly FactoryHelper factory;
        private MethodInfo defaultMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceDescriptor"/> class.
        /// </summary>
        /// <param name="contract">
        /// The contract implemented by the micro-service component.
        /// </param>
        /// <param name="implementation">
        /// The micro-service component implementation type.
        /// </param>
        /// <param name="factory">
        /// The micro-service instance factory to use for obtaining instances 
        /// of the micro-service.
        /// </param>
        public MicroServiceDescriptor(Type contract, Type implementation, object factory)
        {
            if (contract == null)
                throw new ArgumentNullException("contract");
            if (implementation == null)
                throw new ArgumentNullException("implementation");
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (!contract.IsAssignableFrom(implementation))
                throw new ArgumentException("The implementation type must provide a realization of the micro-service contract.", "implementation");

            this.contract = contract;
            this.implementation = implementation;
            this.factory = FactoryHelper.Create(this, factory);
        }

        /// <summary>
        /// Gets the contract associated with the micro-service.
        /// </summary>
        public Type Contract
        {
            get
            {
                return contract;
            }
        }

        /// <summary>
        /// Gets the type of the micro-service implementation component.
        /// </summary>
        public Type Implementation
        {
            get
            {
                return implementation;
            }
        }

        /// <summary>
        /// Gets the name of the request queue associated with the 
        /// micro-service.
        /// </summary>
        public string RequestQueue
        {
            get
            {
                return Contract.FullName;
            }
        }

        /// <summary>
        /// Gets the default method for handling incoming messages for the 
        /// micro-service.
        /// </summary>
        public MethodInfo DefaultMethod
        {
            get
            {
                return defaultMethod ?? (defaultMethod = Contract.GetMethods().Single());
            }
        }

        public MicroServiceInstance Get(params object[] dependencies)
        {
            var instance = factory.Get(dependencies);
            var result = new MicroServiceInstance(this, instance);
            return result;
        }

        public void Release(MicroServiceInstance instance)
        {
            factory.Release(instance.Instance);
        }

        private abstract class FactoryHelper
        {
            internal static FactoryHelper Create(MicroServiceDescriptor descriptor, object factory)
            {
                if (descriptor == null)
                    throw new ArgumentNullException("descriptor");

                var helperType = typeof(FactoryHelper<>).MakeGenericType(descriptor.Contract);
                var constructor = helperType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single();
                var helper = (FactoryHelper)constructor.Invoke(new[] { factory });
                return helper;
            }

            internal abstract object Get(params object[] dependencies);

            internal abstract void Release(object component);
        }

        private class FactoryHelper<T> : FactoryHelper
        {
            private delegate T GetDelegate(IMqConnection connection);
            private delegate void ReleaseDelegate(T instance);
            private readonly GetDelegate get;
            private readonly ReleaseDelegate release;

            internal FactoryHelper(object factory)
            {
                if (factory == null)
                    throw new ArgumentNullException("factory");

                var getParameterTypes = typeof(GetDelegate).GetMethod("Invoke").GetParameters().Select(x => x.ParameterType).ToArray();
                var getMethod = factory.GetType().GetMethod("Resolve", getParameterTypes);
                if (getMethod == null)
                    throw new ArgumentException("Unable to find Resolve method on factory.", "factory");
                get = (GetDelegate)getMethod.CreateDelegate(typeof(GetDelegate), factory);
                if (get == null)
                    throw new ArgumentException("Unable to create delegate for invoking Resolve on factory.", "factory");

                var releaseParameterTypes = typeof(ReleaseDelegate).GetMethod("Invoke").GetParameters().Select(x => x.ParameterType).ToArray();
                var releaseMethod = factory.GetType().GetMethod("Release", releaseParameterTypes);
                if (releaseMethod == null)
                    throw new ArgumentException("Unable to find Release method on factory.", "factory");
                release = (ReleaseDelegate)releaseMethod.CreateDelegate(typeof(ReleaseDelegate), factory);
                if (release == null)
                    throw new ArgumentException("Unable to create delegate for invoking Release on factory.", "factory");
            }

            internal override object Get(params object[] dependencies)
            {
                var connection = (IMqConnection)dependencies.First(x => x is IMqConnection);
                var result = get(connection);
                return result;
            }

            internal override void Release(object component)
            {
                var componentT = (T)component;
                release(componentT);
            }
        }
    }
}