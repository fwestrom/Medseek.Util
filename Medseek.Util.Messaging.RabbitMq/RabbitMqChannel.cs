﻿namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Medseek.Util.Ioc;
    using RabbitMQ.Client;

    /// <summary>
    /// Provides a messaging system channel for interacting with RabbitMQ.
    /// </summary>
    [Register(typeof(IMqChannel), Lifestyle = Lifestyle.Transient)]
    public class RabbitMqChannel : MqChannelBase
    {
        private readonly IRabbitMqFactory factory;
        private readonly IModel model;
        private bool isPaused;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqChannel" 
        /// /> class.
        /// </summary>
        public RabbitMqChannel(IConnection connection, IRabbitMqFactory factory, IMqPlugin plugin)
            : base(plugin)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (factory == null)
                throw new ArgumentNullException("factory");

            this.factory = factory;
            model = connection.CreateModel();
        }

        /// <summary>
        /// Gets a value indicating whether the channel can be paused.
        /// </summary>
        public override bool CanPause
        {
            get
            {
                return true;
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
        /// <param name="autoDelete">
        /// A value indicating whether closing the consumer should cause any 
        /// applicable services and messaging primitives to be removed.
        /// </param>
        /// <returns>
        /// The message consumer component that was created.
        /// </returns>
        protected override IMqConsumer OnCreateConsumer(MqAddress address, bool autoDelete)
        {
            var consumerAddress = Plugin.ToConsumerAddress(address);
            var consumer = CreateConsumer(new[] { consumerAddress }, autoDelete);
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
        /// <param name="autoDelete">
        /// A value indicating whether closing the consumer should cause any 
        /// applicable services and messaging primitives to be removed.
        /// </param>
        /// <returns>
        /// The message consumer components that were created.
        /// </returns>
        protected override IMqConsumer[] OnCreateConsumers(MqConsumerAddress[] addresses, bool autoDelete)
        {
            return addresses.GroupBy(x => x.SourceKey)
                .Select(x => CreateConsumer(x.ToArray(), autoDelete))
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
            var publisher = factory.GetRabbitMqPublisher(model, address);
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

        private IMqConsumer CreateConsumer(IEnumerable<MqConsumerAddress> addresses, bool autoDelete)
        {
            var rabbitAddresses = addresses
                .Select(x => x as RabbitMqAddress ?? (RabbitMqAddress)Plugin.ToConsumerAddress(x))
                .ToArray();
            var consumer = factory.GetRabbitMqConsumer(model, rabbitAddresses, autoDelete);
            consumer.Disposed += (sender, e) => factory.Release(consumer);
            return consumer;
        }
    }
}