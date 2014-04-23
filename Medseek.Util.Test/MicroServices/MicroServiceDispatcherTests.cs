namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Medseek.Util.Messaging;
    using Medseek.Util.MicroServices.Lookup;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="MicroServiceDispatcher"/> class.
    /// </summary>
    [TestFixture]
    public sealed class MicroServiceDispatcherTests : TestFixture<MicroServiceDispatcher>
    {
        private Mock<IMqChannel> channel;
        private Mock<IEnvironment> environment;
        private List<string> environmentCommandLineArgs;
        private Mock<IMqConnection> connection;
        private Mock<IMicroServicesFactory> factory;
        private Mock<IMicroServiceLookup> lookup;
        private Mock<IMqPlugin> plugin;
        private Mock<IEventSubscriber> subscriber;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            channel = new Mock<IMqChannel>();
            environment = Mock<IEnvironment>();
            environmentCommandLineArgs = new List<string> { "/enableMicroServiceDispatcherLookup" };
            connection = Mock<IMqConnection>();
            factory = Mock<IMicroServicesFactory>();
            lookup = new Mock<IMicroServiceLookup>();
            plugin = Mock<IMqPlugin>();
            subscriber = new Mock<IEventSubscriber>();

            connection.Setup(x => 
                x.CreateChannnel())
                .Returns(channel.Object);
            environment.Setup(x => 
                x.GetCommandLineArgs())
                .Returns(() => environmentCommandLineArgs.ToArray());
            factory.Setup(x => 
                x.GetLookup(channel.Object))
                .Returns(lookup.Object);
            plugin.Setup(x =>
                x.ToPublisherAddress(It.IsAny<MqPublisherAddress>()))
                .Returns((MqAddress a) => (MqPublisherAddress)a);

            Obj.Returned += subscriber.Object.OnReturned;            
            Obj.Start();
        }

        /// <summary>
        /// Verifies that the returned message notification is raised.
        /// </summary>
        [Test]
        public void ReturnedIsRaisedWhenChannelReturnedIsRaised()
        {
            var e = new ReturnedEventArgs();
            subscriber.Setup(x => 
                x.OnReturned(Obj, e))
                .Verifiable();

            channel.Raise(x => 
                x.Returned += null, e);

            subscriber.Verify();
        }

        /// <summary>
        /// Verifies that the micro-service lookup is not performed if it has 
        /// been disabled by the option.
        /// </summary>
        [Test]
        public void SendDoesNotInvokeLookupIfEnableLookupIsFalse()
        {
        }

        /// <summary>
        /// Verifies that the micro-service lookup is invoked and the message 
        /// is published.
        /// </summary>
        [TestCase(false)]
        [TestCase(true)]
        public void SendPublishesWithResolvedOrOriginalIfNull(bool resolveReturnsAddress)
        {
            var body = Enumerable.Range(1, 100).Select(n => (byte)n).ToArray();
            var properties = new MessageProperties { ContentType = "ContentType-" + Guid.NewGuid(), ReplyToString = "topic://medseek-test/remotemicroserviceinvokertests" };
            var originalAddress = new MqPublisherAddress("exchange-type://exchange-name/routing-key.original", "routing-key.original");
            var resolvedAddress = resolveReturnsAddress ? new MqPublisherAddress("exchange-type://exchange-name/routing-key.resolved", "routing-key.resolved") : null;
            var publishAddress = resolvedAddress ?? originalAddress;
            var messageContextDisposable = new Mock<IDisposable>();
            var resolvedPublisher = new Mock<IMqPublisher>();

            lookup.Setup(x =>
                x.Resolve(originalAddress, It.IsAny<TimeSpan>()))
                .Returns(resolvedAddress)
                .Verifiable();
            channel.Setup(x =>
                x.CreatePublisher(publishAddress))
                .Returns(resolvedPublisher.Object);
            resolvedPublisher.Setup(x =>
                x.Publish(body, properties))
                .Verifiable();

            Obj.Send(originalAddress, body, properties, true);

            lookup.Verify();
            resolvedPublisher.Verify();
            messageContextDisposable.Verify();
        }

        /// <summary>
        /// Verifies that an exception is thrown if the dispatcher has been 
        /// disposed.
        /// </summary>
        [Test]
        public void SendThrowsIfDisposed()
        {
            Obj.Dispose();

            var address = new MqAddress("exchange-type://exchange-name/routing-key");
            var body = new byte[0];
            var properties = new MessageProperties();
            TestDelegate action = () => Obj.Send(address, body, properties, true);
            Assert.That(action, Throws.InstanceOf<ObjectDisposedException>());
        }

        /// <summary>
        /// Verifies that an exception is thrown if the dispatcher has been 
        /// disposed.
        /// </summary>
        [Test]
        public void StartThrowsIfDisposed()
        {
            Obj.Dispose();

            TestDelegate action = () => Obj.Start();
            Assert.That(action, Throws.InstanceOf<ObjectDisposedException>());
        }

        /// <summary>
        /// Verifies that an exception is thrown if the dispatcher has been 
        /// disposed.
        /// </summary>
        [Test]
        public void StopThrowsIfDisposed()
        {
            Obj.Dispose();

            TestDelegate action = () => Obj.Stop();
            Assert.That(action, Throws.InstanceOf<ObjectDisposedException>());
        }

        /// <summary>
        /// Helper interface used for verifying event notification behavior.
        /// </summary>
        public interface IEventSubscriber
        {
            void OnReturned(object sender, ReturnedEventArgs e);
        }
    }
}