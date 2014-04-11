namespace Medseek.Util.Logging
{
    using System;

    /// <summary>
    /// Interface for types that can provide the functionality for interacting 
    /// with and controlling loggers.
    /// </summary>
    public interface ILogManager
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
        ILog GetChildLogger(ILog parent, string name);

        /// <summary>
        /// Gets a logger with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the logger.
        /// </param>
        /// <returns>
        /// The logger.
        /// </returns>
        ILog GetLogger(string name);

        /// <summary>
        /// Gets a logger named for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to use for determining the logger name.
        /// </param>
        /// <returns>
        /// The logger.
        /// </returns>
        ILog GetLogger(Type type);

        /// <summary>
        /// Gets a logger named for the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type to use for determining the logger name.
        /// </typeparam>
        /// <returns>
        /// The logger.
        /// </returns>
        ILog GetLogger<T>();
    }
}