namespace Medseek.Util.Messaging
{
    using System;
    using System.Collections.Generic;
    using Medseek.Util.Testing;
    using NUnit.Framework;

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
            Obj.ContentType = contentType;

            var propertiesClone = Obj.Clone();

            Assert.That(propertiesClone.ContentType, Is.EqualTo(contentType));
        }

        /// <summary>
        /// Verifies that Clone sets the correlation id.
        /// </summary>
        [Test]
        public void MessagePropertiesCloneSetsCorrelationId()
        {
            var correlationId = "correlationId-" + Guid.NewGuid();
            Obj.CorrelationId = correlationId;

            var propertiesClone = Obj.Clone();

            Assert.That(propertiesClone.CorrelationId, Is.EqualTo(correlationId));
        }

        /// <summary>
        /// Verifies that Clone sets the reply to.
        /// </summary>
        [Test]
        public void MessagePropertiesCloneSetsReplyTo()
        {
            var replyTo = new MqAddress("replyTo-" + Guid.NewGuid());
            Obj.ReplyTo = replyTo;

            var propertiesClone = Obj.Clone();

            Assert.That(propertiesClone.ReplyTo, Is.EqualTo(replyTo));
        }

        /// <summary>
        /// Verifies that Clone sets the reply to.
        /// </summary>
        [Test]
        public void MessagePropertiesCloneSetsAdditionalProperties()
        {
            var additionalProperties = new Dictionary<string, object>
            {
                { "str", "testString" }, 
                { "obj", new List<int> { 7 } },
            };
            Obj.AdditionalProperties = additionalProperties;

            var propertiesClone = Obj.Clone();

            Assert.That(propertiesClone.AdditionalProperties, Is.EquivalentTo(additionalProperties));
        }
    }
}