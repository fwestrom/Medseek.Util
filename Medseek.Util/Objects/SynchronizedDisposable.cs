namespace Medseek.Util.Objects
{
    using System;
    using System.Threading;

    /// <summary>
    /// Provides a disposable object with thread synchronization features.
    /// </summary>
    public class SynchronizedDisposable : Disposable
    {
        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();

        /// <summary>
        /// Returns an object that blocks object disposal until it has been 
        /// disposed.
        /// </summary>
        /// <param name="throwIfDisposed">
        /// A value indicating whether an exception should be thrown if the 
        /// object has already been disposed.
        /// </param>
        /// <param name="objectName">
        /// An optional object name to use if an exception is thrown because 
        /// the object has already been disposed.
        /// </param>
        /// <returns>
        /// An object that must be disposed to release the lock.
        /// </returns>
        public IDisposable EnterDisposeLock(bool throwIfDisposed = true, string objectName = null)
        {
            sync.EnterReadLock();
            try
            {
                if (throwIfDisposed)
                    ThrowIfDisposed(objectName);

                var unlocker = new Disposable(true);
                unlocker.Disposing += (sender, e) => sync.ExitReadLock();
                return unlocker;
            }
            catch
            {
                sync.ExitReadLock();
                throw;
            }
        }

        protected override void Dispose(bool suppressFinalize)
        {
            bool doDispose;
            sync.EnterUpgradeableReadLock();
            try
            {
                doDispose = !IsDisposed;
                if (doDispose)
                {
                    sync.EnterWriteLock();
                    try
                    {
                        IsDisposed = true;
                    }
                    finally
                    {
                        sync.ExitWriteLock();
                    }
                }
            }
            finally
            {
                sync.ExitUpgradeableReadLock();
            }

            if (doDispose)
                DoDispose(suppressFinalize);
        }
    }
}