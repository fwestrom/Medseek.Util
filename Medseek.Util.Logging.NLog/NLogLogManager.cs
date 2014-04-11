namespace Medseek.Util.Logging.NLog
{
    using global::NLog;
    using global::NLog.Interface;

    public class NLogLogManager : LogManagerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly LogFactory factory = Logger.Factory;

        public override ILog GetLogger(string name)
        {
            var logger = factory.GetLogger(name);
            return new NLogLog(new LoggerAdapter(logger), this);
        }
    }
}
