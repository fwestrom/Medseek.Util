using System;
using System.Runtime.Serialization;
using RabbitMQ.Client;

namespace Medseek.Util.Messaging.RabbitMq
{
    /// <summary>
    /// Thrown when the RabbitMQ Connection was forced closed by the broker.
    /// </summary>
    [Serializable]
    public class ConnectionForcedClosedException : Exception
    {
        public ConnectionForcedClosedException(ShutdownEventArgs reason) : base(reason.ToString()) { }

        public ConnectionForcedClosedException() { }

        public ConnectionForcedClosedException(string message) : base(message) { }

        public ConnectionForcedClosedException(string message, Exception inner) : base(message, inner) { }

        protected ConnectionForcedClosedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
