namespace Medseek.Util.Plugin.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Dependencies;
    using Medseek.Util.Ioc;

    /// <summary>
    /// Provides a dependency resolver for the Castle Windsor container.
    /// </summary>
    [Register(typeof(IDependencyResolver))]
    public class IocDependencyResolver : IDependencyResolver
    {
        private readonly IIocContainer container;
        private readonly List<object> toRelease = new List<object>();
        private readonly bool isScope;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="IocDependencyResolver" /> class.
        /// </summary>
        public IocDependencyResolver(IIocContainer container)
            : this(container, false)
        {
        }

        private IocDependencyResolver(IIocContainer container, bool isScope)
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

            var resolver = new IocDependencyResolver(container, true);
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
            var hasComponent = HasComponent(serviceType);
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
            var hasComponent = HasComponent(serviceType);
            if (hasComponent)
            {
                results = container.ResolveAll(serviceType).ToArray();
                if (isScope)
                    toRelease.AddRange(results);
            }

            return results;
        }

        private bool HasComponent(Type serviceType)
        {
            var result = container.Components.SelectMany(x => x.Services).Contains(serviceType);
            return result;
        }
    }
}