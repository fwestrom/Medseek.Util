namespace Medseek.Util.Messaging
{
    using System;
    using NUnit.Framework;
    using Medseek.Util.Testing;
    using System.Collections.Generic;

    /// <summary>
    /// Tests for the <see cref="MessageProperties"/> class.
    /// </summary>
    [TestFixture]
    public sealed class MessagePropertiesTests : TestFixture<MessageProperties>
    {
        /// <summary>
        /// Verifies that Clone sets the content type.
        /// </summary>
        [Test]
        public void MessagePropertiesCloneSetsContentType()
        {
            var contentType = "contentType-" + Guid.NewGuid();
            var properties = new MessageProperties { ContentType = contentType };

            var propertiesClone = (MessageProperties)properties.Clone();

            Assert.That(propertiesClone.ContentType, Is.EqualTo(properties.ContentType));
        }

        /// <summary>
        /// Verifies that Clone sets the correlation id.
        /// </summary>
        [Test]
        public void MessagePropertiesCloneSetsCorrelationId()
        {
            var correlationId = "correlationId-" + Guid.NewGuid();
            var properties = new MessageProperties { CorrelationId = correlationId };

            var propertiesClone = (MessageProperties)properties.Clone();

            Assert.That(propertiesClone.CorrelationId, Is.EqualTo(properties.CorrelationId));
        }

        /// <summary>
        /// Verifies that Clone sets the routing key.
        /// </summary>
        [Test]
        public void MessagePropertiesCloneSetsRoutingKey()
        {
            var routingKey = "routingKey-" + Guid.NewGuid();
            var properties = new MessageProperties { RoutingKey = routingKey };

            var propertiesClone = (MessageProperties)properties.Clone();

            Assert.That(propertiesClone.RoutingKey, Is.EqualTo(properties.RoutingKey));
        }

        /// <summary>
        /// Verifies that Clone sets the reply to.
        /// </summary>
        [Test]
        public void MessagePropertiesCloneSetsReplyTo()
        {
            var replyTo = "replyTo-" + Guid.NewGuid();
            var properties = new MessageProperties { ReplyTo = new MqAddress(replyTo) };

            var propertiesClone = (MessageProperties)properties.Clone();

            Assert.That(propertiesClone.ReplyTo, Is.EqualTo(properties.ReplyTo));
        }

        /// <summary>
        /// Verifies that Clone sets the reply to.
        /// </summary>
        [Test]
        public void MessagePropertiesCloneSetsAdditionalProperties()
        {
            var testStr = "testString";
            var testObj = new List<int>() { 7 };

            var properties = new MessageProperties { AdditionalProperties = new Dictionary<string, object>() { { "str", testStr }, { "obj", testObj } } };

            var propertiesClone = (MessageProperties)properties.Clone();

            Assert.That(propertiesClone.AdditionalProperties, Is.EqualTo(properties.AdditionalProperties));
        }
    }
}