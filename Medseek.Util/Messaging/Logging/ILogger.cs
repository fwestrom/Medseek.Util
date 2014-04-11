namespace Medseek.Util.Logging
{
    /// <summary>
    /// Interface for logger internals.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        string Name
        {
            get;
        }
    }
}