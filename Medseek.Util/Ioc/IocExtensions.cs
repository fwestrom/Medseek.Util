namespace Medseek.Util.Ioc
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for working with inversion of control types.
    /// </summary>
    public static class IocExtensions
    {
        /// <summary>
        /// Installs the specified installable types into the container.
        /// </summary>
        /// <param name="container">
        /// The inversion of control container.
        /// </param>
        /// <param name="installables">
        /// The installable types to install into the container.
        /// </param>
        /// <seealso cref="IIocContainer.Install" />
        [DebuggerNonUserCode]
        public static void Install(this IIocContainer container, IEnumerable<IInstallable> installables)
        {
            container.Install(installables.ToArray());
        }

        /// <summary>
        /// Installs the specified installable types into the container.
        /// </summary>
        /// <param name="bootstrapper">
        /// The inversion of control bootstrapper.
        /// </param>
        /// <param name="installables">
        /// The installable types to install into the container.
        /// </param>
        /// <seealso cref="IIocContainer.Install" />
        [DebuggerNonUserCode]
        public static void Install(this IocBootstrapper bootstrapper, IEnumerable<IInstallable> installables)
        {
            bootstrapper.Install(installables.ToArray());
        }
    }
}