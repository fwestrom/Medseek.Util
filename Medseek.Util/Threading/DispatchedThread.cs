//#define USE_DISPATCHER
namespace Medseek.Util.Threading
{
    using System;
#if !USE_DISPATCHER
    using System.Collections.Generic;
#endif
#if USE_DISPATCHER
    using System.Diagnostics;
#endif
    using System.Reflection;
    using System.Threading;
#if USE_DISPATCHER
    using System.Windows.Threading;
#endif
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;

    /// <summary>
    /// Provides a thread that uses a dispatcher oriented model for scheduling 
    /// work items to be performed using the thread.
    /// </summary>
    [Register(typeof(IDispatchedThread), Lifestyle = Lifestyle.Transient)]
    public class DispatchedThread : IDispatchedThread
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#if !USE_DISPATCHER
        private readonly Queue<Op> queue = new Queue<Op>();
#endif
        private readonly object sync = new object();
        private readonly IThread thread;
#if USE_DISPATCHER
        private Dispatcher dispatcher;
#endif
        private bool disposed;
#if !USE_DISPATCHER
        private bool started;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchedThread" /> 
        /// class.
        /// </summary>
        /// <remarks>
        /// The dispatched thread that is initialized by this constructor is 
        /// set as a background thread, and must be started using the <see 
        /// cref="Start" /> method.
        /// </remarks>
        /// <seealso cref="IsBackground" />
        public DispatchedThread(IThreadingFactory threadingFactory)
        {
            if (threadingFactory == null)
                throw new ArgumentNullException("threadingFactory");

            thread = threadingFactory.CreateThread(RunDispatchThread);
            thread.IsBackground = true;
        }

