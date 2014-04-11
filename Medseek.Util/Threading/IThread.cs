namespace Medseek.Util.Threading
{
    using System;
    using System.Threading;

    /// <summary>
    /// Interface for types that provide the functionality of a thread.
    /// </summary>
    public interface IThread
    {
        /// <summary>
        /// Gets the unique thread identifier.
        /// </summary>
        /// <seealso cref="Thread.ManagedThreadId" />
        int Id
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the thread has been started and has
        /// not yet been terminated normally or being aborted.
        /// </summary>
        /// <seealso cref="Thread.IsAlive" />
        bool IsAlive
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the thread is a background 
        /// thread.
        /// </summary>
        /// <seealso cref="Thread.IsBackground" />
        bool IsBackground
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the name of the thread.
        /// </summary>
        /// <seealso cref="Thread.Name" />
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the scheduling priority of the thread.
        /// </summary>
        /// <seealso cref="Thread.Priority" />
        ThreadPriority Priority
        {
            get; 
            set;
        }

        /// <summary>
        /// Raises a <seealso cref="ThreadAbortException" /> on the thread, 
        /// which begins the process of terminating the thread.
        /// </summary>
        /// <remarks>
        /// Invoking this method usually causes the thread to terminate.
        /// </remarks>
        /// <seealso cref="Thread.Abort()" />
        void Abort();

        /// <summary>
        /// Interrupts a thread that is in the <see 
        /// cref="System.Threading.ThreadState.WaitSleepJoin" /> state.
        /// </summary>
        /// <remarks>
        /// If this thread is not currently blocked in a wait, sleep, or join 
        /// state, it will be interrupted when it next begins to block.
        /// </remarks>
        /// <seealso cref="Thread.Interrupt" />
        void Interrupt();

        /// <summary>
        /// Blocks the calling thread until a thread terminates or the 
        /// specified time elapses, while continuing to perform standard COM 
        /// and SendMessage pumping.
        /// </summary>
        /// <seealso cref="Thread.Join(TimeSpan)" />
        void Join(TimeSpan waitTime = default(TimeSpan));

        /// <summary>
        /// Starts the thread.
        /// </summary>
        /// <seealso cref="Thread.Start()" />
        void Start();

        /// <summary>
        /// Starts the thread.
        /// </summary>
        /// <seealso cref="System.Threading.Thread.Start(object)" />
        void Start(object parameter);
    }
}