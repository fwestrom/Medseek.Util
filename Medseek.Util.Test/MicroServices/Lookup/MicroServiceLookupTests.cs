namespace Medseek.Util.MicroServices.Lookup
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Medseek.Util.Messaging;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="MicroServiceLookup"/> class.
    /// </summary>
    [TestFixture]
    public sealed class MicroServiceLookupTests : TestFixture<MicroServiceLookup>
    {
        private static readonly Random Rand = new Random();
        private MqAddress address;
        private Mock<IMqChannel> channel;
        private Mock<IMessageContext> messageContext;
        private MessageProperties messageContextProperties;
        private Mock<IMqPublisher> publisher;
        private Mock<IMicroServiceSerializer> serializer;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            address = new MqAddress("exchange-type://exchange-name/routing-key.original");
            channel = Mock<IMqChannel>();
            messageContext = Mock<IMessageContext>();
            messageContextProperties = new MessageProperties { ContentType = "ContentType-" + Guid.NewGuid() };
            publisher = new Mock<IMqPublisher>();
            serializer = Mock<IMicroServiceSerializer>();

            channel.Setup(x => 
                x.CreatePublisher(It.IsAny<MqAddress>()))
                .Returns(publisher.Object);
            messageContext.Setup(x => 
                x.Properties)
                .Returns(messageContextProperties);

            Obj.Start();
        }

        /// <summary>
        /// Verifies that the lookup address prefix value is as expected.
        /// </summary>
        [Test]
        public void LookupAddressPrefixIsCorrect()
        {
            const string expectedPrefix = "topic://medseek-util/medseek-lookup.1.";
            Assert.That(MicroServiceLookup.LookupAddressPrefix, Is.EqualTo(expectedPrefix));
        }

        /// <summary>
        /// Verifies that the method is marked with the correct micro-service
        /// binding.
        /// </summary>
        [TestCase("Reply", MicroServiceLookup.LookupAddressPrefix + "reply.#")]
        [TestCase("Update", MicroServiceLookup.LookupAddressPrefix + "update.#")]
        public void MicroServiceBindingMarkedOnMethods(string methodName, string expectedAddressValue)
        {
            var method = Obj.GetType().GetMethod(methodName);
            var attribute = method.GetCustomAttribute<MicroServiceBindingAttribute>();
            Assert.That(attribute.Address.Value, Is.EqualTo(expectedAddressValue));
            Assert.That(attribute.IsOneWay, Is.True);
        }

        /// <summary>
        /// Verifies that the resolved address is returned.
        /// </summary>
        [Test]
        [Timeout(10000)]
        public void ResolveReturnsAddressFromReply()
        {
            ResolveReturnsAddressFromRecordAction(Obj.Reply);
        }

        /// <summary>
        /// Verifies that the updated address is returned.
        /// </summary>
        [Test]
        [Timeout(10000)]
        public void ResolveReturnsAddressFromUpdate()
        {
            ResolveReturnsAddressFromRecordAction(Obj.Update);
        }

        /// <summary>
        /// Verifies that a returned query message causes the resolve to return
        /// null.
        /// </summary>
        [Test]
        [Timeout(10000)]
        public void ResolveReturnsNullFromReturnedQuery()
        {
            channel.Setup(x => 
                x.CreatePublisher(It.IsAny<MqAddress>()))
                .Callback((MqAddress a) =>
                    publisher.Setup(x =>
                        x.Publish(It.IsAny<byte[]>(), It.IsAny<MessageProperties>()))
                        .Callback((byte[] b, MessageProperties c) =>
                        {
                            var mc = new Mock<IMessageContext>();
                            var properties = new MessageProperties { CorrelationId = c.CorrelationId };
                            mc.Setup(x => x.Properties).Returns(properties);
                            channel.Raise(x => x.Returned += null, new ReturnedEventArgs(mc.Object, a, 999, "reply-text"));
                        }))
                .Returns(publisher.Object);

            var result = Obj.Resolve(address, Timeout.InfiniteTimeSpan);

            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// Verifies that the lookup query is sent correctly.
        /// </summary>
        [Test]
        [Timeout(10000)]
        public void ResolveSendsLookupQuery()
        {
            var contentType = messageContextProperties.ContentType;
            var id = address.Value;
            var queryAddress = new MqAddress(MicroServiceLookup.LookupAddressPrefix + "query." + id + "/");
            var queryBytes = Enumerable.Range(1, 512).Select(n => (byte)Rand.Next()).ToArray();
            var queryPublisher = new Mock<IMqPublisher>();
            channel.Setup(x => 
                x.CreatePublisher(It.Is<MqAddress>(a => a.Value == queryAddress.Value)))
                .Returns(queryPublisher.Object);
            serializer.Setup(x => 
                x.Serialize(contentType, typeof(LookupQuery), It.Is<LookupQuery>(a => a.Id == id), It.IsAny<Stream>()))
                .Callback((string a, Type b, object c, Stream d) => d.Write(queryBytes, 0, queryBytes.Length));
            publisher.Setup(x => 
                x.Publish(It.IsAny<byte[]>(), It.IsAny<MessageProperties>()))
                .Throws<Exception>();
            queryPublisher.Setup(x =>
                x.Publish(It.IsAny<byte[]>(), It.IsAny<MessageProperties>()))
                .Throws<Exception>();

            queryPublisher.Setup(x => 
                x.Publish(
                    queryBytes, 
                    It.Is<MessageProperties>(a => 
                        a.ContentType == contentType && 
                        a.ReplyToString == MicroServiceLookup.LookupAddressPrefix + ("reply." + id))))
                .Throws<Exception>()
                .Verifiable();

            try
            {
                Obj.Resolve(address, Timeout.InfiniteTimeSpan);
            }
            catch (Exception)
            {
                serializer.Verify();
                publisher.Verify();
            }
        }

        /// <summary>
        /// Verifies that the mandatory option is set for the publisher.
        /// </summary>
        [Test]
        [Timeout(10000)]
        public void ResolveSetsPublisherMandatoryOption()
        {
            publisher.Setup(x =>
                x.Publish(It.IsAny<byte[]>(), It.IsAny<MessageProperties>()))
                .Callback(() => 
                    publisher.VerifySet(x => 
                        x.Mandatory = true,
                        Times.AtLeastOnce))
                .Throws<Exception>();

            publisher.SetupSet(x => 
                x.Mandatory = true)
                .Verifiable();

            try
            {
                Obj.Resolve(address, Timeout.InfiniteTimeSpan);
            }
            catch (Exception)
            {
                publisher.Verify();
                publisher.VerifySet(x =>
                    x.Mandatory = false,
                    Times.Never);
            }
        }

        /// <summary>
        /// Verifies that the address provided by the record handler action is 
        /// returned by the resolve attempt.
        /// </summary>
        private void ResolveReturnsAddressFromRecordAction(Action<LookupRecord> recordAction)
        {
            var resolvedAddress = new MqAddress("resolved-exchange-type://resolved-exchange/resolved-routing-key");
            publisher.Setup(x =>
                x.Publish(It.IsAny<byte[]>(), It.IsAny<MessageProperties>()))
                .Callback((byte[] a, MessageProperties b) =>
                {
                    messageContext
                        .Setup(x => x.Properties)
                        .Returns(new MessageProperties { CorrelationId = b.CorrelationId });
                    recordAction(new LookupRecord { Id = address.Value, Address = resolvedAddress.Value });
                    messageContext
                        .Setup(x => x.Properties)
                        .Throws<InvalidOperationException>();
                });

            var result = Obj.Resolve(address, Timeout.InfiniteTimeSpan);

            Assert.That(result, Is.EqualTo(resolvedAddress));
        }
    }
}