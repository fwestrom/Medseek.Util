namespace Medseek.Util.Logging.Log4Net
{
    using System.Reflection;
    using Medseek.Util.Ioc;

    /// <summary>
    /// Describes the log4net logging plugin components.
    /// </summary>
    public class Log4NetComponents : ComponentsInstallable, ILoggingPlugin
    {
        private static readonly Log4NetComponents MyPlugin = new Log4NetComponents();
        private readonly ILogManager logManager = new Log4NetLogManager();

        /// <summary>
        /// Prevents a default instance of the <see 
        /// cref="Log4NetComponents" /> class from being created.
        /// </summary>
        private Log4NetComponents()
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