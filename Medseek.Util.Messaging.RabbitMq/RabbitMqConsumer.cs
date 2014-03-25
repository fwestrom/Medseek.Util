namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
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
        private readonly IRabbitMqHelper helper;
        private readonly IModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConsumer" 
        /// /> class.
        /// </summary>
        public RabbitMqConsumer(MqAddress address, bool autoDelete, IRabbitMqHelper helper, IModel model)
            : base(address)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");
            if (model == null)
                throw new ArgumentNullException("model");

            this.helper = helper;
            this.model = model;
            consumer.Received += OnConsumerReceived;

            var queue = helper.Queue(address);
            model.QueueDeclare(queue, false, false, autoDelete, null);

            var pa = helper.ToPublicationAddress(address);
            if (!(string.IsNullOrEmpty(pa.ExchangeName) || string.IsNullOrEmpty(pa.ExchangeType)))
            {
                model.ExchangeDeclare(pa.ExchangeName, pa.ExchangeType);
                model.QueueBind(queue, pa.ExchangeName, pa.RoutingKey);
            }

            model.BasicConsume(address.Value, true, consumer);
        }

        /// <summary>
        /// Disposes the messaging system consumer.
        /// </summary>
        protected override void OnDisposingConsumer()
        {
            var pa = helper.ToPublicationAddress(Address);
            if (!(string.IsNullOrEmpty(pa.ExchangeName) || string.IsNullOrEmpty(pa.ExchangeType)))
            {
                var queue = helper.Queue(Address);
                model.QueueUnbind(queue, pa.ExchangeName, pa.RoutingKey, null);
            }

            model.BasicCancel(consumer.ConsumerTag);
        }

        private void OnConsumerReceived(IBasicConsumer sender, BasicDeliverEventArgs e)
        {
            var properties = helper.ToProperties(e.BasicProperties);
            RaiseReceived(e.Body, 0, e.Body.Length, properties);
        }
    }
}