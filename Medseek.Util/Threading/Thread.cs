namespace Medseek.Util.Threading
{
    using System;
    using System.Threading;
    using Medseek.Util.Ioc;

    /// <summary>
    /// Provides thread functionality by wrapping an instance of <see 
    /// cref="System.Threading.Thread" />.
    /// </summary>
    [Register(Lifestyle = Lifestyle.Transient)]
    public class Thread : IThread
    {
        private readonly System.Threading.Thread thread;

        /// <summary>
        /// Initializes a new instance of the <see cref="Thread" /> class.
        /// </summary>
        /// <param name="thread">
        /// The <see cref="System.Threading.Thread" /> that will be wrapped by 
        /// the <see cref="Thread" /> instance initialized by the constructor.
        /// </param>
        public Thread(System.Threading.Thread thread)
        {
            if (thread == null)
                throw new ArgumentNullException("thread");

            this.thread = thread;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <param name="start">
        /// The delegate to invoke as the entry point for the thread.
        /// </param>
        /// <param name="maxStackSize">
        /// The maximum stack size, in bytes, to be used by the thread, or 0 to
        /// use the default maximum stack size.
        /// </param>
        /// <seealso cref="System.Threading.Thread(ThreadStart,int)" />
        public Thread(ThreadStart start, int maxStackSize = 0)
            : this(new System.Threading.Thread(start, maxStackSize))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <param name="start">
        /// The delegate to invoke as the entry point for the thread.
        /// </param>
        /// <param name="maxStackSize">
        /// The maximum stack size, in bytes, to be used by the thread, or 0 to
        /// use the default maximum stack size.
        /// </param>
        /// <seealso cref="System.Threading.Thread(ParameterizedThreadStart,int)" />
        public Thread(ParameterizedThreadStart start, int maxStackSize = 0)
            : this(new System.Threading.Thread(start, maxStackSize))
        {
        }

        /// <summary>
        /// Gets the unique thread identifier.
        /// </summary>
        /// <seealso cref="System.Threading.Thread.ManagedThreadId" />
        public int Id
        {
            get
            {
                return thread.ManagedThreadId;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the thread has been started and has
        /// not yet been terminated normally or being aborted.
        /// </summary>
        /// <seealso cref="System.Threading.Thread.IsAlive" />
        public bool IsAlive
        {
            get
            {
                return thread.IsAlive;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the thread is a background 
        /// thread.
        /// </summary>
        /// <seealso cref="System.Threading.Thread.IsBackground" />
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
        /// <seealso cref="System.Threading.Thread.Name" />
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
        /// <seealso cref="System.Threading.Thread.Priority" />
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
        /// <seealso cref="System.Threading.Thread.Abort()" />
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
        /// <seealso cref="System.Threading.Thread.Interrupt" />
        public void Interrupt()
        {
            thread.Interrupt();
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates or the 
        /// specified time elapses, while continuing to perform standard COM 
        /// and SendMessage pumping.
        /// </summary>
        /// <seealso cref="System.Threading.Thread.Join(TimeSpan)" />
        public void Join(TimeSpan waitTime = default(TimeSpan))
        {
            thread.Join(waitTime != default(TimeSpan) ? waitTime : Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Starts the thread.
        /// </summary>
        /// <seealso cref="System.Threading.Thread.Start()" />
        public void Start()
        {
            thread.Start();
        }
    }
}