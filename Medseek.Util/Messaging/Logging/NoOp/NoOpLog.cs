namespace Medseek.Util.Logging.NoOp
{
    using System;

    /// <summary>
    /// Provides a logger that does nothing when invoked.
    /// </summary>
    public class NoOpLog : LogBase, ILogger
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="NoOpLog" /> class.
        /// </summary>
        /// <param name="name">
        /// The logger name.
        /// </param>
        /// <param name="logManager">
        /// The log manager.
        /// </param>
        public NoOpLog(string name, ILogManager logManager)
            : base(logManager)
        {
            if (name == null) 
                throw new ArgumentNullException("name");

            this.name = name;
        }

        /// <summary>
        /// Gets a value indicating whether debug messages are enabled for the 
        /// logger.
        /// </summary>
        public override bool IsDebugEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether error messages are enabled for the 
        /// logger.
        /// </summary>
        public override bool IsErrorEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether fatal messages are enabled for the 
        /// logger.
        /// </summary>
        public override bool IsFatalEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether informational messages are enabled 
        /// for the logger.
        /// </summary>
        public override bool IsInfoEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether warning messages are enabled for 
        /// the logger.
        /// </summary>
        public override bool IsWarnEnabled
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the core logger.
        /// </summary>
        public override ILogger Logger
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public override string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// This method is not used.
        /// </summary>
        public override void Debug(object message, Exception ex = null)
        {
        }

        /// <summary>
        /// This method is not used.
        /// </summary>
        public override void Error(object message, Exception ex = null)
        {
        }

        /// <summary>
        /// This method is not used.
        /// </summary>
        public override void Fatal(object message, Exception ex = null)
        {
        }

        /// <summary>
        /// This method is not used.
        /// </summary>
        public override void Info(object message, Exception ex = null)
        {
        }

        /// <summary>
        /// This method is not used.
        /// </summary>
        public override void Warn(object message, Exception ex = null)
        {
        }
    }
}