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
            if (ex == null)
                log.Log(typeof(LogBase), new LogEventInfo(LogLevel.Debug, log.Name, message.ToString()));
            else
                log.DebugException(message.ToString(), ex);
        }

        public override void Error(object message, Exception ex = null)
        {
            if (ex == null)
                log.Log(typeof(LogBase), new LogEventInfo(LogLevel.Error, log.Name, message.ToString()));
            else
                log.ErrorException(message.ToString(), ex);
        }

        public override void Fatal(object message, Exception ex = null)
        {
            if (ex == null)
                log.Log(typeof(LogBase), new LogEventInfo(LogLevel.Fatal, log.Name, message.ToString()));
            else
                log.FatalException(message.ToString(), ex);
        }

        public override void Info(object message, Exception ex = null)
        {
            if (ex == null)
                log.Log(typeof(LogBase), new LogEventInfo(LogLevel.Info, log.Name, message.ToString()));
            else
                log.InfoException(message.ToString(), ex);
        }

        public override void Warn(object message, Exception ex = null)
        {
            if (ex == null)
                log.Log(typeof(LogBase), new LogEventInfo(LogLevel.Warn, log.Name, message.ToString()));
            else
                log.WarnException(message.ToString(), ex);
        }
    }
}
