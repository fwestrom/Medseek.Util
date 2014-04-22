namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Medseek.Util.MicroServices;
    using Moq;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Testing;

    /// <summary>
    /// Tests for the <see cref="RabbitMqPlugin"/> class.
    /// </summary>
    [TestFixture]
    public sealed class RabbitMqPluginTests : TestFixture<RabbitMqPlugin>
    {
        private static readonly Random Rand = new Random();
        private Mock<IMessageContext> messageContext;
        private MessageProperties messageContextProperties;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            messageContext = new Mock<IMessageContext>();
            messageContextProperties = new MessageProperties();

            messageContext.Setup(x => 
                x.Properties)
                .Returns(messageContextProperties);
        }

        /// <summary>
        /// Verifies that routing key matching works correctly.
        /// </summary>
        [TestCase("A.B.C.D", "A.B.C.X")]
        [TestCase("direct:///A.B.C.X", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.B.C", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.B.C.D", "A.B.C")]
        [TestCase("topic://medseek-api/A.*.C.D", "A.B.X.D")]
        [TestCase("topic://medseek-api/A.X.C.D", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.#.C.D", "A.B.C.B.D")]
        [TestCase("topic://medseek-api/A.#.C.D", "A1.B.C.D")]
        [TestCase("topic://medseek-api/A.B.#", "A.B")]
        [TestCase("topic://medseek-api/A.B.*", "A.B.X.D")]
        public void IsMatchReturnsFalseCorrectly(string addressText, string routingKey)
        {
            var address = new MqAddress(addressText);
            messageContext.Setup(x => 
                x.RoutingKey)
                .Returns(routingKey);

            var result = Obj.IsMatch(messageContext.Object, address);

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Verifies that routing key matching works correctly.
        /// </summary>
        [TestCase("A.B.C.D", "A.B.C.D")]
        [TestCase("direct:///A.B.C.D", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.B.C.D", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.#.C.D", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.#.C.D", "A.B1.B2.B3.C.D")]
        [TestCase("topic://medseek-api/A.B.*.D", "A.B.X.D")]
        public void IsMatchReturnsTrueCorrectly(string addressText, string routingKey)
        {
            var address = new MqAddress(addressText);
            messageContext.Setup(x =>
                x.RoutingKey)
                .Returns(routingKey);

            var result = Obj.IsMatch(messageContext.Object, address);

            Assert.That(result, Is.True);
        }

        /// <summary>
        /// Verifies that the message context is set up correctly.
        /// </summary>
        [Test]
        public void ToMessageContextSetsBody()
        {
            var body = Enumerable.Range(1, 512).Select(n => (byte)Rand.Next()).ToArray();
            var e = new BasicDeliverEventArgs
            {
                BasicProperties = new Mock<IBasicProperties>().Object,
                Body = body,
            };

            var result = Obj.ToMessageContext(e);

            Assert.That(result.BodyLength, Is.EqualTo(body.Length));
            Assert.That(result.GetBodyArray(), Is.EqualTo(body));
            Assert.That(new BinaryReader(result.GetBodyStream()).ReadBytes(result.BodyLength), Is.EqualTo(body));
        }

        /// <summary>
        /// Verifies that the message context is set up correctly.
        /// </summary>
        [Test]
        public void ToMessageContextSetsRoutingKey()
        {
            var e = new BasicDeliverEventArgs
            {
                BasicProperties = new Mock<IBasicProperties>().Object,
                RoutingKey = "RoutingKey-" + Guid.NewGuid(),
            };

            var result = Obj.ToMessageContext(e);

            Assert.That(result.RoutingKey, Is.EqualTo(e.RoutingKey));
        }

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

        /// <summary>
        /// Verifies that the returned message event data includes the correct 
        /// property values.
        /// </summary>
        [TestCase("direct", "direct-exchange", "direct-routing-key")]
        [TestCase("topic", "topic-exchange", "topic-routing-key")]
        public void ToReturnedEventArgsSetsAddress(string exchangeType, string exchangeName, string routingKey)
        {
            var result = Obj.ToReturnedEventArgs(exchangeType, new BasicReturnEventArgs
            {
                BasicProperties = new Mock<IBasicProperties>().Object,
                Body = new byte[0],
                Exchange = exchangeName,
                RoutingKey = routingKey,
            });

            Assert.That(result.Address.Value, Is.EqualTo(string.Format("{0}://{1}/{2}", exchangeType, exchangeName, routingKey)));
        }

        /// <summary>
        /// Verifies that the returned message event data includes the correct 
        /// property values.
        /// </summary>
        [Test]
        public void ToReturnedEventArgsSetsBody()
        {
            var body = new byte[0];

            var result = Obj.ToReturnedEventArgs("topic", new BasicReturnEventArgs
            {
                BasicProperties = new Mock<IBasicProperties>().Object,
                Body = body,
            });

            Assert.That(result.MessageContext.GetBodyArray(), Is.SameAs(body));
        }

        /// <summary>
        /// Verifies that the returned message event data includes the correct 
        /// property values.
        /// </summary>
        [Test]
        public void ToReturnedEventArgsSetsProperties()
        {
            var basicProperties = new Mock<IBasicProperties>();
            var properties = NewMessageProperties(basicProperties);

            var result = Obj.ToReturnedEventArgs("topic", new BasicReturnEventArgs
            {
                BasicProperties = basicProperties.Object,
                Body = new byte[0],
            });

            Assert.That(result.MessageContext.Properties.ContentType, Is.EqualTo(properties.ContentType));
            Assert.That(result.MessageContext.Properties.CorrelationId, Is.EqualTo(properties.CorrelationId));
            Assert.That(result.MessageContext.Properties.ReplyToString, Is.EqualTo(properties.ReplyToString));
            Assert.That(result.MessageContext.Properties.AdditionalProperties, Is.EquivalentTo(properties.AdditionalProperties));
        }

        /// <summary>
        /// Verifies that the returned message event data includes the correct 
        /// property values.
        /// </summary>
        [Test]
        public void ToReturnedEventArgsSetsReplyCodeReplyText()
        {
            var replyCode = (ushort)Rand.Next();
            var replyText = "ReplyText_" + replyCode + "_" + Guid.NewGuid();

            var result = Obj.ToReturnedEventArgs("topic", new BasicReturnEventArgs
            {
                BasicProperties = new Mock<IBasicProperties>().Object,
                Body = new byte[0],
                ReplyCode = replyCode,
                ReplyText = replyText,
            });

            Assert.That(result.ReplyCode, Is.EqualTo(replyCode));
            Assert.That(result.ReplyText, Is.EqualTo(replyText));
        }

        private static MessageProperties NewMessageProperties(Mock<IBasicProperties> bp = null)
        {
            var result = new MessageProperties
            {
                ContentType = "ContentType-" + Guid.NewGuid(),
                CorrelationId = "CorrelationId-" + Guid.NewGuid(),
                ReplyToString = "topic://exchange-" + Guid.NewGuid() + "/routing-key-" + Guid.NewGuid(),
                AdditionalProperties = new Dictionary<string, object>
                {
                    { "key-1", "value-1" },
                    { "key-2", new object() },
                    { "key-3", 3 },
                },
            };

            if (bp != null)
            {
                bp.Setup(x => x.ContentType).Returns(result.ContentType);
                bp.Setup(x => x.CorrelationId).Returns(result.CorrelationId);
                bp.Setup(x => x.ReplyTo).Returns(result.ReplyToString);
                bp.Setup(x => x.Headers).Returns(new Dictionary<string, object>(result.AdditionalProperties));
            }

            return result;
        }
    }
}