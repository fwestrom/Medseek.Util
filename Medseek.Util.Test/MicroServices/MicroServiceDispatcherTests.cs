namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Medseek.Util.Extensions;
    using Medseek.Util.Messaging;
    using Medseek.Util.Messaging.RabbitMq;
    using Medseek.Util.MicroServices.Lookup;
    using Medseek.Util.Testing;
    using Medseek.Util.Threading;
    using Moq;
    using NUnit.Framework;
    using MessageProperties = Medseek.Util.Messaging.MessageProperties;

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
        private Mock<IMicroServiceLocator> locator;
        private List<MicroServiceBinding> locatorBindings; 
        private Mock<IMicroServiceLookup> lookup;
        private Mock<IMessageContextAccess> messageContextAccess;
        private Mock<MyService> myService;
        private MicroServiceBinding myServiceInvoke1Binding;
        private Mock<IMqConsumer> myServiceInvoke1Consumer;
        private Mock<IMicroServiceInvoker> myServiceInvoke1Invoker;
        private Mock<IMqPlugin> plugin;
        private Mock<IEventSubscriber> subscriber;
        private Mock<IDispatchedThread> thread;

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
            messageContextAccess = Mock<IMessageContextAccess>();
            myService = new Mock<MyService>();
            myServiceInvoke1Binding = ToBinding(myService.Object, x => x.Invoke1(null));
            myServiceInvoke1Consumer = new Mock<IMqConsumer>();
            myServiceInvoke1Invoker = new Mock<IMicroServiceInvoker>();
            locator = Mock<IMicroServiceLocator>();
            locatorBindings = new List<MicroServiceBinding> { myServiceInvoke1Binding };
            plugin = Mock<IMqPlugin>();
            subscriber = new Mock<IEventSubscriber>();
            thread = Mock<IDispatchedThread>();

            channel.Setup(x =>
                x.CreateConsumers(It.Is<MqConsumerAddress[]>(a => a.Contains(myServiceInvoke1Binding.Address)), myServiceInvoke1Binding.AutoAckDisabled, true))
                .Returns(new[] { myServiceInvoke1Consumer.Object });
            connection.Setup(x => 
                x.CreateChannnel())
                .Returns(channel.Object);
            environment.Setup(x => 
                x.GetCommandLineArgs())
                .Returns(() => environmentCommandLineArgs.ToArray());
            factory.Setup(x => 
                x.GetLookup(channel.Object))
                .Returns(lookup.Object);
            locator.Setup(x =>
                x.Bindings)
                .Returns(locatorBindings);
            locator.Setup(x => 
                x.Get(myServiceInvoke1Binding, It.IsAny<object[]>()))
                .Returns(myServiceInvoke1Invoker.Object);
            messageContextAccess.Setup(x => 
                x.Enter(It.IsAny<MessageContext>()))
                .Returns(new Mock<IDisposable>().Object);
            plugin.Setup(x =>
                x.ToPublisherAddress(It.IsAny<MqPublisherAddress>()))
                .Returns((MqAddress a) => (MqPublisherAddress)a);
            plugin.Setup(x =>
                x.ToConsumerAddress(It.IsAny<MqConsumerAddress>()))
                .Returns((MqAddress a) => (MqConsumerAddress)a);
            thread.Setup(x => 
                x.Invoke(It.IsAny<Action>()))
                .Callback((Action a) => a());
            thread.Setup(x =>
                x.InvokeAsync(It.IsAny<Action>()))
                .Callback((Action a) => a());

            Obj.Returned += subscriber.Object.OnReturned;
            Obj.UnhandledException += subscriber.Object.OnUnhandledException;
            Obj.Start();
        }

        /// <summary>
        /// Verifies that the unhandled exception event is raised when a 
        /// micro-service invocation throws an exception.
        /// </summary>
        [Test]
        [Ignore("Not using the service invoker")]
        public void OnReceivedRaisesUnhandledExceptionWhenMicroServiceInvocationThrows()
        {
            var testException = new Exception("test-exception");
            myServiceInvoke1Invoker.Setup(x => 
                x.Invoke(It.IsAny<object[]>()))
                .Throws(testException);
            plugin.Setup(x => 
                x.IsMatch(It.IsAny<MessageContext>(), It.IsAny<MqAddress>()))
                .Returns(true);
            
            subscriber.Setup(x => 
                x.OnUnhandledException(Obj, It.Is<UnhandledExceptionEventArgs>(a => ReferenceEquals(a.Ex, testException))))
                .Verifiable();

            myServiceInvoke1Consumer.Raise(x =>
                x.Received += null, new ReceivedEventArgs(new MessageContext(new byte[0], MyService.RoutingKey, new MessageProperties())));

            subscriber.Verify();
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

        private static MicroServiceBinding ToBinding<T>(T obj, Expression<Action<T>> expression)
        {
            var method = TypeExt.GetMethod(expression);
            var attribute = method.GetCustomAttribute<MicroServiceBindingAttribute>();
            if (attribute == null)
                throw new InvalidOperationException("Attribute is not defined on the method.");
            var binding = attribute.ToBinding<MicroServiceBinding>(method, typeof(T));
            binding.Address = RabbitMqAddress.Parse(binding.Address.Value);
            return binding;
        }

        /// <summary>
        /// Helper interface used for verifying event notification behavior.
        /// </summary>
        public interface IEventSubscriber
        {
            void OnReturned(object sender, ReturnedEventArgs e);
            void OnUnhandledException(object sender, UnhandledExceptionEventArgs e);
        }

        /// <summary>
        /// Helper class used for verifying micro-service operation invocation 
        /// behavior.
        /// </summary>
        public abstract class MyService
        {
            internal const string RoutingKey = "MicroServiceDispatcher.HelperMicroService";
            [MicroServiceBinding("medseek-util-test", RoutingKey, "Medseek.Util.MicroServices.MicroServiceDispatcherTests.HelperMicroService")]
            public abstract void Invoke1(Stream body);
        }
    }
}