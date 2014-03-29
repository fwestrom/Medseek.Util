namespace Medseek.Util.Messaging
{
    using System.IO;

    /// <summary>
    /// Interface for types that provide the functionality of a channel or 
    /// session that is multiplexed into a connection that is shared with other
    /// channels.
    /// </summary>
    public interface IMqChannel : IMqDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the channel can be paused.
        /// </summary>
        bool CanPause
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the channel is paused to 
        /// enable flow control of messages.
        /// </summary>
        bool IsPaused
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets the messaging system plugin associated with the channel.
        /// </summary>
        IMqPlugin Plugin
        {
            get;
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
        IMqConsumer CreateConsumer(MqAddress address, bool autoDelete);

        /// <summary>
        /// Creates a set of consumers consumer that can be used to receive 
        /// messages from the messaging system channel.
        /// </summary>
        /// <param name="addresses">
        /// A set of consumer addresses describing the messaging primitives to 
        /// which the consumer binds for incoming messages, all of which must 
        /// have the same <see cref="MqConsumerAddress.SourceKey"/>.
        /// </param>
        /// <param name="autoDelete">
        /// A value indicating whether closing the consumer should cause any 
        /// applicable services and messaging primitives to be removed.
        /// </param>
        /// <returns>
        /// The message consumer component that was created.
        /// </returns>
        IMqConsumer[] CreateConsumers(MqConsumerAddress[] addresses, bool autoDelete);

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
        IMqPublisher CreatePublisher(MqAddress address);

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
        IMqRpcClient<byte[], Stream> CreateRpcClient(MqAddress address);
    }
}