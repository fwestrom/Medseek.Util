namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Dependencies;
    using global::Castle.MicroKernel.Lifestyle;
    using global::Castle.Windsor;
    using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

    /// <summary>
    /// Provides a dependency resolver for the Castle Windsor container.
    /// </summary>
    [Register(typeof(IDependencyResolver))]
    public class WindsorDependencyResolver : IDependencyResolver
    {
        private readonly IWindsorContainer container;
        private readonly IDisposable scope;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="WindsorDependencyResolver" /> class.
        /// </summary>
        public WindsorDependencyResolver(IWindsorContainer container)
            : this(container, false)
        {
        }

        private WindsorDependencyResolver(IWindsorContainer container, bool beginScope)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.container = container;
            scope = beginScope ? container.BeginScope() : null;
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
            if (!disposed)
            {
                disposed = true;
                if (scope != null)
                    scope.Dispose();
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

            var hasComponent = container.Kernel.HasComponent(serviceType);
            var result = hasComponent 
                ? container.Resolve(serviceType) 
                : null;
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
            
            var hasComponent = container.Kernel.HasComponent(serviceType);
            var result = hasComponent 
                ? container.ResolveAll(serviceType).Cast<object>() 
                : Enumerable.Empty<object>();
            return result;
        }
    }
}