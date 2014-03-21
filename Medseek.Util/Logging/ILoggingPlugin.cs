namespace Medseek.Util.Logging
{
    using Medseek.Util.Ioc;

    /// <summary>
    /// Interface for types that provide the pluggable functionality for 
    /// logging.
    /// </summary>
    public interface ILoggingPlugin : IInstallable
    {
        /// <summary>
        /// Obtains an instance of the log manager for the logging plugin.
        /// </summary>
        /// <returns>
        /// The log manager provided by the logging plugin.
        /// </returns>
        ILogManager GetLogManager();
    }
}