namespace Medseek.Util.Plugin.WebApi
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Dependencies;
    using Medseek.Util.Ioc;

    /// <summary>
    /// Provides an integration with the Castle Windsor container for web 
    /// applications.
    /// </summary>
    public class IocIntegrationHttpModule : IHttpModule
    {
        private readonly Lazy<IIocContainer> container;

        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="IocIntegrationHttpModule" /> class.
        /// </summary>
        /// <param name="getContainer">
        /// A callback delegate that will be used to obtain the container the 
        /// first time the container is used.
        /// </param>
        internal IocIntegrationHttpModule(Func<IIocContainer> getContainer)
        {
            if (getContainer == null)
                throw new ArgumentNullException("getContainer");

            container = new Lazy<IIocContainer>(getContainer);
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="app">
        /// An <see cref="HttpApplication"/> that provides access to the 
        /// methods, properties, and events common to all application objects
        /// within an ASP.NET application.
        /// </param>
        public void Init(HttpApplication app)
        {
            container.Value.Register(
                new Registration
                {
                    Services = new[] { typeof(IDependencyResolver) },
                    Implementation = typeof(IocDependencyResolver),
                },
                new Registration
                {
                    Services = new[] { typeof(IIocContainer) },
                    Instance = container.Value,
                    OnlyNewServices = true,
                });

            GlobalConfiguration.Configuration.DependencyResolver = (IDependencyResolver)container.Value.Resolve(typeof(IDependencyResolver));
        }

        /// <summary>
        /// Disposes the HTTP module.
        /// </summary>
        public void Dispose()
        {
        }
    }
}