namespace Medseek.Util.Logging.Log4Net
{
    using System;
    using Medseek.Util.Ioc;

    /// <summary>
    /// A logger that writes to a log4net logger.
    /// </summary>
    [Register(typeof(ILog), Lifestyle = Lifestyle.Transient)]
    public class Log4NetLog : LogBase
    {
        private readonly log4net.ILog log;
        private readonly Lazy<ILogger> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLog"/> 
        /// class.
        /// </summary>
        /// <param name="log">
        /// The log4net logger.
        /// </param>
        /// <param name="logManager">
        /// The log manager.
        /// </param>
        public Log4NetLog(log4net.ILog log, ILogManager logManager)
            : base(logManager)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            this.log = log;
            logger = new Lazy<ILogger>(() => new Log4NetCoreLogger(log.Logger));
        }

        /// <summary>
        /// Gets a value indicating whether debug messages are enabled for the 
        /// logger.
        /// </summary>
        public override bool IsDebugEnabled
        {
            get
            {
                return log.IsDebugEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether error messages are enabled for the 
        /// logger.
        /// </summary>
        public override bool IsErrorEnabled
        {
            get
            {
                return log.IsErrorEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether fatal messages are enabled for the 
        /// logger.
        /// </summary>
        public override bool IsFatalEnabled
        {
            get
            {
                return log.IsFatalEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether informational messages are enabled 
        /// for the logger.
        /// </summary>
        public override bool IsInfoEnabled
        {
            get
            {
                return log.IsInfoEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether warning messages are enabled for 
        /// the logger.
        /// </summary>
        public override bool IsWarnEnabled
        {
            get
            {
                return log.IsWarnEnabled;
            }
        }

        /// <summary>
        /// Gets the core logger.
        /// </summary>
        public override ILogger Logger
        {
            get
            {
                return logger.Value;
            }
        }

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public override string Name
        {
            get
            {
                return log.Logger.Name;
            }
        }

        /// <summary>
        /// Writes a debug message to the logger.
        /// </summary>
        /// <param name="message">
        /// The message object.
        /// </param>
        /// <param name="ex">
        /// An optional exception.
        /// </param>
        public override void Debug(object message, Exception ex = null)
        {
            log.Debug(message, ex);
        }

        /// <summary>
        /// Writes an error message to the logger.
        /// </summary>
        /// <param name="message">
        /// The message object.
        /// </param>
        /// <param name="ex">
        /// An optional exception.
        /// </param>
        public override void Error(object message, Exception ex = null)
        {
            log.Error(message, ex);
        }

        /// <summary>
        /// Writes a fatal message to the logger.
        /// </summary>
        /// <param name="message">
        /// The message object.
        /// </param>
        /// <param name="ex">
        /// An optional exception.
        /// </param>
        public override void Fatal(object message, Exception ex = null)
        {
            log.Fatal(message, ex);
        }

        /// <summary>
        /// Writes an informational message to the logger.
        /// </summary>
        /// <param name="message">
        /// The message object.
        /// </param>
        /// <param name="ex">
        /// An optional exception.
        /// </param>
        public override void Info(object message, Exception ex = null)
        {
            log.Info(message, ex);
        }

        /// <summary>
        /// Writes a warning message to the logger.
        /// </summary>
        /// <param name="message">
        /// The message object.
        /// </param>
        /// <param name="ex">
        /// An optional exception.
        /// </param>
        public override void Warn(object message, Exception ex = null)
        {
            log.Warn(message, ex);
        }
    }
}