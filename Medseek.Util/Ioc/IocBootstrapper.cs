namespace Medseek.Util.Ioc
{
    using System;
    using System.Reflection;
    using Medseek.Util.Logging;

    /// <summary>
    /// Provides a bootstrapper for setting up an inversion of control 
    /// container for an application.
    /// </summary>
    public class IocBootstrapper : IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IIocContainer container;
        private readonly IIocPlugin plugin;
        private bool installed;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="IocBootstrapper"/> class.
        /// </summary>
        public IocBootstrapper(IIocPlugin plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");

            this.plugin = plugin;
            container = plugin.NewContainer();
            container.RegisteredComponent += OnContainerRegisteredComponent;
        }

        /// <summary>
        /// Gets the inversion of control plugin associated with the 
        /// bootstrapper.
        /// </summary>
        public IIocPlugin Plugin
        {
            get
            {
                return plugin;
            }
        }

        /// <summary>
        /// Disposes the bootstrapper and its associated container.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                container.Dispose();
            }
        }

        /// <summary>
        /// Gets the inversion of control container associated with the 
        /// bootstrapper.
        /// </summary>
        /// <returns>
        /// The inversion of control container.
        /// </returns>
        public IIocContainer GetContainer()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);
            if (!installed)
                throw new InvalidOperationException("Installation of plugins must be performed first.");

            return container;
        }

        /// <summary>
        /// Installs the specified installable plugins into the container.
        /// </summary>
        /// <param name="plugins">
        /// The installable plugins.
        /// </param>
        public void Install(params IInstallable[] plugins)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);
            if (installed)
                throw new InvalidOperationException("Installation of plugins may only be performed once.");

            installed = true;
            container.Install(plugins);
        }

        private void OnContainerRegisteredComponent(object sender, RegisterComponentEventArgs e)
        {
            log.DebugFormat("{0}: Id = {1}, Detail = {2}", MethodBase.GetCurrentMethod().Name, e.Id, e.Detail);
        }
    }
}