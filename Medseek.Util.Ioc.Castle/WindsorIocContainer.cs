namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Castle.MicroKernel;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;

    /// <summary>
    /// Provides the pluggable functionality of an inversion of control 
    /// container using Castle Windsor.
    /// </summary>
    public class WindsorIocContainer : WindsorContainer, IIocContainer
    {
        private readonly CastlePlugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorIocContainer"/> class.
        /// </summary>
        internal WindsorIocContainer(CastlePlugin plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");

            this.plugin = plugin;
            Kernel.ComponentRegistered += OnKernelComponentRegistered;
        }

        /// <summary>
        /// Raised to indicate that a component has been registered with the 
        /// container.
        /// </summary>
        public event EventHandler<RegisterComponentEventArgs> RegisteredComponent;

        /// <summary>
        /// Gets the set of components that are registered with the container.
        /// </summary>
        public IEnumerable<ComponentInfo> Components
        {
            get
            {
                return Kernel.GetAssignableHandlers(typeof(object))
                    .Select(x => new ComponentInfo(x.ComponentModel.Implementation, x.ComponentModel.Services.ToArray()));
            }
        }

        /// <summary>
        /// Gets the plugin that was used to obtain the container.
        /// </summary>
        public IIocPlugin Plugin
        {
            get
            {
                return plugin;
            }
        }

        /// <summary>
        /// Installs the specified installable types into the container.
        /// </summary>
        /// <param name="installables">
        /// The installable types to install into the container.
        /// </param>
        public IIocContainer Install(params IInstallable[] installables)
        {
            var installer = new InstallablesInstaller(installables.Distinct());
            Install(installer);
            return this;
        }

        /// <summary>
        /// Registers the components with the inversion of control container.
        /// </summary>
        /// <param name="registrations">
        /// The component descriptions to register with the container.
        /// </param>
        /// <returns>
        /// The instance of container upon which the method was invoked, to 
        /// enable chained invocations.
        /// </returns>
        public IIocContainer Register(params Registration[] registrations)
        {
            Register(
                registrations
                    .Select(WindsorBootstrapper.ToRegistration)
                    .Cast<IRegistration>()
                    .ToArray());
            return this;
        }

        private void OnKernelComponentRegistered(string key, IHandler handler)
        {
            var registeredComponent = RegisteredComponent;
            if (registeredComponent != null)
                registeredComponent(this, new RegisterComponentEventArgs(key, handler));
        }
    }
}