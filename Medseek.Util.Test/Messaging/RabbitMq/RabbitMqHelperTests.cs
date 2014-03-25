namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using Medseek.Util.Testing;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="RabbitMqHelper"/> class.
    /// </summary>
    [TestFixture]
    public sealed class RabbitMqHelperTests : TestFixture<RabbitMqHelper>
    {
        private MqAddress address;
        private string exchangeName;
        private string exchangeType;
        private string queueName;
        private string routingKey;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            exchangeName = "medseek-api-" + Guid.NewGuid().ToString("n");
            exchangeType = "type-" + Guid.NewGuid().ToString("n");
            queueName = "Medseek.Platform.Services.Testing.RabbitMqHelperTests." + Guid.NewGuid().ToString("n");
            routingKey = "Medseek.Platform.Services.Testing." + Guid.NewGuid().ToString("n");
            address = new MqAddress(string.Format("{0}://{1}/{2}/{3}", exchangeType, exchangeName, routingKey, queueName));
        }

        /// <summary>
        /// Verifies that the queue is returned correctly.
        /// </summary>
        [Test]
        public void QueueReturnsCorrectValue()
        {
            var result = Obj.Queue(address);
            Assert.That(result, Is.EqualTo(queueName));
        }

        /// <summary>
        /// Verifies that the publication address is set correctly.
        /// </summary>
        [Test]
        public void ToPublicationAddressSetsExchangeName()
        {
            var result = Obj.ToPublicationAddress(address);
            Assert.That(result.ExchangeName, Is.EqualTo(exchangeName));
        }

        /// <summary>
        /// Verifies that the publication address is set correctly.
        /// </summary>
        [Test]
        public void ToPublicationAddressSetsExchangeType()
        {
            var result = Obj.ToPublicationAddress(address);
            Assert.That(result.ExchangeType, Is.EqualTo(exchangeType));
        }

        /// <summary>
        /// Verifies that the publication address is set correctly.
        /// </summary>
        [Test]
        public void ToPublicationAddressSetsRoutingKey()
        {
            var result = Obj.ToPublicationAddress(address);
            Assert.That(result.RoutingKey, Is.EqualTo(routingKey));
        }
    }
}