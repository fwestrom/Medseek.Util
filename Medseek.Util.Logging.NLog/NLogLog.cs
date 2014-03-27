namespace Medseek.Util.Logging.NLog
{
    using System;

    using global::NLog;
    using INLogger = global::NLog.Interface.ILogger;

    public class NLogLog : LogBase
    {
        private readonly INLogger log;
        private readonly Lazy<ILogger> logger;
        public NLogLog(INLogger log, ILogManager manager)
            : base(manager)
        {
            this.log = log;
            if (log == null)
                throw new ArgumentNullException("log");

            logger = new Lazy<ILogger>(() => new NLogCoreLogger(log));
        }

        public override bool IsDebugEnabled
        {
            get { return log.IsDebugEnabled; }
        }

        public override bool IsErrorEnabled
        {
            get { return log.IsErrorEnabled; }
        }

        public override bool IsFatalEnabled
        {
            get { return log.IsFatalEnabled; }
        }

        public override bool IsInfoEnabled
        {
            get { return log.IsInfoEnabled; }
        }

        public override bool IsWarnEnabled
        {
            get { return log.IsWarnEnabled; }
        }

        public override ILogger Logger
        {
            get { return logger.Value; }
        }

        public override string Name
        {
            get { return log.Name; }
        }

        public override void Debug(object message, Exception ex = null)
        {
            log.Debug(message.ToString(), ex);
        }

        public override void Error(object message, Exception ex = null)
        {
            log.Error(message.ToString(), ex);
        }

        public override void Fatal(object message, Exception ex = null)
        {
            log.Fatal(message.ToString(), ex);
        }

        public override void Info(object message, Exception ex = null)
        {
            log.Info(message.ToString(), ex);
        }

        public override void Warn(object message, Exception ex = null)
        {
            log.Warn(message.ToString(), ex);
        }
    }
}
