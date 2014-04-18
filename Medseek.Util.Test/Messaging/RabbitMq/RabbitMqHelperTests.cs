namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Testing;

    /// <summary>
    /// Tests for the <see cref="RabbitMqHelper"/> class.
    /// </summary>
    [TestFixture]
    public sealed class RabbitMqHelperTests : TestFixture<RabbitMqHelper>
    {
        /// <summary>
        /// Verifies that the queue is returned correctly.
        /// </summary>
        [Test]
        public void ToRabbitMqAddressReturnsCorrectQueue()
        {
            var exchangeType = "type-" + Guid.NewGuid().ToString("n");
            var exchangeName = "medseek-api-" + Guid.NewGuid().ToString("n");
            var routingKey = "Medseek.Platform.Services.Testing." + Guid.NewGuid().ToString("n");
            var queueName = "Medseek.Platform.Services.Testing.RabbitMqHelperTests." + Guid.NewGuid().ToString("n");
            var address = new MqAddress(string.Format("{0}://{1}/{2}/{3}", exchangeType, exchangeName, routingKey, queueName));

            var result = Obj.ToRabbitMqAddress(address);

            Assert.That(result.QueueName, Is.EqualTo(queueName));
        }

        /// <summary>
        /// Verifies that the header keys and values are copied over from the 
        /// basic properties object.
        /// </summary>
        [Test]
        public void ToPropertiesWithHeadersDictionarySetsValuesOnProperties()
        {
            var basicProperties = new Mock<IBasicProperties>();
            var headers = new Dictionary<string, object> { { "key1", "value1" }, { "key2", "value2" } };
            basicProperties.Setup(x => 
                x.Headers)
                .Returns(headers);

            MessageProperties result = null;
            TestDelegate action = () => result = Obj.ToProperties(basicProperties.Object);

            Assert.That(action, Throws.Nothing);
            Assert.That(result.AdditionalProperties, Is.EquivalentTo(headers));
        }

        /// <summary>
        /// Verifies that the publication address is set correctly.
        /// </summary>
        [Test]
        public void ToPublicationAddressSetsExchangeName()
        {
            var exchangeType = "type-" + Guid.NewGuid().ToString("n");
            var exchangeName = "medseek-api-" + Guid.NewGuid().ToString("n");
            var routingKey = "Medseek.Platform.Services.Testing." + Guid.NewGuid().ToString("n");
            var queueName = "Medseek.Platform.Services.Testing.RabbitMqHelperTests." + Guid.NewGuid().ToString("n");
            var address = new MqAddress(string.Format("{0}://{1}/{2}/{3}", exchangeType, exchangeName, routingKey, queueName));

            var result = Obj.ToPublicationAddress(address);

            Assert.That(result.ExchangeName, Is.EqualTo(exchangeName));
        }

        /// <summary>
        /// Verifies that the publication address is set correctly.
        /// </summary>
        [Test]
        public void ToPublicationAddressSetsExchangeType()
        {
            var exchangeType = "type-" + Guid.NewGuid().ToString("n");
            var exchangeName = "medseek-api-" + Guid.NewGuid().ToString("n");
            var routingKey = "Medseek.Platform.Services.Testing." + Guid.NewGuid().ToString("n");
            var queueName = "Medseek.Platform.Services.Testing.RabbitMqHelperTests." + Guid.NewGuid().ToString("n");
            var address = new MqAddress(string.Format("{0}://{1}/{2}/{3}", exchangeType, exchangeName, routingKey, queueName));

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
            var correlationId = "correlationId-" + Guid.NewGuid();
            var basicProperties = new Mock<IBasicProperties>();
            basicProperties.Setup(x => 
                x.CorrelationId)
                .Returns(correlationId);

            var result = Obj.ToProperties(basicProperties.Object);

            Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
        }

        /// <summary>
        /// Verifies that the reply to is set correctly.
        /// </summary>
        [Test]
        public void ToPropertiesSetsReplyTo()
        {
            var exchangeType = "type-" + Guid.NewGuid().ToString("n");
            var exchangeName = "medseek-api-" + Guid.NewGuid().ToString("n");
            var routingKey = "Medseek.Platform.Services.Testing." + Guid.NewGuid().ToString("n");
            var queueName = "Medseek.Platform.Services.Testing.RabbitMqHelperTests." + Guid.NewGuid().ToString("n");
            var replyTo = new MqAddress(string.Format("{0}://{1}/{2}/{3}", exchangeType, exchangeName, routingKey, queueName));
            var basicProperties = new Mock<IBasicProperties>();
            basicProperties.Setup(x => 
                x.ReplyTo)
                .Returns(replyTo.ToString());

            var result = Obj.ToProperties(basicProperties.Object);

            Assert.That(result.ReplyTo, Is.EqualTo(replyTo));
        }

        /// <summary>
        /// Verifies that the content type is set correctly.
        /// </summary>
        [Test]
        public void ToPropertiesSetsContentType()
        {
            var contentType = "type-" + Guid.NewGuid().ToString("n");
            var basicProperties = new Mock<IBasicProperties>();
            basicProperties.Setup(x => 
                x.ContentType)
                .Returns(contentType);

            var result = Obj.ToProperties(basicProperties.Object);

            Assert.That(result.ContentType, Is.EqualTo(contentType));
        }

        /// <summary>
        /// Verifies that null is returned for any non-provided property.
        /// </summary>
        [Test]
        public void ToPropertiesReturnsNullOnNonProvidedProperty()
        {
            var basicProperties = new Mock<IBasicProperties>();

            var result = Obj.ToProperties(basicProperties.Object);

            Assert.That(result.Get("DoesNotExist"), Is.EqualTo(null));
        }

        /// <summary>
        /// Verifies that the additional properties are set correctly when provided.
        /// </summary>
        [Test]
        public void ToPropertiesSetsProvidedAdditionalProperties()
        {
            var cookie = "cookie-" + Guid.NewGuid();
            var host = "host-" + Guid.NewGuid();
            var basicPropertiesWithHeaders = new Mock<IBasicProperties>();
            basicPropertiesWithHeaders.Setup(x =>
                x.Headers)
                .Returns(new Dictionary<string, object> { { "host", host }, { "cookie", cookie } });

            var result = Obj.ToProperties(basicPropertiesWithHeaders.Object);

            Assert.That(result.Get("host"), Is.EqualTo(host));
            Assert.That(result.Get("cookie"), Is.EqualTo(cookie));
        }

        /// <summary>
        /// Verifies that the correlation id is set correctly.
        /// </summary>
        [Test]
        public void CreateBasicPropertiesSetsCorrelationId()
        {
            var correlationId = "correlationId-" + Guid.NewGuid();
            var basicProperties = new Mock<IBasicProperties>();
            basicProperties.SetupProperty(x => x.CorrelationId);
            var model = new Mock<IModel>();
            model.Setup(x =>
                x.CreateBasicProperties())
                .Returns(basicProperties.Object);
            var messageProperties = new MessageProperties { CorrelationId = correlationId };

            var result = Obj.CreateBasicProperties(model.Object, messageProperties);

            Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
        }

        /// <summary>
        /// Verifies that the reply to is set correctly.
        /// </summary>
        [Test]
        public void CreateBasicPropertiesSetsReplyTo()
        {
            var exchangeType = "type-" + Guid.NewGuid().ToString("n");
            var exchangeName = "medseek-api-" + Guid.NewGuid().ToString("n");
            var routingKey = "Medseek.Platform.Services.Testing." + Guid.NewGuid().ToString("n");
            var queueName = "Medseek.Platform.Services.Testing.RabbitMqHelperTests." + Guid.NewGuid().ToString("n");
            var replyTo = new MqAddress(string.Format("{0}://{1}/{2}/{3}", exchangeType, exchangeName, routingKey, queueName));
            var basicProperties = new Mock<IBasicProperties>();
            basicProperties.SetupProperty(x => x.ReplyTo);
            var model = new Mock<IModel>();
            model.Setup(x =>
                x.CreateBasicProperties())
                .Returns(basicProperties.Object);
            var messageProperties = new MessageProperties { ReplyTo = replyTo };

            var result = Obj.CreateBasicProperties(model.Object, messageProperties);

            Assert.That(result.ReplyTo, Is.EqualTo(Obj.ToPublicationAddress(replyTo).ToString()));
        }

        /// <summary>
        /// Verifies that the content type is set correctly.
        /// </summary>
        [Test]
        public void CreateBasicPropertiesSetsContentType()
        {
            var contentType = "type-" + Guid.NewGuid().ToString("n");
            var basicProperties = new Mock<IBasicProperties>();
            basicProperties.SetupProperty(x => x.ContentType);
            var model = new Mock<IModel>();
            model.Setup(x =>
                x.CreateBasicProperties())
                .Returns(basicProperties.Object);
            var messageProperties = new MessageProperties { ContentType = contentType };

            var result = Obj.CreateBasicProperties(model.Object, messageProperties);

            Assert.That(result.ContentType, Is.EqualTo(contentType));
        }

        /// <summary>
        /// Verifies that the headers are set correctly when provided.
        /// </summary>
        [Test]
        public void CreateBasicPropertiesSetsProvidedHeaders()
        {
            var cookie = "cookie-" + Guid.NewGuid();
            var host = "host-" + Guid.NewGuid();
            var basicProperties = new Mock<IBasicProperties>();
            basicProperties.SetupProperty(x => x.Headers);
            var model = new Mock<IModel>();
            model.Setup(x =>
                x.CreateBasicProperties())
                .Returns(basicProperties.Object);
            var messageProperties = new MessageProperties { AdditionalProperties = new Dictionary<string, object> { { "host", host }, { "cookie", cookie } } };

            var result = Obj.CreateBasicProperties(model.Object, messageProperties);
            
            Assert.That(result.Headers["host"], Is.EqualTo(host));
            Assert.That(result.Headers["cookie"], Is.EqualTo(cookie));
        }
    }
}