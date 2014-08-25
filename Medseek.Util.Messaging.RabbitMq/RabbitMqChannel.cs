namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Medseek.Util.Ioc;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// Provides a messaging system channel for interacting with RabbitMQ.
    /// </summary>
    [Register(typeof(IMqChannel), Lifestyle = Lifestyle.Transient)]
    public class RabbitMqChannel : MqChannelBase
    {
        private readonly Dictionary<string, string> exchangeTypes = new Dictionary<string, string>();
        private readonly IRabbitMqFactory factory;
        private readonly IModel model;
        private readonly IRabbitMqPlugin plugin;
        private bool isPaused;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqChannel" 
        /// /> class.
        /// </summary>
        public RabbitMqChannel(IRabbitMqConnection connection, IRabbitMqFactory factory, IRabbitMqPlugin plugin)
            : base(plugin)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (factory == null)
                throw new ArgumentNullException("factory");
            if (plugin == null)
                throw new ArgumentNullException("plugin");

            this.factory = factory;
            this.plugin = plugin;
            model = connection.CreateModel();
            model.BasicReturn += OnModelBasicReturn;
        }

        /// <summary>
        /// Gets a value indicating whether the channel can be paused.
        /// </summary>
        public override bool CanPause
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
        public override bool IsPaused
        {
            get
            {
                return isPaused;
            }
            set
            {
                if (value != isPaused)
                {
                    isPaused = value;
                    model.ChannelFlow(!value);
                }
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
        protected override IMqConsumer OnCreateConsumer(MqAddress address, bool autoAckDisabled, bool autoDelete)
        {
            var consumerAddress = TranslateAddress(address);
            var consumer = CreateConsumer(new[] { consumerAddress }, autoAckDisabled, autoDelete);
            return consumer;
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
        /// The message consumer components that were created.
        /// </returns>
        protected override IMqConsumer[] OnCreateConsumers(MqConsumerAddress[] addresses, bool autoAckDisabled, bool autoDelete)
        {
            return addresses
                .GroupBy(x => x.SourceKey)
                .Select(x => CreateConsumer(x.ToArray(), autoAckDisabled, autoDelete))
                .ToArray();
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
        protected override IMqPublisher OnCreatePublisher(MqAddress address)
        {
            var rabbitAddress = TranslateAddress(address);
            var publisher = factory.GetRabbitMqPublisher(model, rabbitAddress);
            publisher.Disposed += (sender, e) => factory.Release(publisher);
            return publisher;
        }

        /// <summary>
        /// Disposes the messaging system channel.
        /// </summary>
        protected override void OnDisposingChannel()
        {
            model.Dispose();
        }

        private IMqConsumer CreateConsumer(IEnumerable<MqConsumerAddress> addresses, bool autoAckDisabled, bool autoDelete)
        {
            var rabbitAddresses = addresses
                .Select(TranslateAddress)
                .ToArray();
            var consumer = factory.GetRabbitMqConsumer(model, rabbitAddresses, autoAckDisabled, autoDelete);
            consumer.Disposed += (sender, e) => factory.Release(consumer);
            return consumer;
        }

        private void OnModelBasicReturn(IModel sender, BasicReturnEventArgs e)
        {
            string exchangeType;
            if (!exchangeTypes.TryGetValue(e.Exchange, out exchangeType))
                exchangeType = "unknown";

            var returnedEventArgs = plugin.ToReturnedEventArgs(exchangeType, e);
            RaiseReturned(returnedEventArgs);
        }

        private RabbitMqAddress TranslateAddress(MqAddress address)
        {
            var ra = address as RabbitMqAddress ?? plugin.ToRabbitMqAddress(address);
            exchangeTypes[ra.ExchangeName] = ra.ExchangeType;
            return ra;
        }
    }
}