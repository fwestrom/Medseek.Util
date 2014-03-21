namespace Medseek.Util.MicroServices.Host
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Provides the functionality for hosting micro-services defined in an
    /// external .NET assembly.
    /// </summary>
    public class MicroServiceHost : IDisposable
    {
        private readonly List<IInstallable> installables = new List<IInstallable> { UtilComponents.Framework };
        private readonly IIocContainer container;
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceHost" /> 
        /// class.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Message Queue is not hugarian notation.")]
        public MicroServiceHost(IIocPlugin ioc, ILoggingPlugin logging, IMqPlugin mqPlugin, params Assembly[] assembliesToInstall)
        {
            if (ioc == null)
                throw new ArgumentNullException("ioc");
            if (logging == null)
                throw new ArgumentNullException("logging");
            if (mqPlugin == null) throw new ArgumentNullException("mqPlugin");

            log = logging.GetLogManager().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.InfoFormat("Creating micro-service host; Plugins = {0}, {1}, {2}, AssembliesToInstall = {3}.", ioc, logging, mqPlugin, string.Join(", ", assembliesToInstall.Select(x => x.GetName().Name)));
            container = ioc.NewContainer();
            container.RegisteredComponent += OnRegisteredComponent;
            installables.Add(ioc);
            installables.Add(logging);
            installables.Add(mqPlugin);
            installables.AddRange(
                assembliesToInstall
                    .SelectMany(Registrations.FromAssembly));
        }

        /// <summary>
        /// Disposes the micro-service host.
        /// </summary>
        public void Dispose()
        {
            log.Info("Disposing micro-service host.");
            container.Dispose();
        }

        /// <summary>
        /// Used to enter the main micro-service hosting loop.
        /// </summary>
        public void Start()
        {
            log.Info("Starting micro-service host.");

            container.Install(installables);
        }

        private void OnRegisteredComponent(object sender, RegisterComponentEventArgs e)
        {
            log.DebugFormat("{0}: Key = {1}, Handler = {2}", MethodBase.GetCurrentMethod().Name, e.Id, e.Detail);
        }
    }
}