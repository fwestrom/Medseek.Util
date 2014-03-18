﻿namespace Medseek.Util.Logging.Log4Net
{
    /// <summary>
    /// Provides a log manager for interacting with log4net loggers.
    /// </summary>
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
            var log = log4net.LogManager.GetLogger(name);
            return new Log4NetLog(log, this);
        }
    }
}