namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Castle.Facilities.Startable;
    using global::Castle.Facilities.TypedFactory;
    using global::Castle.Facilities.WcfIntegration;
    using global::Castle.MicroKernel;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;
    using global::Castle.Windsor.Installer;

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
        public WindsorIocContainer(CastlePlugin plugin)
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
            var facilities = Kernel.GetFacilities();
            if (plugin.AddStartableFacility && !facilities.Any(f => f is StartableFacility))
                AddFacility<StartableFacility>(x => x.DeferredStart());
            if (plugin.AddTypedFactoryFacility && !facilities.Any(f => f is TypedFactoryFacility))
                AddFacility<TypedFactoryFacility>();
            if (plugin.AddWcfFacility && !facilities.Any(f => f is WcfFacility))
                AddFacility<WcfFacility>();
            if (plugin.RegisterIWindsorContainer 
                && !Kernel.HasComponent(typeof(IIocContainer))
                && !Kernel.HasComponent(typeof(IWindsorContainer)))
                Register(Component.For<IWindsorContainer, IIocContainer>().Instance(this));

            var installers = new List<IWindsorInstaller> { new InstallablesInstaller(installables.Distinct()) };
            if (plugin.InstallFromConfiguration)
                installers.Add(Configuration.FromAppConfig());

            Install(installers.ToArray());
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
                    .Select(plugin.ToRegistration)
                    .Cast<IRegistration>()
                    .ToArray());
            return this;
        }

        /// <summary>
        /// Resolves the available components by a required service type.
        /// </summary>
        /// <param name="service">
        /// The required service type provided by the components.
        /// </param>
        /// <returns>
        /// The component instances that were resolved.
        /// </returns>
        public new IEnumerable<object> ResolveAll(Type service)
        {
            return base.ResolveAll(service).Cast<object>();
        }

        private void OnKernelComponentRegistered(string key, IHandler handler)
        {
            var registeredComponent = RegisteredComponent;
            if (registeredComponent != null)
                registeredComponent(this, new RegisterComponentEventArgs(key, handler));
        }
    }
}