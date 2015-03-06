namespace Medseek.Util.Logging.Log4Net
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using log4net.Appender;
    using System.IO;
    using System.Net.Sockets;


    public class LogioAppender : AppenderSkeleton
    {
        private readonly Dictionary<string, string> registeredStreams = new Dictionary<string, string>();
        
        private TcpClient tcpClient;
        
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
        public string ThisHost { get; set; }
        
        /// <summary>
        /// Writes to the logIo tool.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            var message = string.Format("+log|{0}|{1}|{2}|{3}\r\n", loggingEvent.LoggerName, this.ThisHost, loggingEvent.Level.Name.ToLowerInvariant(),loggingEvent.RenderedMessage.Replace(Environment.NewLine, "/"));
            SendTheMessageToRemoteHost(message, loggingEvent.LoggerName);
        }
        private void SendTheMessageToRemoteHost(string message, string stream)
        {
            if (!registeredStreams.ContainsKey(stream))
                RegisterStreamAndHost(stream, this.ThisHost);
            SendMessageToRemoteHost(message);
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
                if (tcpClient == null)
                    tcpClient = new TcpClient(Host, Port);

                if (tcpClient != null)
                {
                    var data = Encoding.ASCII.GetBytes(message);

                    var stream = tcpClient.GetStream();
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (SocketException)
            {
            }
            catch (IOException)
            {
            }
        }
    }
}
