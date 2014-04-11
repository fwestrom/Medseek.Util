namespace Medseek.Util.Messaging
{
    using System;
    using System.Collections.Generic;
    using Medseek.Util.Objects;

    /// <summary>
    /// Provides common functionality to messaging system types that need the 
    /// synchronized dispose functionality.
    /// </summary>
    public abstract class MqSynchronizedDisposable : SynchronizedDisposable, IMqDisposable
    {
        private readonly List<IMqDisposable> disposables = new List<IMqDisposable>();

        /// <summary>
        /// Invoked to notify the base class that a messaging system disposable
        /// object has been created, and should be tracked for disposal.
        /// </summary>
        protected void OnDisposableCreated(IMqDisposable disposable)
        {
            lock (disposables)
            {
                disposables.Add(disposable);
                disposable.Disposed += OnDisposableDisposed;
            }
        }

        /// <summary>
        /// Disposes the object and any messaging system disposable objects 
        /// that were previously created and are still being tracked.
        /// </summary>
        protected override void OnDisposing()
        {
            IMqDisposable[] toDispose;
            lock (disposables)
                toDispose = disposables.ToArray();
            Array.ForEach(toDispose, x => x.Dispose());

            OnDisposingMqDisposable();
        }

        /// <summary>
        /// Disposes the messaging system disposable component.
        /// </summary>
        protected abstract void OnDisposingMqDisposable();

        private void OnDisposableDisposed(object sender, EventArgs e)
        {
            lock (disposables)
            {
                var disposable = (IMqDisposable)sender;
                disposable.Disposed -= OnDisposableDisposed;
                disposables.Remove(disposable);
            }
        }
    }
}