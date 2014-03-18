namespace Medseek.Util.Messaging
{
    using System;
    using Medseek.Util.Objects;

    /// <summary>
    /// Interface for messaging system component types that need to be cleaned 
    /// up when they are no longer required.
    /// </summary>
    public interface IMqDisposable : IDisposable, INotifyDisposed
    {
    }
}