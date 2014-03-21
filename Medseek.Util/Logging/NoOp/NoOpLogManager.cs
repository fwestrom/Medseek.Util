namespace Medseek.Util.Logging.NoOp
{
    /// <summary>
    /// A log manager that provides no-operation loggers.
    /// </summary>
    public class NoOpLogManager : LogManagerBase
    {
        /// <summary>
        /// Gets a logger with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the logger.
        /// </param>
        /// <returns>
        /// The logger.
        /// </returns>
        public override ILog GetLogger(string name)
        {
            return new NoOpLog(name, this);
        }
    }
}