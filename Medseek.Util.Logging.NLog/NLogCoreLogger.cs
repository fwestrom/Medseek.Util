namespace Medseek.Util.Logging.NLog
{
    using System;

    using global::NLog;
    using INLogger = global::NLog.Interface.ILogger;
    
    /// <summary>
    /// A logger that writes to a log4net logger.
    /// </summary>
    public class NLogCoreLogger : ILogger
    {
        private readonly INLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogCoreLogger" /> 
        /// class.
        /// </summary>
        /// <param name="logger">
        /// The core logger.
        /// </param>
        public NLogCoreLogger(INLogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.logger = logger;
        }

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public string Name
        {
            get
            {
                return logger.Name;
            }
        }
    }
}