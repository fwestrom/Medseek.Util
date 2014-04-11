namespace Medseek.Util.Logging
{
    using System;

    /// <summary>
    /// Provides common functionality to log managers.
    /// </summary>
    public abstract class LogManagerBase : ILogManager
    {
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
        public virtual ILog GetChildLogger(ILog parent, string name)
        {
            var childName = string.Join(".", parent.Name, name);
            return GetLogger(childName);
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
        public abstract ILog GetLogger(string name);

        /// <summary>
        /// Gets a logger named for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to use for determining the logger name.
        /// </param>
        /// <returns>
        /// The logger.
        /// </returns>
        public virtual ILog GetLogger(Type type)
        {
            var log = GetLogger(type.FullName);
            return log;
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
        public virtual ILog GetLogger<T>()
        {
            var type = typeof(T);
            var log = GetLogger(type);
            return log;
        }
    }
}