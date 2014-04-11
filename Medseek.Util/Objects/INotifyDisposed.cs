namespace Medseek.Util.Objects
{
    using System;

    /// <summary>
    /// Interface for types that provide a notification when the object has 
    /// been disposed.
    /// </summary>
    public interface INotifyDisposed
    {
        /// <summary>
        /// Raised to indicate that the object has been disposed.
        /// </summary>
        /// <remarks>
        /// This event should only raised once, even if <see 
        /// cref="IDisposable.Dispose" /> is invoked multiple times.
        /// </remarks>
        event EventHandler Disposed;
    }
}