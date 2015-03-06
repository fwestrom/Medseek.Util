namespace Medseek.Util.Logging.Log4Net
{
    using System;
    using System.Xml;
    using Medseek.Util.Logging;
    using Medseek.Util.Ioc;

    /// <summary>
    /// Provides a log manager for interacting with log4net loggers.
    /// </summary>
    [Register(typeof(ILogManager), Lifestyle = Lifestyle.Transient)]
    public class Log4NetLogManager : LogManagerBase
    {
        /// <summary>
        /// Gets a logger with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the logger.
        /// </param>
        /// <returns>
        /// The logger.
        /// </returns>
        public override ILog GetLogger(string name)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
            var log = log4net.LogManager.GetLogger(name);
            return new Log4NetLog(log, this);
        }
    }
}