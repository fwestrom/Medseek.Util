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
        private Mock<IMqChannel> channel;
        private Mock<IMessageContextAccess> messageContextAccess;
        private Mock<IMqPlugin> plugin;
        private Mock<IMqPublisher> publisher;
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
            channel = Mock<IMqChannel>();
            messageContextAccess = Mock<IMessageContextAccess>();
            plugin = Mock<IMqPlugin>();
            publisher = new Mock<IMqPublisher>();
            serializer = Mock<IMicroServiceSerializer>();
            subscriber = new Mock<IEventSubscriber>();

            channel.Setup(x => 
                x.Plugin)
                .Returns(plugin.Object);
            channel.Setup(x => 
                x.CreatePublisher(binding.Address))
                .Returns(publisher.Object);

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
        /// Verifies that the remote micro-service is invoked.
        /// </summary>
        [Test]
        public void SendByAddressPublishesOutgoingMessage()
        {
            var body = Enumerable.Range(1, 100).Select(n => (byte)n).ToArray();
            var properties = new Mock<IMessageProperties>();
            publisher.Setup(x =>
                x.Publish(body, properties.Object))
                .Verifiable();

            Obj.Send(bindingAddress, body, properties.Object);

            publisher.Verify();
        }

        /// <summary>
        /// Verifies that the remote micro-service is invoked.
        /// </summary>
        [Test]
        public void SendByBindingOneWayPublishesOutgoingMessage()
        {
            var request = new object();
            var requestData = Enumerable.Range(1, 100).Select(n => (byte)n).ToArray();
            var messageContext = new Mock<IMessageContext>();
            var messageContextProperties = new Mock<IMessageProperties>();
            var messageContextDisposable = new Mock<IDisposable>();
            messageContext.Setup(x => 
                x.Properties)
                .Returns(messageContextProperties.Object);
            messageContextAccess.Setup(x => 
                x.Enter(null))
                .Callback(() => messageContextAccess.Setup(x => x.Current).Returns(messageContext.Object))
                .Returns(messageContextDisposable.Object);
            messageContextDisposable.Setup(x => 
                x.Dispose())
                .Callback(() => messageContextAccess.Setup(x => x.Current).Returns((IMessageContext)null));
            serializer.Setup(x => 
                x.Serialize(messageContext.Object, typeof(object), request, It.IsAny<Stream>()))
                .Callback((IMessageContext a, Type b, object c, Stream d) => d.Write(requestData, 0, requestData.Length));
            publisher.Setup(x => 
                x.Publish(requestData, messageContextProperties.Object))
                .Callback((byte[] a, IMessageProperties b) =>
                {
                    messageContextProperties.VerifySet(x => 
                        x.ReplyTo = null);
                    messageContextProperties.VerifySet(x =>
                        x.RoutingKey = DoItRoutingKey);
                })
                .Verifiable();

            Obj.Send(binding, request);

            publisher.Verify();
        }

        /// <summary>
        /// Verifies that an exception is thrown if the invoker was already
        /// disposed.
        /// </summary>
        [Test]
        public void SendByAddressThrowsIfAlreadyDisposed()
        {
            Obj.Dispose();

            TestDelegate action = () => Obj.Send(bindingAddress, new byte[0], new Mock<IMessageProperties>().Object);
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