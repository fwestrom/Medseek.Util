namespace Medseek.Util.Messaging
{
    using Medseek.Util.Logging;

    /// <summary>
    /// Provides common functionality to derived message system connection 
    /// types.
    /// </summary>
    public abstract class MqConnectionBase : MqSynchronizedDisposable, IMqConnection
    {
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqConnectionBase" /> 
        /// class.
        /// </summary>
        protected MqConnectionBase()
        {
            log = LogManager.GetLogger(GetType());
            log.DebugFormat("Creating the connection.");
        }

        /// <summary>
        /// Creates a new channel within the connection that can be used to 
        /// interact with the messaging system.
        /// </summary>
        /// <returns>
        /// The channel that was created.
        /// </returns>
        public IMqChannel CreateChannnel()
        {
            using (EnterDisposeLock())
            {
                log.DebugFormat("Creating a channel.");
                var channel = OnCreateChannel();
                OnDisposableCreated(channel);
                return channel;
            }
        }

        /// <summary>
        /// Creates a new channel within the connection that can be used to 
        /// interact with the messaging system.
        /// </summary>
        /// <returns>
        /// A new channel associated with the connection.
        /// </returns>
        protected abstract IMqChannel OnCreateChannel();

        /// <summary>
        /// Disposes the connection resources used by the connection.
        /// </summary>
        protected abstract void OnDisposingConnection();

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        protected override void OnDisposingMqDisposable()
        {
            log.DebugFormat("Disposing the connection.");
            OnDisposingConnection();
        }
    }
}