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
        private readonly EventingBasicConsumer consumer = new EventingBasicConsumer();
        private readonly RabbitMqAddress[] addresses;
        private readonly IRabbitMqHelper helper;
        private readonly IModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConsumer" 
        /// /> class.
        /// </summary>
        public RabbitMqConsumer(RabbitMqAddress[] addresses, bool autoDelete, IRabbitMqHelper helper, IModel model)
            : base(addresses.Cast<MqAddress>().ToArray())
        {
            if (addresses.Select(x => x.QueueName).Distinct().Count() > 1)
                throw new ArgumentException("All addresses must share the same source key.", "addresses");
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (model == null)
                throw new ArgumentNullException("model");

            this.addresses = addresses;
            this.helper = helper;
            this.model = model;
            consumer.Received += OnConsumerReceived;

            var queue = addresses.Select(x => x.QueueName).Distinct().Single();
            model.QueueDeclare(queue, false, false, autoDelete, null);

            var declaredExchanges = new List<string>();
            foreach (var address in addresses.Where(DoBindQueue))
            {
                if (!declaredExchanges.Contains(address.ExchangeName))
                    model.ExchangeDeclare(address.ExchangeName, address.ExchangeType);
                model.QueueBind(queue, address.ExchangeName, address.RoutingKey);
            }

            model.BasicConsume(queue, true, consumer);
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