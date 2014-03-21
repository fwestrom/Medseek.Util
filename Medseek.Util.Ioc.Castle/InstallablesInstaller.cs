namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Collections.Generic;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.MicroKernel.SubSystems.Configuration;
    using global::Castle.Windsor;

    /// <summary>
    /// Installer for integrating <see cref="IInstallable" /> types with a 
    /// Castle Windsor container.
    /// </summary>
    public class InstallablesInstaller : IWindsorInstaller
    {
        private readonly IEnumerable<IInstallable> installables;

        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="InstallablesInstaller" /> class.
        /// </summary>
        public InstallablesInstaller(IEnumerable<IInstallable> installables)
        {
            if (installables == null)
                throw new ArgumentNullException("installables");

            this.installables = installables;
        }

        /// <summary>
        /// Installs the installable types into the container.
        /// </summary>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var iocContainer = (IIocContainer)container;
            foreach (var installable in installables)
                installable.Installing(iocContainer);
        }
    }
}