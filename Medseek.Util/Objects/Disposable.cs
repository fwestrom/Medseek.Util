namespace Medseek.Util.Objects
{
    using System;

    /// <summary>
    /// Provides a utility that raises an event when it is being disposed.
    /// </summary>
    public class Disposable : IDisposable, INotifyDisposed
    {
        private readonly bool finalizeDispose;

        /// <summary>
        /// Initializes a new instance of the <see cref="Disposable"/> class.
        /// </summary>
        /// <param name="finalizeDispose">
        /// A value indicating whether the finalizer should ensure that the 
        /// object has been disposed and dispose the object if it has not.
        /// </param>
        public Disposable(bool finalizeDispose = false)
        {
            this.finalizeDispose = finalizeDispose;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Disposable"/> class, 
        /// ensuring that the object is disposed if that option was enabled.
        /// </summary>
        ~Disposable()
        {
            if (finalizeDispose)
                Dispose(false);
        }

        /// <summary>
        /// Raised to indicate that the object has been disposed.
        /// </summary>
        /// <remarks>
        /// This event is only raised once, even if <see cref="Dispose" /> 
        /// is invoked multiple times.
        /// </remarks>
        /// <seealso cref="Dispose" />
        /// <seealso cref="Disposing" />
        public event EventHandler Disposed;

        /// <summary>
        /// Raised to indicate that the object is being disposed.
        /// </summary>
        /// <remarks>
        /// This event is only raised once, even if <see cref="Dispose" /> 
        /// is invoked multiple times.
        /// </remarks>
        /// <seealso cref="Dispose" />
        /// <seealso cref="Disposed" />
        public event EventHandler Disposing;

        /// <summary>
        /// Gets a value indicating whether the object has been disposed.
        /// </summary>
        /// <seealso cref="Dispose" />
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Disposes the object and raises the callbacks and events to allow 
        /// subclasses and subscribers to provide customized behavior.
        /// <para>
        /// When the method is invoked, the following sequence occurs to allow 
        /// customized object disposal:
        /// <list type="number">
        /// <item><description>
        /// Set <value><see cref="IsDisposed" /> = true</value>
        /// </description></item>
        /// <item><description>
        /// Call <see cref="OnDisposing" />
        /// </description></item>
        /// <item><description>
        /// Raise <see cref="Disposing" />
        /// </description></item>
        /// <item><description>
        /// Call <see cref="OnDisposed" />
        /// </description></item>
        /// <item><description>
        /// Raise <see cref="Disposed" />
        /// </description></item>
        /// </list>
        /// </para>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException" /> if the object has 
        /// already been disposed.
        /// </summary>
        /// <param name="objectName">
        /// The object name to use when creating the exception, or null to use 
        /// the default value obtained from <value>GetType().Name</value>.
        /// </param>
        public void ThrowIfDisposed(string objectName = null)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(objectName ?? GetType().Name);
        }

        /// <summary>
        /// Invoked by the base class when the object has been disposed to 
        /// allow subclasses to customize the behavior of dispose.
        /// </summary>
        /// <remarks>
        /// This method is only invoked once, even if <see cref="Dispose" /> 
        /// is invoked multiple times.
        /// </remarks>
        /// <seealso cref="Dispose" />
        protected virtual void OnDisposed()
        {
        }

        /// <summary>
        /// Invoked by the base class when the object is being disposed to 
        /// allow subclasses to customize the behavior of dispose.
        /// </summary>
        /// <remarks>
        /// This method is only invoked once, even if <see cref="Dispose" /> 
        /// is invoked multiple times.
        /// </remarks>
        /// <seealso cref="Dispose" />
        protected virtual void OnDisposing()
        {
        }

        protected virtual void Dispose(bool suppressFinalize)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                try
                {
                    try
                    {
                        OnDisposing();
                        var disposing = Disposing;
                        if (disposing != null)
                            disposing(this, EventArgs.Empty);
                    }
                    finally
                    {
                        if (suppressFinalize)
                            GC.SuppressFinalize(this);
                    }
                }
                finally
                {
                    var disposed = Disposed;
                    if (disposed != null)
                        disposed(this, EventArgs.Empty);
                }
            }
        }
    }
}