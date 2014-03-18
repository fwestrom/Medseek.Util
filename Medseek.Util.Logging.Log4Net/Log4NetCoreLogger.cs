namespace Medseek.Util.Logging.Log4Net
{
    using System;

    /// <summary>
    /// A logger that writes to a log4net logger.
    /// </summary>
    public class Log4NetCoreLogger : ILogger
    {
        private readonly log4net.Core.ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetCoreLogger" /> 
        /// class.
        /// </summary>
        /// <param name="logger">
        /// The core logger.
        /// </param>
        public Log4NetCoreLogger(log4net.Core.ILogger logger)
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