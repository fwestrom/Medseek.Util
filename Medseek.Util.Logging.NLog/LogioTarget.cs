namespace Medseek.Util.Logging.NLog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;

    using global::NLog;
    using global::NLog.Config;
    using global::NLog.Targets;

    [Target("LogioTarget")]
    public sealed class LogioTarget : TargetWithLayout
    {
        private readonly Dictionary<string, string> registeredStreams = new Dictionary<string, string>();
        private TcpClient tcpClient;
        private bool shouldLog = true;
        private DateTime lastTried;

        [RequiredParameter]
        public string ThisHostName { get; set; }

        [RequiredParameter]
        public string LogioServer { get; set; }

        [RequiredParameter]
        public int LogioServerPort { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = logEvent.FormattedMessage;

            if (logEvent.Exception != null)
                logMessage = string.Format("{0}. {1}", logMessage, logEvent.StackTrace).Trim();

            SendTheMessageToRemoteHost(logMessage, logEvent.LoggerName);
        }

        private void SendTheMessageToRemoteHost(string message, string stream)
        {
            if (!registeredStreams.ContainsKey(stream))
                RegisterStreamAndHost(stream, ThisHostName);
            message = message.Replace(Environment.NewLine, "/");
            SendMessageToRemoteHost("+log|" + stream + "|" + ThisHostName + "|info|" + message + "\r\n");
        }

        private void RegisterStreamAndHost(string stream, string host)
        {
            SendMessageToRemoteHost("+stream|" + stream + "|" + host + "\r\n");
            registeredStreams[stream] = DateTime.Now.ToShortDateString();
        }

        private void SendMessageToRemoteHost(string message)
        {
            try
            {
                if ((tcpClient == null && shouldLog) || (!shouldLog && HaventTriedInFiveMinutes()))
                    tcpClient = new TcpClient(LogioServer, LogioServerPort);

                if (tcpClient != null)
                {
                    shouldLog = true;
                    var data = Encoding.ASCII.GetBytes(message);

                    var stream = tcpClient.GetStream();
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (SocketException)
            {
                // couldn't connect, try again in a little while
                lastTried = DateTime.Now;
                shouldLog = false;
            }
            catch (IOException)
            {
                // lost connection, retry next time around
                tcpClient = null;
            }
        }

        private bool HaventTriedInFiveMinutes()
        {
            var result = DateTime.Now - lastTried;
            return result.Minutes >= 5;
        }
    }
}
