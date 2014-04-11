namespace Medseek.Util.Logging
{
    using System;

    /// <summary>
    /// Interface for loggers.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Gets a value indicating whether debug messages are enabled for the 
        /// logger.
        /// </summary>
        bool IsDebugEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether error messages are enabled for the 
        /// logger.
        /// </summary>
        bool IsErrorEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether fatal messages are enabled for the 
        /// logger.
        /// </summary>
        bool IsFatalEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether informational messages are enabled 
        /// for the logger.
        /// </summary>
        bool IsInfoEnabled
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether warning messages are enabled for 
        /// the logger.
        /// </summary>
        bool IsWarnEnabled
        {
            get;
        }

        /// <summary>
        /// Gets the core logger.
        /// </summary>
        ILogger Logger
        {
            get;
        }

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        string Name
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
        void Debug(object message, Exception ex = null);

        /// <summary>
        /// Writes a debug message to the logger.
        /// </summary>
        /// <param name="format">
        /// The format string for the message
        /// </param>
        /// <param name="args">
        /// The format arguments.
        /// </param>
        void DebugFormat(string format, params object[] args);

        /// <summary>
        /// Writes an error message to the logger.
        /// </summary>
        /// <param name="message">
        /// The message object.
        /// </param>
        /// <param name="ex">
        /// An optional exception.
        /// </param>
        void Error(object message, Exception ex = null);

        /// <summary>
        /// Writes an error message to the logger.
        /// </summary>
        /// <param name="format">
        /// The format string for the message
        /// </param>
        /// <param name="args">
        /// The format arguments.
        /// </param>
        void ErrorFormat(string format, params object[] args);

        /// <summary>
        /// Writes a fatal message to the logger.
        /// </summary>
        /// <param name="message">
        /// The message object.
        /// </param>
        /// <param name="ex">
        /// An optional exception.
        /// </param>
        void Fatal(object message, Exception ex = null);

        /// <summary>
        /// Writes a debug message to the logger.
        /// </summary>
        /// <param name="format">
        /// The format string for the message
        /// </param>
        /// <param name="args">
        /// The format arguments.
        /// </param>
        void FatalFormat(string format, params object[] args);

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
        ILog GetChildLogger(string name);

        /// <summary>
        /// Writes an informational message to the logger.
        /// </summary>
        /// <param name="message">
        /// The message object.
        /// </param>
        /// <param name="ex">
        /// An optional exception.
        /// </param>
        void Info(object message, Exception ex = null);

        /// <summary>
        /// Writes an informational message to the logger.
        /// </summary>
        /// <param name="format">
        /// The format string for the message
        /// </param>
        /// <param name="args">
        /// The format arguments.
        /// </param>
        void InfoFormat(string format, params object[] args);

        /// <summary>
        /// Writes a warning message to the logger.
        /// </summary>
        /// <param name="message">
        /// The message object.
        /// </param>
        /// <param name="ex">
        /// An optional exception.
        /// </param>
        void Warn(object message, Exception ex = null);

        /// <summary>
        /// Writes a warning message to the logger.
        /// </summary>
        /// <param name="format">
        /// The format string for the message
        /// </param>
        /// <param name="args">
        /// The format arguments.
        /// </param>
        void WarnFormat(string format, params object[] args);
    }
}