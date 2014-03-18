namespace Medseek.Util.Messaging
{
    /// <summary>
    /// Interface for types that provide the functionality of a connection to 
    /// a message queuing system.
    /// </summary>
    public interface IMqConnection : IMqDisposable
    {
        /// <summary>
        /// Creates a new channel within the connection that can be used to 
        /// interact with the messaging system.
        /// </summary>
        /// <returns>
        /// The channel that was created.
        /// </returns>
        IMqChannel CreateChannnel();
    }
}