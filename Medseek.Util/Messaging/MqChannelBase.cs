namespace Medseek.Util.Messaging
{
    using System;
    using System.IO;
    using System.Linq;
    using Medseek.Util.Logging;

    /// <summary>
    /// Provides common functionality to messaging system channel components.
    /// </summary>
    public abstract class MqChannelBase : MqSynchronizedDisposable, IMqChannel
    {
        private readonly ILog log;
        private readonly IMqPlugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqChannelBase"/> class.
        /// </summary>
        protected MqChannelBase(IMqPlugin plugin)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");

            this.plugin = plugin;
            log = LogManager.GetLogger(GetType());
            log.DebugFormat("Creating channel.");
        }

        /// <summary>
        /// Raised to indicate that a message was returned as undeliverable.
        /// </summary>
        public event EventHandler<ReturnedEventArgs> Returned;

        /// <summary>
        /// Gets a value indicating whether the channel can be paused.
        /// </summary>
        public virtual bool CanPause
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the channel is paused to 
        /// enable flow control of messages.
        /// </summary>
        public virtual bool IsPaused
        {
            get
            {
                return false;
            }
            set
            {
                if (value != IsPaused)
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the messaging system plugin associated with the channel.
        /// </summary>
        public IMqPlugin Plugin
        {
            get
            {
                return plugin;
            }
        }

        /// <summary>
        /// Creates a consumer that can be used to receive messages from the 
        /// messaging system channel.
        /// </summary>
        /// <param name="address">
        /// A description of the services and messaging primitives to which the
        /// consumer binds for incoming messages.
        /// </param>
        /// <param name="autoAckDisabled">
        /// A value indicating whether automatic message acknowledgement is 
        /// disabled.
        /// </param>
        /// <param name="autoDelete">
        /// A value indicating whether closing the consumer should cause any 
        /// applicable services and messaging primitives to be removed.
        /// </param>
        /// <returns>
        /// The message consumer component that was created.
        /// </returns>
        public IMqConsumer CreateConsumer(MqAddress address, bool autoAckDisabled, bool autoDelete)
        {
            using (EnterDisposeLock())
            {
                log.DebugFormat("Creating consumer; Address = {0}, AutoDelete = {1}.", address, autoDelete);
                var consumer = OnCreateConsumer(address, autoAckDisabled, autoDelete);
                OnDisposableCreated(consumer);
                return consumer;
            }
        }

        /// <summary>
        /// Creates a consumer that can be used to receive messages from the 
        /// messaging system channel.
        /// </summary>
        /// <param name="addresses">
        /// A set of consumer addresses describing the messaging primitives to 
        /// which the consumer binds for incoming messages, all of which must 
        /// have the same <see cref="MqConsumerAddress.SourceKey"/>.
        /// </param>
        /// <param name="autoAckDisabled">
        /// A value indicating whether automatic message acknowledgement is 
        /// disabled.
        /// </param>
        /// <param name="autoDelete">
        /// A value indicating whether closing the consumer should cause any 
        /// applicable services and messaging primitives to be removed.
        /// </param>
        /// <returns>
        /// The message consumer component that was created.
        /// </returns>
        public IMqConsumer[] CreateConsumers(MqConsumerAddress[] addresses, bool autoAckDisabled, bool autoDelete)
        {
            using (EnterDisposeLock())
            {
                log.DebugFormat("Creating consumer; Addresses = {0}, AutoDelete = {1}.", string.Join(", ", addresses.Select(x => x.ToString())), autoDelete);
                var consumers = OnCreateConsumers(addresses, autoAckDisabled, autoDelete);
                foreach (var consumer in consumers)
                    OnDisposableCreated(consumer);
                return consumers;
            }
        }

        /// <summary>
        /// Creates a publisher that can be used to send messages over to the 
        /// messaging system channel.
        /// </summary>
        /// <param name="address">
        /// A description of the services and messaging primitives that should 
        /// be used when publishing outgoing messages.
        /// </param>
        /// <returns>
        /// The message publisher component that was created.
        /// </returns>
        public IMqPublisher CreatePublisher(MqAddress address)
        {
            using (EnterDisposeLock())
            {
                log.DebugFormat("Creating publisher; Address = {0}.", address);
                var publisher = OnCreatePublisher(address);
                OnDisposableCreated(publisher);
                return publisher;
            }
        }

        /// <summary>
        /// Creates an RPC client that can be used to perform RPC style 
        /// communication using the messaging system channel.
        /// </summary>
        /// <param name="address">
        /// A description of the services and messaging primitives that should 
        /// be used when publishing outgoing messages.
        /// </param>
        /// <returns>
        /// The RPC client messaging component that was created.
        /// </returns>
        public IMqRpcClient<byte[], Stream> CreateRpcClient(MqAddress address)
        {
            using (EnterDisposeLock())
            {
                var client = new MqRpcClient(this, address);
                OnDisposableCreated(client);
                return client;
            }
        }

        /// <summary>
        /// Creates a consumer that can be used to receive messages from the 
        /// messaging system channel.
        /// </summary>
        /// <param name="address">
        /// A description of the services and messaging primitives to which the
        /// consumer binds for incoming messages.
        /// </param>
        /// <param name="autoAckDisabled">
        /// A value indicating whether automatic message acknowledgement is 
        /// disabled.
        /// </param>
        /// <param name="autoDelete">
        /// A value indicating whether closing the consumer should cause any 
        /// applicable services and messaging primitives to be removed.
        /// </param>
        /// <returns>
        /// The message consumer component that was created.
        /// </returns>
        protected abstract IMqConsumer OnCreateConsumer(MqAddress address, bool autoAckDisabled, bool autoDelete);

        /// <summary>
        /// Creates a consumer that can be used to receive messages from the 
        /// messaging system channel.
        /// </summary>
        /// <param name="addresses">
        /// A set of consumer addresses describing the messaging primitives to 
        /// which the consumer binds for incoming messages, all of which must 
        /// have the same <see cref="MqConsumerAddress.SourceKey"/>.
        /// </param>
        /// <param name="autoAckDisabled">
        /// A value indicating whether automatic message acknowledgement is 
        /// disabled.
        /// </param>
        /// <param name="autoDelete">
        /// A value indicating whether closing the consumer should cause any 
        /// applicable services and messaging primitives to be removed.
        /// </param>
        /// <returns>
        /// The message consumer components that were created.
        /// </returns>
        protected virtual IMqConsumer[] OnCreateConsumers(MqConsumerAddress[] addresses, bool autoAckDisabled, bool autoDelete)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a publisher that can be used to send messages over to the 
        /// messaging system channel.
        /// </summary>
        /// <param name="address">
        /// A description of the services and messaging primitives that should 
        /// be used when publishing outgoing messages.
        /// </param>
        /// <returns>
        /// The message publisher component that was created.
        /// </returns>
        protected abstract IMqPublisher OnCreatePublisher(MqAddress address);

        /// <summary>
        /// Disposes the messaging system channel.
        /// </summary>
        protected abstract void OnDisposingChannel();

        /// <summary>
        /// Disposes the messaging system disposable component.
        /// </summary>
        protected override void OnDisposingMqDisposable()
        {
            log.DebugFormat("Disposing the channel.");
            OnDisposingChannel();
        }

        /// <summary>
        /// Raises the undeliverable message return notification.
        /// </summary>
        protected void RaiseReturned(ReturnedEventArgs e)
        {
            log.DebugFormat("Raising returned message notification; Address = {0}; CorrelationId = {1}, ReplyCode = {2}, ReplyText = {3}.", e.Address, e.MessageContext.Properties.CorrelationId, e.ReplyCode, e.ReplyText);
            var returned = Returned;
            if (returned != null)
                returned(this, e);
        }
    }
}