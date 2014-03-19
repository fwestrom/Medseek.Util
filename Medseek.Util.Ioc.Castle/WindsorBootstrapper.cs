namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Dependencies;
    using global::Castle.Facilities.Startable;
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
        /// Initializes static members of the <see cref="WindsorBootstrapper" /> class.
        /// </summary>
        static WindsorBootstrapper()
        {
            InstallFromConfiguration = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application 
        /// configuration should be used as a source for what to install into 
        /// the container.
        /// </summary>
        public static bool InstallFromConfiguration
        {
            get;
            set;
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
            // Services
            var result = Component.For(registration.Services)
                .OnlyNewServices();

            // Name
            if (registration.Name != null)
                result = result.Named(registration.Name);

            // AsFactory
            if (registration.AsFactory)
            {
                const bool fallbackToResolveByType = true;
                result = result.AsFactory(new DefaultTypedFactoryComponentSelector(fallbackToResolveByTypeIfNameNotFound: fallbackToResolveByType));
            }

            // Implementation
            if (registration.Implementation != null)
            {
                result = result.ImplementedBy(registration.Implementation);
                result = registration.Implementation
                    .GetConstructors()
                    .SelectMany(c => c.GetParameters()
                        .SelectMany(p => p.GetCustomAttributes(typeof(InjectAttribute), false)
                            .Cast<InjectAttribute>()
                            .Select(a => Dependency.OnComponent(p.Name, a.ComponentName ?? p.Name))))
                    .Aggregate(result, (r, d) => r.DependsOn(d));
                result = registration.Implementation
                    .GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(OnCreateAttribute), false)
                        .Any())
                    .Aggregate(result, (r, m) => r.OnCreate(x => m.Invoke(x, null)));
            }

            // Lifestyle
            if (!LifestyleMap.ContainsKey(registration.Lifestyle))
                throw new NotSupportedException("Unsupported lifestyle " + registration.Lifestyle + ".");

            // StartMethod
            if (registration.StartMethod != null)
                result = result.StartUsingMethod(registration.StartMethod.Name);

            var applyLifestyle = LifestyleMap[registration.Lifestyle];
            result = applyLifestyle(result);

            // Interceptors
            if (!string.IsNullOrEmpty(registration.Interceptors))
                result = result.Interceptors(registration.Interceptors.Split(','));

            return result;
        }

        private static IWindsorContainer Init()
        {
            var container = new WindsorContainer();
            container.Register(
                Component
                    .For<IWindsorContainer>()
                    .Instance(container),
                Component
                    .For<WindsorDependencyResolver, IDependencyResolver>());
            
            if (InstallFromConfiguration)
                container.Install(Configuration.FromAppConfig());

            return container;
        }
    }
}