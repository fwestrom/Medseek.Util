namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Medseek.Util.Interactive;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Provides a locator for finding and exposing micro-service 
    /// implementation components.
    /// </summary>
    [Register(typeof(IMicroServiceLocator))]
    public class MicroServiceLocator : IMicroServiceLocator
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<MyBinding> bindings = new List<MyBinding>();
        private readonly Dictionary<object, MyBinding> instanceMap = new Dictionary<object, MyBinding>();
        private readonly IMqConnection connection;
        private readonly IIocContainer container;
        private readonly IMicroServicesFactory microServicesFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceLocator" 
        /// /> class.
        /// </summary>
        public MicroServiceLocator(IMqConnection connection, IIocContainer container, IMicroServicesFactory microServicesFactory)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (container == null)
                throw new ArgumentNullException("container");
            if (microServicesFactory == null)
                throw new ArgumentNullException("microServicesFactory");

            this.connection = connection;
            this.container = container;
            this.microServicesFactory = microServicesFactory;
        }

        /// <summary>
        /// Gets the collection of micro-service bindings.
        /// </summary>
        public IEnumerable<MicroServiceBinding> Bindings
        {
            get
            {
                return bindings;
            }
        }

        /// <summary>
        /// Initializes the micro-service locator so that it can fulfill requests.
        /// </summary>
        public void Initialize()
        {
            Log.Debug(MethodBase.GetCurrentMethod().Name);

            var bindingProviders = microServicesFactory.GetBindingProviders();
            bindings.Clear();
            container.Components
                .SelectMany(ci => bindingProviders
                    .SelectMany(x => x.GetBindings<MyBinding>(ci.Implementation)))
                .Do(b => b.Address = connection.Plugin.ToConsumerAddress(b.Address))
                .Do(b => b.Factory = FactoryHelper.Create(b.Service, container.Resolve(typeof(IMicroServiceInstanceFactory<>).MakeGenericType(b.Service))))
                .ForEach(bindings.Add);
        }

        /// <summary>
        /// Retrieves a micro-service component instance for the specified 
        /// contract.
        /// </summary>
        /// <remarks>
        /// Micro-service instance descriptor objects obtained using this 
        /// method must be released using <see cref="Release"/> when they are 
        /// no longer needed.
        /// </remarks>
        /// <param name="contract">
        /// The contract type provided by the micro-service.
        /// </param>
        /// <param name="dependencies">
        /// Additional dependencies to provide to the micro-service.
        /// </param>
        /// <returns>
        /// A micro-service instance descriptor object.
        /// </returns>
        /// <seealso cref="Release" />
        public MicroServiceInstance Get(Type contract, params object[] dependencies)
        {
            var binding = bindings.First(x => x.Service == contract);
            var instance = new MicroServiceInstance(binding, binding.Factory.Get(connection));
            lock (instanceMap)
                instanceMap.Add(instance, binding);
            return instance;
        }

        /// <summary>
        /// Releases a micro-service component instance that was previously 
        /// obtained from the locator.
        /// </summary>
        /// <param name="instance">
        /// The micro-service instance descriptor object.
        /// </param>
        /// <seealso cref="Get" />
        public void Release(MicroServiceInstance instance)
        {
            MyBinding binding;
            lock (instanceMap)
            {
                binding = instanceMap[instance];
                instanceMap.Remove(instance);
            }

            binding.Factory.Release(instance.Instance);
        }

        private class MyBinding : MicroServiceBinding
        {
            internal FactoryHelper Factory
            {
                get;
                set;
            }
        }

        private abstract class FactoryHelper
        {
            internal static FactoryHelper Create(Type service, object factory)
            {
                if (service == null)
                    throw new ArgumentNullException("service");
                var helperType = typeof(FactoryHelper<>).MakeGenericType(service);
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