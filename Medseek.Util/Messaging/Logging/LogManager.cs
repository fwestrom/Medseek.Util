namespace Medseek.Util.Logging
{
    using System;
    using Medseek.Util.Logging.NoOp;

    /// <summary>
    /// Provides a static log manager for use when migrating legacy code that 
    /// uses similar functionality from logging frameworks like log4net.
    /// This static logging factory pattern is not recommended for new 
    /// development.
    /// </summary>
    public abstract class LogManager
    {
        private static ILogManager @default;

        /// <summary>
        /// Gets or sets the callback to use for obtaining loggers from the 
        /// static logging factory.
        /// </summary>
        public static ILogManager Default
        {
            get
            {
                return @default ?? (@default = new NoOpLogManager());
            }
            set
            {
                @default = value;
            }
        }

        /// <summary>
        /// Gets the child logger with a name corresponding to the name of the
        /// parent logger and the name of the child logger joined by a dot.
        /// </summary>
        /// <param name="parent">
        /// The parent logger.
        /// </param>
        /// <param name="name">
        /// The name of the child logger.
        /// </param>
        /// <returns>
        /// The child logger.
        /// </returns>
        public static ILog GetChildLogger(ILog parent, string name)
        {
            return Default.GetChildLogger(parent, name);
        }

        /// <summary>
        /// Gets a logger with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the logger.
        /// </param>
        /// <returns>
        /// The logger.
        /// </returns>
        public static ILog GetLogger(string name)
        {
            return Default.GetLogger(name);
        }

        /// <summary>
        /// Gets a logger named for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to use for determining the logger name.
        /// </param>
        /// <returns>
        /// The logger.
        /// </returns>
        public static ILog GetLogger(Type type)
        {
            return Default.GetLogger(type);
        }

        /// <summary>
        /// Gets a logger named for the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type to use for determining the logger name.
        /// </typeparam>
        /// <returns>
        /// The logger.
        /// </returns>
        public static ILog GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }
    }
}