namespace Medseek.Util.Logging
{
    using System;

    /// <summary>
    /// Provides common functionality for loggers.
    /// </summary>
    public abstract class LogBase : ILog
    {
        private readonly ILogManager logManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogBase"/> class.
        /// </summary>
        /// <param name="logManager">
        /// The log manager.
        /// </param>
        protected LogBase(ILogManager logManager)
        {
            if (logManager == null)
                throw new ArgumentNullException("logManager");

            this.logManager = logManager;
        }

        /// <summary>
        /// Gets a value indicating whether debug messages are enabled for the 
        /// logger.
        /// </summary>
        public abstract bool IsDebugEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether error messages are enabled for the 
        /// logger.
        /// </summary>
        public abstract bool IsErrorEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether fatal messages are enabled for the 
        /// logger.
        /// </summary>
        public abstract bool IsFatalEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether informational messages are enabled 
        /// for the logger.
        /// </summary>
        public abstract bool IsInfoEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether warning messages are enabled for 
        /// the logger.
        /// </summary>
        public abstract bool IsWarnEnabled
        {
            get;
        }

        /// <summary>
        /// Gets the core logger.
        /// </summary>
        public abstract ILogger Logger
        {
            get;
        }

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public abstract string Name
        {
            get;
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
        public abstract void Debug(object message, Exception ex = null);

        /// <summary>
        /// Writes a debug message to the logger.
        /// </summary>
        /// <param name="format">
        /// The format string for the message
        /// </param>
        /// <param name="args">
        /// The format arguments.
        /// </param>
        public void DebugFormat(string format, params object[] args)
        {
            var message = new FormatObj(format, args);
            Debug(message);
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
        public abstract void Error(object message, Exception ex = null);

        /// <summary>
        /// Writes an error message to the logger.
        /// </summary>
        /// <param name="format">
        /// The format string for the message
        /// </param>
        /// <param name="args">
        /// The format arguments.
        /// </param>
        public void ErrorFormat(string format, params object[] args)
        {
            var message = new FormatObj(format, args);
            Error(message);
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
        public abstract void Fatal(object message, Exception ex = null);

        /// <summary>
        /// Writes a debug message to the logger.
        /// </summary>
        /// <param name="format">
        /// The format string for the message
        /// </param>
        /// <param name="args">
        /// The format arguments.
        /// </param>
        public void FatalFormat(string format, params object[] args)
        {
            var message = new FormatObj(format, args);
            Fatal(message);
        }

        /// <summary>
        /// Gets the child logger with a name corresponding to the name of the
        /// parent logger and the name of the child logger joined by a dot.
        /// </summary>
        /// <param name="name">
        /// The name of the child logger.
        /// </param>
        /// <returns>
        /// The child logger.
        /// </returns>
        public ILog GetChildLogger(string name)
        {
            var result = logManager.GetChildLogger(this, name);
            return result;
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
        public abstract void Info(object message, Exception ex = null);

        /// <summary>
        /// Writes an informational message to the logger.
        /// </summary>
        /// <param name="format">
        /// The format string for the message
        /// </param>
        /// <param name="args">
        /// The format arguments.
        /// </param>
        public void InfoFormat(string format, params object[] args)
        {
            var message = new FormatObj(format, args);
            Info(message);
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
        public abstract void Warn(object message, Exception ex = null);

        /// <summary>
        /// Writes a warning message to the logger.
        /// </summary>
        /// <param name="format">
        /// The format string for the message
        /// </param>
        /// <param name="args">
        /// The format arguments.
        /// </param>
        public void WarnFormat(string format, params object[] args)
        {
            var message = new FormatObj(format, args);
            Warn(message);
        }

        /// <summary>
        /// Helper class for delayed string formatting for log messages.
        /// </summary>
        private class FormatObj
        {
            private readonly object[] args;
            private readonly string format;

            internal FormatObj(string format, object[] args)
            {
                this.args = args;
                this.format = format;
            }

            /// <summary>
            /// Returns the formatted string.
            /// </summary>
            /// <returns>
            /// The formatted string.
            /// </returns>
            public override string ToString()
            {
                var result = string.Format(format, args);
                return result;
            }
        }
    }
}