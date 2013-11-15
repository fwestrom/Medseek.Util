namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Dependencies;
    using global::Castle.Windsor;

    /// <summary>
    /// Provides an integration with the Castle Windsor container for web 
    /// applications.
    /// </summary>
    public class WindsorIntegrationHttpModule : IHttpModule
    {
        private readonly Lazy<IWindsorContainer> container = new Lazy<IWindsorContainer>(WindsorBootstrapper.GetContainer);

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
            var dependencyResolver = container.Value.Resolve<IDependencyResolver>();
            GlobalConfiguration.Configuration.DependencyResolver = dependencyResolver;
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
            container.Value.Dispose();
        }
    }
}