        /// <summary>
        /// Gets the unique thread identifier.
        /// </summary>
        public int Id
        {
            get
            {
                return thread.Id;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the thread has been started and has
        /// not yet been terminated normally or being aborted.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return thread.IsAlive;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the thread is a background 
        /// thread, which has a default value of true.
        /// </summary>
        public bool IsBackground
        {
            get
            {
                return thread.IsBackground;
            }
            set
            {
                thread.IsBackground = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the thread.
        /// </summary>
        public string Name
        {
            get
            {
                return thread.Name;
            }
            set
            {
                thread.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the scheduling priority of the thread.
        /// </summary>
        public ThreadPriority Priority
        {
            get
            {
                return thread.Priority;
            }
            set
            {
                thread.Priority = value;
            }
        }

        /// <summary>
        /// Raises a <seealso cref="ThreadAbortException" /> on the thread, 
        /// which begins the process of terminating the thread.
        /// </summary>
        /// <remarks>
        /// Invoking this method usually causes the thread to terminate.
        /// </remarks>
        public void Abort()
        {
            thread.Abort();
        }

        /// <summary>
        /// Interrupts a thread that is in the <see 
        /// cref="System.Threading.ThreadState.WaitSleepJoin" /> state.
        /// </summary>
        /// <remarks>
        /// If this thread is not currently blocked in a wait, sleep, or join 
        /// state, it will be interrupted when it next begins to block.
        /// </remarks>
        /// <seealso cref="Thread.Interrupt" />
        public void Interrupt()
        {
            thread.Interrupt();
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates or the 
        /// specified time elapses, while continuing to perform standard COM 
        /// and SendMessage pumping.
        /// </summary>
        public void Join(TimeSpan waitTime = new TimeSpan())
        {
            thread.Join(waitTime != default(TimeSpan) ? waitTime : Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Starts the thread.
        /// </summary>
        public void Start()
        {
            lock (sync)
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().Name);

                Log.DebugFormat("{0}", MethodBase.GetCurrentMethod().Name);
                thread.Start();
                Monitor.Wait(sync);
#if USE_DISPATCHER
                Debug.Assert(dispatcher != null, "The dispatcher must be set by the thread.");
#endif
                started = true;
            }
        }

        /// <summary>
        /// Starts the thread.
        /// </summary>
        void IThread.Start(object ignored)
        {
            Start();
        }

        /// <summary>
        /// Disposes and stops the thread and its dispatcher.
        /// </summary>
        public void Dispose()
        {
            lock (sync)
            {
                if (!disposed)
                {
                    disposed = true;
#if USE_DISPATCHER
                    if (dispatcher != null)
                        dispatcher.InvokeShutdown();
#else
                    Monitor.Pulse(sync);
#endif
                }
            }
        }

        /// <summary>
        /// Invokes an action synchronously using the dispatched thread.
        /// </summary>
        /// <param name="action">
        /// The action to be performed on the dispatched thread.
        /// </param>
        public void Invoke(Action action)
        {
            if (!started)
                throw new InvalidOperationException("The thread must be started before operations can be dispatched.");
            
#if USE_DISPATCHER
            Debug.Assert(dispatcher != null, "CurrentDispatcher should never be null.");
            dispatcher.Invoke(action);
#else
            using (var done = new ManualResetEventSlim())
            {
                Exception exception = null;
                InvokeInternal(action, ex => exception = ex, done.Set);
                done.Wait();
                if (exception != null)
                    throw new TargetInvocationException(exception);
            }
#endif
        }

        /// <summary>
        /// Invokes an action synchronously using the dispatched thread.
        /// </summary>
        /// <param name="action">
        /// The action to be performed on the dispatched thread.
        /// </param>
        public void InvokeAsync(Action action)
        {
            if (!started)
                throw new InvalidOperationException("The thread must be started before operations can be dispatched.");

#if USE_DISPATCHER
            Debug.Assert(dispatcher != null, "CurrentDispatcher should never be null.");
            dispatcher.InvokeAsync(action);
#else
            InvokeInternal(action, OnDispatcherUnhandledException);
#endif
        }

#if !USE_DISPATCHER
        private void InvokeInternal(Action action, Action<Exception> onException, Action onCompleted = null)
        {
            var op = new Op(action, onException, onCompleted);
            lock (sync)
            {
                queue.Enqueue(op);
                if (queue.Count == 1)
                    Monitor.Pulse(sync);
            }
        }
#endif

        // ReSharper disable UnusedParameter.Local
        private void OnDispatcherShutdownFinished(object sender, EventArgs e)
        {
            // ReSharper restore UnusedParameter.Local
            Log.DebugFormat("Dispatcher shutdown finished; Thread = {0}:{1}.", thread.Id, thread.Name);
        }

        // ReSharper disable UnusedParameter.Local
        private void OnDispatcherShutdownStarted(object sender, EventArgs e)
        {
            // ReSharper restore UnusedParameter.Local
            Log.DebugFormat("Dispatcher shutdown started; Thread = {0}:{1}.", thread.Id, thread.Name);
        }

#if USE_DISPATCHER
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (!e.Handled)
                OnDispatcherUnhandledException(e.Exception);
        }
#endif

        private void OnDispatcherUnhandledException(Exception ex)
        {
            var message = string.Format("Unhandled exception from dispatcher; Thread = {0}:{1}, Cause = {2}: {3}.", thread.Id, thread.Name, ex.GetType().Name, ex.Message.TrimEnd('.'));
            Log.Warn(message, ex);
        }

        private void RunDispatchThread()
        {
            Log.DebugFormat("Started dispatcher thread; Thread = {0}:{1}.", thread.Id, thread.Name);
            lock (sync)
            {
                Monitor.Pulse(sync);
#if USE_DISPATCHER
                dispatcher = Dispatcher.CurrentDispatcher;
                dispatcher.UnhandledException += OnDispatcherUnhandledException;
                dispatcher.ShutdownFinished += OnDispatcherShutdownFinished;
                dispatcher.ShutdownStarted += OnDispatcherShutdownStarted;
                Debug.Assert(dispatcher != null, "CurrentDispatcher should never be null.");
#endif
            }

#if USE_DISPATCHER
            Dispatcher.Run();
#else
            while (!disposed)
            {
                Op op;
                lock (sync)
                {
                    if (queue.Count == 0)
                    {
                        Monitor.Wait(sync);
                        continue;
                    }

                    op = queue.Dequeue();
                }
                
                op.Invoke();
            }

            OnDispatcherShutdownStarted(this, EventArgs.Empty);
            OnDispatcherShutdownFinished(this, EventArgs.Empty);
#endif
        }

#if !USE_DISPATCHER
        private class Op
        {
            private readonly Action action;
            private readonly Action onCompleted;
            private readonly Action<Exception> onException;

            internal Op(Action action, Action<Exception> onException, Action onCompleted)
            {
                this.action = action;
                this.onCompleted = onCompleted;
                this.onException = onException;
            }

            internal void Invoke()
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    onException(ex);
                }
                finally
                {
                    if (onCompleted != null)
                        onCompleted();
                }
            }
        }
#endif
    }
}