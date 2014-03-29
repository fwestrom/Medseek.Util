namespace Medseek.Util.Messaging.ActiveMq
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Apache.NMS;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using Medseek.Util.Messaging;

    /// <summary>
    /// A message consumer for working with ActiveMQ.
    /// </summary>
    [Register(typeof(IMqConsumer), Lifestyle = Lifestyle.Transient)]
    public class ActiveMqConsumer : MqConsumerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool autoDelete;
        private readonly IMessageConsumer consumer;
        private readonly IDestination destination;
        private readonly ISession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveMqConsumer"/> 
        /// class.
        /// </summary>
        public ActiveMqConsumer(ISession session, MqAddress address, bool autoDelete)
            : base(new[] { address })
        {
            if (session == null)
                throw new ArgumentNullException("session");

            this.autoDelete = autoDelete;
            this.session = session;
            destination = address.ToDestination();
            consumer = session.CreateConsumer(destination);
            consumer.Listener += OnConsumerListener;
        }

        /// <summary>
        /// Disposes the message consumer.
        /// </summary>
        protected override void OnDisposingConsumer()
        {
            try
            {
                consumer.Dispose();
                if (autoDelete)
                    session.DeleteDestination(destination);
            }
            catch (Exception ex)
            {
                var message = string.Format("Failed while trying to delete consumer destination; Address = {0}, Destination = {1}, Cause = {2}: {3}.", string.Join(", ", Addresses.Select(x => x.ToString())), destination, ex.GetType().Name, ex.Message.TrimEnd('.'));
                Log.Error(message, ex);
            }
        }

        private void OnConsumerListener(IMessage message)
        {
            var m = (IBytesMessage)message;
            var properties = message.GetProperties();
            RaiseReceived(m.Content, 0, (int)m.BodyLength, properties);
        }
    }
}