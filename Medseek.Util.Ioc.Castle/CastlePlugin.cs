namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using global::Castle.Facilities.Startable;
    using global::Castle.Facilities.TypedFactory;
    using global::Castle.MicroKernel.Registration;

    /// <summary>
    /// Provides a pluggable integration with the Castle project, including 
    /// features from the Windsor inversion of control container.
    /// </summary>
    public class CastlePlugin : ComponentsInstallable, IIocPlugin
    {
        private static readonly Dictionary<Lifestyle, Func<ComponentRegistration<object>, ComponentRegistration<object>>> LifestyleMap = new Dictionary<Lifestyle, Func<ComponentRegistration<object>, ComponentRegistration<object>>>
        {
            { Lifestyle.WebRequest, x => x.LifeStyle.PerWebRequest },
            { Lifestyle.Singleton, x => x.LifeStyle.Singleton },
            { Lifestyle.Transient, x => x.LifeStyle.Transient },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="CastlePlugin"/> class.
        /// </summary>
        public CastlePlugin()
        {
            AddStartableFacility = true;
            AddTypedFactoryFacility = true;
            AddWcfFacility = false;
            InstallFromConfiguration = true;
            RegisterIWindsorContainer = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the startable facility
        /// should be added to containers created by this plugin.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Startable is correct.")]
        public bool AddStartableFacility
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the typed factory facility
        /// should be added to containers created by this plugin.
        /// </summary>
        public bool AddTypedFactoryFacility
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the WCF facility
        /// should be added to containers created by this plugin.
        /// </summary>
        public bool AddWcfFacility
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application 
        /// configuration should be used as a source for what to install into 
        /// the container.
        /// </summary>
        public bool InstallFromConfiguration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the container should be 
        /// registered with the container.
        /// </summary>
        public bool RegisterIWindsorContainer
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new instance of the inversion of control container 
        /// supported by the plugin.
        /// </summary>
        /// <returns>
        /// A new instance of the container.
        /// </returns>
        public IIocContainer NewContainer()
        {
            var container = new WindsorIocContainer(this);
            return container;
        }

        /// <summary>
        /// Converts a registration into the Castle Windsor specific type for
        /// registrations.
        /// </summary>
        /// <param name="registration">
        /// The registration to convert.
        /// </param>
        /// <returns>
        /// The Castle registration.
        /// </returns>
        public ComponentRegistration<object> ToRegistration(Registration registration)
        {
            // Services
            var result = (ComponentRegistration<object>)Component.For(registration.Services);

            if (registration.OnlyNewServices)
                result = result.OnlyNewServices();

            // Name
            if (registration.Name != null)
                result = result.Named(registration.Name);

            // AsFactory
            if (registration.AsFactory)
            {
                const bool fallbackToResolveByType = true;
                result = registration.ComponentSelectorName != null 
                    ? result.AsFactory(registration.ComponentSelectorName) 
                    : result.AsFactory(new DefaultTypedFactoryComponentSelector(fallbackToResolveByTypeIfNameNotFound: fallbackToResolveByType));
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

            // Instance
            if (registration.Instance != null)
                result = result.Instance(registration.Instance);

            // Interceptors
            if (!string.IsNullOrEmpty(registration.Interceptors))
                result = result.Interceptors(registration.Interceptors.Split(','));

            // IsDefault
            if (registration.IsDefault)
                result = result.IsDefault();

            // Lifestyle
            if (!LifestyleMap.ContainsKey(registration.Lifestyle))
                throw new NotSupportedException("Unsupported lifestyle " + registration.Lifestyle + ".");
            var applyLifestyle = LifestyleMap[registration.Lifestyle];
            result = applyLifestyle(result);

            // StartMethod
            if (registration.StartMethod != null)
                result = result.StartUsingMethod(registration.StartMethod.Name);

            return result;
        }
    }
}