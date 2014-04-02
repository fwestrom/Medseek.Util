namespace Medseek.Util.Ioc.Castle
{
    using System;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;

    /// <summary>
    /// Provides a bootstrapper for setting up an instance of the Castle 
    /// Windsor container for an application.
    /// </summary>
    public static class WindsorBootstrapper
    {
        private static readonly Lazy<WindsorIocContainer> Container = new Lazy<WindsorIocContainer>(Init, true);
        private static readonly CastlePlugin Plugin = CastleComponents.Plugin;

        /// <summary>
        /// Initializes static members of the <see cref="WindsorBootstrapper" /> class.
        /// </summary>
        static WindsorBootstrapper()
        {
            Plugin.AddStartableFacility = false;
            Plugin.AddTypedFactoryFacility = false;
            Plugin.AddWcfFacility = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application 
        /// configuration should be used as a source for what to install into 
        /// the container.
        /// </summary>
        public static bool InstallFromConfiguration
        {
            get
            {
                return Plugin.InstallFromConfiguration;
            }
            set
            {
                Plugin.InstallFromConfiguration = value;
            }
        }

        /// <summary>
        /// Gets and initializes the container associated with the 
        /// bootstrapper.
        /// </summary>
        /// <returns>
        /// The fully initialized container.
        /// </returns>
        public static IWindsorContainer GetContainer()
        {
            return Container.Value;
        }

        /// <summary>
        /// Converts a registration into the Castle Windsor specific type for
        ///  registrations.
        /// </summary>
        /// <param name="registration">
        /// The registration to convert.
        /// </param>
        /// <returns>
        /// The Castle registration.
        /// </returns>
        public static ComponentRegistration<object> ToRegistration(Registration registration)
        {
            return Plugin.ToRegistration(registration);
        }

        private static WindsorIocContainer Init()
        {
            var container = new WindsorIocContainer(Plugin);
            return container;
        }
    }
}