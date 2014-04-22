namespace Medseek.Util.MicroServices.Lookup
{
    using System;
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
        private Mock<IMqChannel> channel;
        private Mock<IMicroServiceDispatcher> dispatcher;
        private Mock<IRemoteMicroServiceInvoker> invoker;
        private Mock<IMessageContext> messageContext;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            channel = Mock<IMqChannel>();
            dispatcher = Mock<IMicroServiceDispatcher>();
            invoker = new Mock<IRemoteMicroServiceInvoker>();
            messageContext = Mock<IMessageContext>();

            dispatcher.Setup(x => 
                x.RemoteMicroServiceInvoker)
                .Returns(invoker.Object);

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
            var originalAddress = new MqAddress("original-exchange-type://origninal-exchange/original-routing-key");
            invoker.Setup(x =>
                x.Send(It.IsAny<MqAddress>(), It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<MessageProperties>()))
                .Callback((MqAddress a, Type b, object c, MessageProperties d) =>
                {
                    var properties = new MessageProperties { CorrelationId = d.CorrelationId };
                    channel.Raise(x => x.Returned += null, new ReturnedEventArgs(a, new ArraySegment<byte>(), properties, 999, "reply-text"));
                });

            var result = Obj.Resolve(originalAddress, Timeout.InfiniteTimeSpan);

            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// Verifies that the lookup query is sent correctly.
        /// </summary>
        [Test]
        [Timeout(10000)]
        public void ResolveSendsLookupQuery()
        {
            var originalAddress = new MqAddress("original-exchange-type://origninal-exchange/original-routing-key");
            invoker.Setup(x => 
                x.Send(It.IsAny<MqAddress>(), It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<MessageProperties>()))
                .Throws<Exception>();

            var id = originalAddress.Value;
            var queryAddress = new MqAddress(MicroServiceLookup.LookupAddressPrefix + "query." + id + "/");
            invoker.Setup(x => 
                x.Send(
                    queryAddress, 
                    typeof(LookupQuery), 
                    It.Is<LookupQuery>(a => a.Id == id), 
                    It.Is<MessageProperties>(a => a.ReplyToString == MicroServiceLookup.LookupAddressPrefix + ("reply." + id))))
                .Throws<Exception>()
                .Verifiable();

            try
            {
                Obj.Resolve(originalAddress, Timeout.InfiniteTimeSpan);
            }
            catch (Exception)
            {
                invoker.Verify();
            }
        }

        /// <summary>
        /// Verifies that the address provided by the record handler action is 
        /// returned by the resolve attempt.
        /// </summary>
        private void ResolveReturnsAddressFromRecordAction(Action<LookupRecord> recordAction)
        {
            var originalAddress = new MqAddress("original-exchange-type://origninal-exchange/original-routing-key");
            var resolvedAddress = new MqAddress("resolved-exchange-type://resolved-exchange/resolved-routing-key");
            invoker.Setup(x =>
                x.Send(It.IsAny<MqAddress>(), It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<MessageProperties>()))
                .Callback((MqAddress a, Type b, object c, MessageProperties d) =>
                {
                    messageContext
                        .Setup(x => x.Properties)
                        .Returns(new MessageProperties { CorrelationId = d.CorrelationId });
                    recordAction(new LookupRecord { Id = originalAddress.Value, Address = resolvedAddress.Value });
                    messageContext
                        .Setup(x => x.Properties)
                        .Throws<InvalidOperationException>();
                });

            var result = Obj.Resolve(originalAddress, Timeout.InfiniteTimeSpan);

            Assert.That(result, Is.EqualTo(resolvedAddress));
        }
    }
}