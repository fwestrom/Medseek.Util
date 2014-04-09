namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using Medseek.Util.Testing;
    using NUnit.Framework;
    using Moq;
    using RabbitMQ.Client;
    using System.Collections.Generic;

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
        private Mock<IBasicProperties> basicProperties;
        private Mock<IBasicProperties> basicPropertiesWithHeaders;
        private Mock<IMessageProperties> messageProperties;
        private string correlationId;
        private string exchange;
        private string host;
        private string cookie;

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
            correlationId = "correlationId-" + Guid.NewGuid();
            exchange = "exchange-" + Guid.NewGuid();
            host = "host-" + Guid.NewGuid();
            cookie = "cookie-" + Guid.NewGuid();

            basicProperties = new Mock<IBasicProperties>();
            basicProperties.Setup(x => x.ContentType).Returns(exchangeType);
            basicProperties.Setup(x => x.CorrelationId).Returns(correlationId);
            basicProperties.Setup(x => x.ReplyTo).Returns(address.ToString());
            basicProperties.Setup(x => x.Headers).
                Returns(new Dictionary<string, object>());
            basicPropertiesWithHeaders = new Mock<IBasicProperties>();
            basicPropertiesWithHeaders.Setup(x => x.Headers).
                Returns(new Dictionary<string, object>() { { "host", host }, { "cookie", cookie } });

            messageProperties = new Mock<IMessageProperties>();
            messageProperties.Setup(x => x.ContentType).Returns(exchangeType);
            messageProperties.Setup(x => x.CorrelationId).Returns(correlationId);
            messageProperties.Setup(x => x.ReplyTo).Returns(address);
            messageProperties.Setup(x => x.AdditionalProperties)
                .Returns(new Dictionary<string, object>() { { "host", host }, { "cookie", cookie } });
        }

        /// <summary>
        /// Verifies that the queue is returned correctly.
        /// </summary>
        [Test]
        public void ToRabbitMqAddressReturnsCorrectQueue()
        {
            var result = Obj.ToRabbitMqAddress(address);
            Assert.That(result.QueueName, Is.EqualTo(queueName));
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
        [TestCase("direct:///Test.Direct", "Test.Direct")]
        [TestCase("topic://medseek-api/Test.Topic", "Test.Topic")]
        public void ToPublicationAddressSetsRoutingKey(string text, string expectedValue)
        {
            var result = Obj.ToPublicationAddress(new MqAddress(text));
            Assert.That(result.RoutingKey, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Verifies that the correlation id is set correctly.
        /// </summary>
        [Test]
        public void ToPropertiesSetsCorrelationId()
        {
            var result = Obj.ToProperties(basicProperties.Object);
            Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
        }

        /// <summary>
        /// Verifies that the reply to is set correctly.
        /// </summary>
        [Test]
        public void ToPropertiesSetsReplyTo()
        {
            var result = Obj.ToProperties(basicProperties.Object);
            Assert.That(result.ReplyTo, Is.EqualTo(address));
        }

        /// <summary>
        /// Verifies that the content type is set correctly.
        /// </summary>
        [Test]
        public void ToPropertiesSetsContentType()
        {
            var result = Obj.ToProperties(basicProperties.Object);
            Assert.That(result.ContentType, Is.EqualTo(exchangeType));
        }

        /// <summary>
        /// Verifies that null is returned for any non-provided property.
        /// </summary>
        [Test]
        public void ToPropertiesReturnsNullOnNonProvidedProperty()
        {
            var result = Obj.ToProperties(basicProperties.Object);
            Assert.That(result.Get("doesnotexist"), Is.EqualTo(null));
        }

        /// <summary>
        /// Verifies that the host header is set correctly when provided.
        /// </summary>
        [Test]
        public void ToPropertiesSetsProvidedHostHeader()
        {
            var result = Obj.ToProperties(basicPropertiesWithHeaders.Object);
            Assert.That(result.Get("host"), Is.EqualTo(host));
        }

        /// <summary>
        /// Verifies that the cookie header is set correctly when provided.
        /// </summary>
        [Test]
        public void ToPropertiesSetsProvidedCookieHeader()
        {
            var result = Obj.ToProperties(basicPropertiesWithHeaders.Object);
            Assert.That(result.Get("cookie"), Is.EqualTo(cookie));
        }

        /// <summary>
        /// Verifies that the correlation id is set correctly.
        /// </summary>
        [Test]
        public void CreateBasicPropertiesSetsCorrelationId()
        {
            basicPropertiesWithHeaders.SetupSet(x => x.CorrelationId = correlationId).Verifiable();

            Obj.CreateBasicProperties(basicPropertiesWithHeaders.Object, messageProperties.Object);

            basicProperties.Verify();
        }

        /// <summary>
        /// Verifies that the reply to is set correctly.
        /// </summary>
        [Test]
        public void CreateBasicPropertiesSetsReplyTo()
        {
            basicPropertiesWithHeaders.SetupSet(x => x.ReplyTo = address.ToString()).Verifiable();

            Obj.CreateBasicProperties(basicPropertiesWithHeaders.Object, messageProperties.Object);

            basicProperties.Verify();
        }

        /// <summary>
        /// Verifies that the content type is set correctly.
        /// </summary>
        [Test]
        public void CreateBasicPropertiesSetsContentType()
        {
            basicPropertiesWithHeaders.SetupSet(x => x.ContentType = exchangeType).Verifiable();

            Obj.CreateBasicProperties(basicPropertiesWithHeaders.Object, messageProperties.Object);

            basicProperties.Verify();
        }

        /// <summary>
        /// Verifies that the host property is set correctly when provided.
        /// </summary>
        [Test]
        public void CreateBasicPropertiesSetsProvidedHostProperty()
        {
            var result = Obj.CreateBasicProperties(basicProperties.Object, messageProperties.Object);
            Assert.That(result.Headers["host"], Is.EqualTo(host));
        }

        /// <summary>
        /// Verifies that the cookie property is set correctly when provided.
        /// </summary>
        [Test]
        public void CreateBasicPropertiesSetsProvidedCookieProperty()
        {
            var result = Obj.CreateBasicProperties(basicProperties.Object, messageProperties.Object);
            Assert.That(result.Headers["cookie"], Is.EqualTo(cookie));
        }
    }
}