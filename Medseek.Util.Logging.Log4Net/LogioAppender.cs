using System.Diagnostics;
using System.Text.RegularExpressions;
using Medseek.Util.MicroServices.MessageHandlers;

namespace Medseek.Util.Logging.Log4Net
{
    using System;
    using System.Text;
    using log4net.Appender;
    using System.IO;
    using System.Net.Sockets;


    public class LogioAppender : AppenderSkeleton
    {
        private TcpClient tcpClient;
        private DateTime LastFailure = DateTime.Now.AddMinutes(-1);

        /// <summary>
        /// Gets or sets the Log io Port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets Log io Host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets this host name.
        /// </summary>
        public string ThisHost 
        { 
            get 
            {
                return Environment.MachineName;
            } 
        }

        public string ShouldLog { get; set; }

        /// <summary>
        /// Writes to the logIo tool.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            if (string.IsNullOrEmpty(ShouldLog) && ShouldLog != "True")
                return;

            var message = string.Format("+log|{0}|{1}|{2}|{3}\r\n", loggingEvent.LoggerName, this.ThisHost, loggingEvent.Level.Name.ToLowerInvariant(),loggingEvent.RenderedMessage.Replace(Environment.NewLine, "/"));
            SendTheMessageToRemoteHost(FormatMessage(message));
        }

        private string FormatMessage(string message)
        {
            var messageParts = message.Split('|');

            messageParts[1] = Regex.Split(messageParts[1], ".pid-")[0];

            return String.Join("|", messageParts);
        }

        private void SendTheMessageToRemoteHost(string message)
        {
            SendMessageToRemoteHost(message);
        }
      
        private void SendMessageToRemoteHost(string message)
        {
            try
            {
                if (tcpClient == null && HaveNotTriedInAMinute())
                    tcpClient = new TcpClient(Host, Port);

                if (tcpClient == null) return;
                var data = Encoding.ASCII.GetBytes(message);

                var stream = tcpClient.GetStream();
                stream.Write(data, 0, data.Length);
            }
            catch (SocketException)
            {
                LastFailure = DateTime.Now;
            }
            catch (IOException)
            {
            }
        }

        private bool HaveNotTriedInAMinute()
        {
            return DateTime.Now.AddMinutes(-1) > LastFailure;
        }
    }
}
