namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using global::Castle.Windsor;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using Medseek.Util.Messaging;
    using Medseek.Util.MicroServices;

    /// <summary>
    /// Provides a locator for finding and exposing micro-service 
    /// implementation components.
    /// </summary>
    [Register(typeof(IMicroServiceLocator))]
    public class MicroServiceLocator : IMicroServiceLocator
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<MicroServiceDescriptor> descriptors = new List<MicroServiceDescriptor>();
        private readonly Dictionary<object, MicroServiceDescriptor> instanceMap = new Dictionary<object, MicroServiceDescriptor>();
        private readonly IMqConnection connection;
        private readonly IWindsorContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceLocator" 
        /// /> class.
        /// </summary>
        public MicroServiceLocator(IMqConnection connection, IWindsorContainer container)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (container == null)
                throw new ArgumentNullException("container");

            this.connection = connection;
            this.container = container;
        }

        /// <summary>
        /// Gets the collection of micro-service descriptors.
        /// </summary>
        public IEnumerable<MicroServiceDescriptor> Descriptors
        {
            get
            {
                return descriptors;
            }
        }

        /// <summary>
        /// Initializes the micro-service locator so that it can fulfill requests.
        /// </summary>
        public void Initialize()
        {
            Log.Debug(MethodBase.GetCurrentMethod().Name);

            descriptors.Clear();
            
            var toAdd = container.Kernel.GetAssignableHandlers(typeof(object))
                .SelectMany(handler => handler.ComponentModel.Implementation.CustomAttributes
                    .Select(ad => new { handler, type = handler.ComponentModel.Implementation, ad.AttributeType }))
                .Where(x => x.AttributeType == typeof(RegisterMicroServiceAttribute))
                .SelectMany(x => x.type.GetCustomAttributes<RegisterMicroServiceAttribute>()
                    .Select(attribute => new { x.type, attribute, x.handler }))
                .SelectMany(x => x.attribute.MicroServiceContracts
                    .Select(contract => new { contract, x.handler, x.type, factory = container.Resolve(typeof(IMicroServiceInstanceFactory<>).MakeGenericType(contract)) }))
                .Select(x => new MicroServiceDescriptor(x.contract, x.type, x.factory));

            foreach (var descriptor in toAdd)
            {
                Log.DebugFormat("MicroServiceDescriptor: Contract = {0}, Implementation = {1}, RequestQueue = {2}", descriptor.Contract, descriptor.Implementation, descriptor.RequestQueue);
                descriptors.Add(descriptor);
            }
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
            var descriptor = Descriptors.Single(x => x.Contract == contract);
            var instance = descriptor.Get(connection);
            lock (instanceMap)
                instanceMap.Add(instance, descriptor);
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
            MicroServiceDescriptor descriptor;
            lock (instanceMap)
            {
                descriptor = instanceMap[instance];
                instanceMap.Remove(instance);
            }

            descriptor.Release(instance);
        }
    }
}