namespace Medseek.Util.Messaging
{
    using System;

    /// <summary>
    /// Interface for types that can be used to receive incoming messages from 
    /// a messaging system channel.
    /// </summary>
    public interface IMqConsumer : IMqDisposable
    {
        /// <summary>
        /// Raised to indicate that a message has been read from the channel 
        /// and is ready to be processed by the application.
        /// </summary>
        event EventHandler<ReceivedEventArgs> Received;
    }
}