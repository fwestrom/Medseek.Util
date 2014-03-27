using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medseek.Util.Logging.NLog
{
    using System.Reflection;

    using Medseek.Util.Ioc;

    public class NLogComponents : ComponentsInstallable, ILoggingPlugin
    {
        private static readonly NLogComponents MyPlugin = new NLogComponents();
        private readonly ILogManager logManager = new NLogLogManager();

        /// <summary>
        /// Prevents a default instance of the <see cref="NLogComponents"/> class from being created.
        /// </summary>
        private NLogComponents()
        {
            var log = logManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Initialized log4net.");
            LogManager.Default = logManager;
        }

        /// <summary>
        /// Gets the plugin that provides the logging functionality using 
        /// log4net.
        /// </summary>
        public static ILoggingPlugin Plugin
        {
            get
            {
                return MyPlugin;
            }
        }

        /// <summary>
        /// Obtains an instance of the log manager for the logging plugin.
        /// </summary>
        /// <returns>
        /// The log manager provided by the logging plugin.
        /// </returns>
        ILogManager ILoggingPlugin.GetLogManager()
        {
            return logManager;
        }
    }
}
