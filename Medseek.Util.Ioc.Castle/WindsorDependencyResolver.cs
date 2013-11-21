namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Dependencies;
    using global::Castle.Windsor;
    using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

    /// <summary>
    /// Provides a dependency resolver for the Castle Windsor container.
    /// </summary>
    [Register(typeof(IDependencyResolver))]
    public class WindsorDependencyResolver : IDependencyResolver
    {
        private readonly IWindsorContainer container;
        private readonly List<object> toRelease = new List<object>();
        private readonly bool isScope;
        private bool disposed;


        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="WindsorDependencyResolver" /> class.
        /// </summary>
        public WindsorDependencyResolver(IWindsorContainer container)
            : this(container, false)
        {
        }

        private WindsorDependencyResolver(IWindsorContainer container, bool isScope)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.container = container;
            this.isScope = isScope;
        }

        /// <summary>
        /// Starts a resolution scope. 
        /// </summary>
        /// <returns>
        /// The dependency scope.
        /// </returns>
        public IDependencyScope BeginScope()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

            var resolver = new WindsorDependencyResolver(container, true);
            return resolver;
        }

        /// <summary>
        /// Disposes the dependency resolver.
        /// </summary>
        public void Dispose()
        {
            if (isScope && !disposed)
            {
                disposed = true;
                toRelease.ForEach(container.Release);
            }
        }

        /// <summary>
        /// Retrieves a service from the scope.
        /// </summary>
        /// <param name="serviceType">
        /// The service to be retrieved.
        /// </param>
        /// <returns>
        /// The retrieved service.
        /// </returns>
        public object GetService(Type serviceType)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

            object result = null;
            var hasComponent = container.Kernel.HasComponent(serviceType);
            if (hasComponent)
            {
                result = container.Resolve(serviceType);
                if (isScope)
                    toRelease.Add(result);
            }

            return result;
        }

        /// <summary>
        /// Retrieves a collection of services from the scope.
        /// </summary>
        /// <param name="serviceType">
        /// The collection of services to be retrieved.
        /// </param>
        /// <returns>
        /// The retrieved collection of services.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);

            var results = new object[0];
            var hasComponent = container.Kernel.HasComponent(serviceType);
            if (hasComponent)
            {
                results = container.ResolveAll(serviceType).Cast<object>().ToArray();
                if (isScope)
                    toRelease.AddRange(results);
            }

            return results;
        }
    }
}