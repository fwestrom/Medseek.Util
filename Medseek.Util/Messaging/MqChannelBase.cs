namespace Medseek.Util.Messaging
{
    using System;
    using System.IO;
    using Medseek.Util.Logging;

    /// <summary>
    /// Provides common functionality to messaging system channel components.
    /// </summary>
    public abstract class MqChannelBase : MqSynchronizedDisposable, IMqChannel
    {
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqChannelBase"/> class.
        /// </summary>
        protected MqChannelBase()
        {
            log = LogManager.GetLogger(GetType());
            log.DebugFormat("Creating channel.");
        }

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
        /// Creates a consumer that can be used to receive messages from the 
        /// messaging system channel.
        /// </summary>
        /// <param name="address">
        /// A description of the services and messaging primitives to which the
        /// consumer binds for incoming messages.
        /// </param>
        /// <param name="autoDelete">
        /// A value indicating whether closing the consumer should cause any 
        /// applicable services and messaging primitives to be removed.
        /// </param>
        /// <returns>
        /// The message consumer component that was created.
        /// </returns>
        public IMqConsumer CreateConsumer(MqAddress address, bool autoDelete)
        {
            using (EnterDisposeLock())
            {
                log.DebugFormat("Creating consumer; Address = {0}, AutoDelete = {1}.", address, autoDelete);
                var consumer = OnCreateConsumer(address, autoDelete);
                OnDisposableCreated(consumer);
                return consumer;
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
        /// <param name="autoDelete">
        /// A value indicating whether closing the consumer should cause any 
        /// applicable services and messaging primitives to be removed.
        /// </param>
        /// <returns>
        /// The message consumer component that was created.
        /// </returns>
        protected abstract IMqConsumer OnCreateConsumer(MqAddress address, bool autoDelete);

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
    }
}