namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Collections.Generic;
    using global::Castle.Facilities.TypedFactory;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;
    using global::Castle.Windsor.Installer;

    /// <summary>
    /// Provides a bootstrapper for setting up an instance of the Castle 
    /// Windsor container for an application.
    /// </summary>
    public static class WindsorBootstrapper
    {
        private static readonly Lazy<IWindsorContainer> Container = new Lazy<IWindsorContainer>(Init, true);
        private static readonly Dictionary<Lifestyle, Func<ComponentRegistration<object>, ComponentRegistration<object>>> LifestyleMap = new Dictionary<Lifestyle, Func<ComponentRegistration<object>, ComponentRegistration<object>>>
        {
            { Lifestyle.WebRequest, x => x.LifeStyle.PerWebRequest },
            { Lifestyle.Singleton, x => x.LifeStyle.Singleton },
            { Lifestyle.Transient, x => x.LifeStyle.Transient },
        };

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
            // Services
            var result = Component.For(registration.Services)
                .OnlyNewServices();

            // AsFactory
            if (registration.AsFactory)
                result = result.AsFactory();

            // Implementation
            if (registration.Implementation != null)
                result = result.ImplementedBy(registration.Implementation);

            // Lifestyle
            if (!LifestyleMap.ContainsKey(registration.Lifestyle))
                throw new NotSupportedException("Unsupported lifestyle " + registration.Lifestyle + ".");

            var applyLifestyle = LifestyleMap[registration.Lifestyle];
            result = applyLifestyle(result);

            // Name
            if (registration.Name != null)
                result = result.Named(registration.Name);

            return result;
        }

        private static IWindsorContainer Init()
        {
            var container = new WindsorContainer();
            container.Register(
                Component
                    .For<IWindsorContainer>()
                    .Instance(container));
            container.Install(
                Configuration.FromAppConfig());
            return container;
        }
    }
}