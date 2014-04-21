namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Medseek.Util.Ioc;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// A message consumer for interacting with RabbitMQ.
    /// </summary>
    [Register(typeof(IMqConsumer), Lifestyle = Lifestyle.Transient)]
    public class RabbitMqConsumer : MqConsumerBase
    {
        private readonly List<RabbitMqAddress> addresses = new List<RabbitMqAddress>();
        private readonly EventingBasicConsumer consumer = new EventingBasicConsumer();
        private readonly List<string> declaredExchanges = new List<string>(); 
        private readonly IRabbitMqPlugin helper;
        private readonly IModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConsumer" 
        /// /> class.
        /// </summary>
        public RabbitMqConsumer(RabbitMqAddress[] addresses, bool autoDelete, IRabbitMqPlugin helper, IModel model)
            : base(addresses.Cast<MqAddress>().ToArray())
        {
            if (addresses.Select(x => x.QueueName).Distinct().Count() > 1)
                throw new ArgumentException("All addresses must share the same source key.", "addresses");
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (model == null)
                throw new ArgumentNullException("model");

            this.helper = helper;
            this.model = model;
            consumer.Received += OnConsumerReceived;

            var queue = addresses.Select(x => x.QueueName).Distinct().Single();
            model.QueueDeclare(queue, false, false, autoDelete, null);

            foreach (var address in addresses.Where(DoBindQueue))
            {
                if (!declaredExchanges.Contains(address.ExchangeName))
                    model.ExchangeDeclare(address.ExchangeName, address.ExchangeType);
                model.QueueBind(queue, address.ExchangeName, address.RoutingKey);
            }

            model.BasicConsume(queue, true, consumer);
        }

        /// <summary>
        /// Binds the consumer using a messaging system address.
        /// </summary>
        public void Bind(MqAddress address, bool autoDelete)
        {
            var ra = helper.ToRabbitMqAddress(address);
            if (addresses.Count > 0 && ra.QueueName != addresses.Select(x => x.QueueName).Single())
                throw new ArgumentException("All addresses must share the same source queue.", "address");

            if (addresses.Count == 0)
                model.QueueDeclare(ra.QueueName, false, false, autoDelete, null);

            addresses.Add(ra);
            if (DoBindQueue(ra))
            {
                if (!declaredExchanges.Contains(ra.ExchangeName))
                {
                    model.ExchangeDeclare(ra.ExchangeName, ra.ExchangeType);
                    declaredExchanges.Add(ra.ExchangeName);
                }

                model.QueueBind(ra.QueueName, ra.ExchangeName, ra.RoutingKey);
            }
        }

        /// <summary>
        /// Disposes the messaging system consumer.
        /// </summary>
        protected override void OnDisposingConsumer()
        {
            foreach (var address in addresses.Where(DoBindQueue))
                model.QueueUnbind(address.QueueName, address.ExchangeName, address.RoutingKey, null);

            model.BasicCancel(consumer.ConsumerTag);
        }

        private static bool DoBindQueue(RabbitMqAddress address)
        {
            return !(string.IsNullOrEmpty(address.ExchangeName) || string.IsNullOrEmpty(address.ExchangeType));
        }

        private void OnConsumerReceived(IBasicConsumer sender, BasicDeliverEventArgs e)
        {
            var properties = helper.ToProperties(e);
            RaiseReceived(e.Body, 0, e.Body.Length, properties);
        }
    }
}