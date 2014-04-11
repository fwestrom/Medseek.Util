namespace Medseek.Util.Logging.Log4Net
{
    using System.Diagnostics;
    using log4net;

    public class Log4NetTraceListener : TraceListener
    {
        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }

        public override void Write(object o)
        {
            Write(o, null);
        }

        public override void Write(object o, string category)
        {
            if (string.IsNullOrEmpty(category))
                category = "Trace";

            LogManager.GetLogger(category).Info(o);
        }

        public override void Write(string message)
        {
            Write(message as object);
        }

        public override void Write(string message, string category)
        {
            Write(message as object, category);
        }

        public override void WriteLine(object o)
        {
            Write(o);
        }

        public override void WriteLine(object o, string category)
        {
            Write(o, category);
        }

        public override void WriteLine(string message)
        {
            Write(message as object);
        }

        public override void WriteLine(string message, string category)
        {
            Write(message as object, category);
        }
    }
}