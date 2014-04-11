namespace Medseek.Util.Threading
{
    using System;

    /// <summary>
    /// Interface for types that provide a dispatcher model thread.
    /// </summary>
    public interface IDispatchedThread : IThread, IDisposable
    {
        /// <summary>
        /// Invokes an action synchronously using the dispatched thread.
        /// </summary>
        /// <param name="action">
        /// The action to be performed on the dispatched thread.
        /// </param>
        void Invoke(Action action);

        /// <summary>
        /// Invokes an action synchronously using the dispatched thread.
        /// </summary>
        /// <param name="action">
        /// The action to be performed on the dispatched thread.
        /// </param>
        void InvokeAsync(Action action);
    }
}