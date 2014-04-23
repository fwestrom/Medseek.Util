namespace Medseek.Util.MicroServices
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Medseek.Util.Messaging;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="RemoteMicroServiceInvoker" /> class.
    /// </summary>
    [TestFixture]
    public sealed class RemoteMicroServiceInvokerTests : TestFixture<RemoteMicroServiceInvoker>
    {
        private const string DoItExchangeName = "medseek-api";
        private const string DoItRoutingKey = "RemoteMicroServiceInvokerTests.Helper.DoIt";
        private MicroServiceBinding binding;
        private MqPublisherAddress bindingAddress;
        private Mock<IMicroServiceDispatcher> dispatcher;
        private Mock<IMessageContextAccess> messageContextAccess;
        private Mock<IMqPlugin> plugin;
        private Mock<IMicroServiceSerializer> serializer;
        private Mock<IEventSubscriber> subscriber;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var helperType = typeof(IHelperMicroService);
            var helperDoItMethod = helperType.GetMethod("DoIt");
            var helperDoItAttribute = helperDoItMethod.GetCustomAttribute<MicroServiceBindingAttribute>();
            binding = helperDoItAttribute.ToBinding<MicroServiceBinding>(helperDoItMethod, helperType);
            binding.Address = bindingAddress = new MqPublisherAddress(binding.Address.Value, DoItRoutingKey);
            dispatcher = Mock<IMicroServiceDispatcher>();
            messageContextAccess = Mock<IMessageContextAccess>();
            plugin = Mock<IMqPlugin>();
            serializer = Mock<IMicroServiceSerializer>();
            subscriber = new Mock<IEventSubscriber>();

            plugin.Setup(x =>
                x.ToPublisherAddress(It.IsAny<MqPublisherAddress>()))
                .Returns((MqAddress a) => (MqPublisherAddress)a);
            plugin.Setup(x => 
                x.ToPublisherAddress(binding.Address))
                .Returns((MqPublisherAddress)binding.Address);

            Obj.Disposing += subscriber.Object.OnDisposing;
        }

        /// <summary>
        /// Verifies that the disposing notification is raised.
        /// </summary>
        [Test]
        public void DisposeRaisesDisposing()
        {
            subscriber.Setup(x =>
                x.OnDisposing(Obj, EventArgs.Empty))
                .Verifiable();

            Obj.Dispose();

            subscriber.Verify();
        }

        /// <summary>
        /// Verifies that the micro-service lookup is invoked and the message 
        /// is published.
        /// </summary>
        [Test]
        public void SendByAddressBodyInvokesDispatcher()
        {
            SendInvokesDispatcher((x, address, body, properties) =>
                x.Send(address, body, properties));
        }

        /// <summary>
        /// Verifies that the micro-service lookup is invoked and the message 
        /// is published.
        /// </summary>
        [Test]
        public void SendByAddressTypeValueInvokesDispatcher()
        {
            SendInvokesDispatcher((x, address, body, properties) =>
            {
                var bodyType = typeof(object);
                var bodyValue = new object();
                serializer.Setup(s => 
                    s.Serialize(properties.ContentType, bodyType, bodyValue, It.IsAny<Stream>()))
                    .Callback((string a, Type b, object c, Stream d) => d.Write(body, 0, body.Length));
                x.Send(address, bodyType, bodyValue, properties);
            });
        }

        /// <summary>
        /// Verifies that the micro-service lookup is invoked and the message 
        /// is published.
        /// </summary>
        [Test]
        public void SendByBindingParametersPublishesWithResolvedOrOriginalIfNull()
        {
            SendInvokesDispatcher((x, address, body, properties) =>
            {
                var bodyType = typeof(object);
                var bodyValue = new object();
                serializer.Setup(s =>
                    s.Serialize(properties.ContentType, bodyType, bodyValue, It.IsAny<Stream>()))
                    .Callback((string a, Type b, object c, Stream d) => d.Write(body, 0, body.Length));
                x.Send(address, bodyType, bodyValue, properties);
            });
        }

        /// <summary>
        /// Verifies that an exception is thrown if the invoker was already
        /// disposed.
        /// </summary>
        [Test]
        public void SendByAddressBodyThrowsIfAlreadyDisposed()
        {
            Obj.Dispose();

            TestDelegate action = () => Obj.Send(bindingAddress, new byte[0], new MessageProperties());
            Assert.That(action, Throws.InstanceOf<ObjectDisposedException>());
        }

        /// <summary>
        /// Verifies that an exception is thrown if the invoker was already
        /// disposed.
        /// </summary>
        [Test]
        public void SendByAddressTypeValueThrowsIfAlreadyDisposed()
        {
            Obj.Dispose();

            TestDelegate action = () => Obj.Send(bindingAddress, typeof(object), new object(), new MessageProperties());
            Assert.That(action, Throws.InstanceOf<ObjectDisposedException>());
        }

        /// <summary>
        /// Verifies that an exception is thrown if the invoker was already
        /// disposed.
        /// </summary>
        [Test]
        public void SendByBindingThrowsIfAlreadyDisposed()
        {
            Obj.Dispose();

            TestDelegate action = () => Obj.Send(binding, new object());
            Assert.That(action, Throws.InstanceOf<ObjectDisposedException>());
        }

        private void SendInvokesDispatcher(
            Action<RemoteMicroServiceInvoker, MqAddress, byte[], MessageProperties> sendAction)
        {
            var address = new MqPublisherAddress("exchange-type://exchange-name/routing-key", "routing-key");
            var body = Enumerable.Range(1, 100).Select(n => (byte)n).ToArray();
            var properties = new MessageProperties { ContentType = "ContentType-" + Guid.NewGuid(), ReplyToString = "topic://medseek-test/remotemicroserviceinvokertests" };
            var messageContextDisposable = new Mock<IDisposable>();
            messageContextAccess.Setup(x =>
                x.Enter(null))
                .Callback(() =>
                {
                    var messageContext = new Mock<IMessageContext>();
                    messageContext.Setup(x =>
                        x.Properties)
                        .Returns(properties);
                    messageContextAccess.Setup(x =>
                        x.Current)
                        .Returns(messageContext.Object);
                    messageContextDisposable.Setup(x =>
                        x.Dispose())
                        .Callback(() =>
                            messageContextAccess.Setup(x =>
                                x.Current)
                                .Returns((IMessageContext)null))
                        .Verifiable();
                })
                .Returns(messageContextDisposable.Object);

            dispatcher.Setup(x =>
                x.Send(address, body, properties, true))
                .Verifiable();

            sendAction(Obj, address, body, properties);

            dispatcher.Verify();
            messageContextDisposable.Verify();
        }

        /// <summary>
        /// Helper interface used for verifying event notification behavior.
        /// </summary>
        public interface IEventSubscriber
        {
            void OnDisposing(object sender, EventArgs e);
        }

        /// <summary>
        /// Helper interface for verifying invoker behavior.
        /// </summary>
        public interface IHelperMicroService
        {
            [MicroServiceBinding(DoItExchangeName, DoItRoutingKey, "RemoteMicroServiceInvokerTests", IsOneWay = true)]
            void DoIt(object value);
        }
    }
}