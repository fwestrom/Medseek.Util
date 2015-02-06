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
    public sealed class MicroServiceDispatcherStartTests : TestFixture<MicroServiceDispatcher>
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
        private Mock<MyServiceWithoutAutoDelete> myServiceWithoutAutoDelete;
        private Mock<MyServiceMultipleBindingsWithAndWithoutAutoDelete> myServiceWithMultipleAutoDeleteBindings;
        private MicroServiceBinding myServiceInvoke1Binding;
        private MicroServiceBinding myServiceInvoke2Binding;
        private MicroServiceBinding myServiceInvoke3Binding;
        private MicroServiceBinding myServiceInvoke4Binding;
        private Mock<IMqConsumer> myServiceInvoke1Consumer;
        private Mock<IMqConsumer> myServiceInvoke2Consumer;
        private Mock<IMqConsumer> myServiceInvoke3Consumer;
        private Mock<IMqConsumer> myServiceInvoke4Consumer;
        private Mock<IMicroServiceInvoker> myServiceInvoke1Invoker;
        private Mock<IMicroServiceInvoker> myServiceInvoke2Invoker;
        private Mock<IMicroServiceInvoker> myServiceInvoke3Invoker;
        private Mock<IMicroServiceInvoker> myServiceInvoke4Invoker;
        private Mock<IMqPlugin> plugin;
        private Mock<IEventSubscriber> subscriber;
        private Mock<IDispatchedThread> thread;
        private Mock<IMicroServiceSerializer> serializer;

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

            channel.Setup(x => x.CreateConsumers(It.Is<MqConsumerAddress[]>(a => a.Contains(myServiceInvoke1Binding.Address)), myServiceInvoke1Binding.AutoAckDisabled, myServiceInvoke1Binding.AutoDelete)).Returns(new[] { myServiceInvoke1Consumer.Object }).Verifiable();
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
        }

        [Test]
        public void MicroServiceBindingAutoDeleteTrue_ChannelCreateConsumerIsCalledWithAutoDeleteTrue()
        {
            Obj.Start();
            channel.Verify();
        }

        [Test]
        public void MicroServiceBindingAutoDeleteFalse_ChannelCreateConsumerIsCalledWithAutoDeleteFalse()
        {
            channel = new Mock<IMqChannel>();
            environment = Mock<IEnvironment>();
            environmentCommandLineArgs = new List<string> { "/enableMicroServiceDispatcherLookup" };
            connection = Mock<IMqConnection>();
            factory = Mock<IMicroServicesFactory>();
            lookup = new Mock<IMicroServiceLookup>();
            messageContextAccess = Mock<IMessageContextAccess>();
            myServiceWithoutAutoDelete = new Mock<MyServiceWithoutAutoDelete>();
            myServiceInvoke1Binding = ToBinding(myServiceWithoutAutoDelete.Object, x => x.Invoke1(null));
            myServiceInvoke1Consumer = new Mock<IMqConsumer>();
            myServiceInvoke1Invoker = new Mock<IMicroServiceInvoker>();
            serializer = new Mock<IMicroServiceSerializer>();
            locator = Mock<IMicroServiceLocator>();
            locatorBindings = new List<MicroServiceBinding> { myServiceInvoke1Binding};
            plugin = Mock<IMqPlugin>();
            subscriber = new Mock<IEventSubscriber>();
            thread = Mock<IDispatchedThread>();

            channel.Setup(x => x.CreateConsumers(It.Is<MqConsumerAddress[]>(a => a.Contains(myServiceInvoke1Binding.Address)), myServiceInvoke1Binding.AutoAckDisabled, false)).Returns(new[] { myServiceInvoke1Consumer.Object }).Verifiable();
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
            var dispatcher = new MicroServiceDispatcher(
                connection.Object,
                environment.Object,
                messageContextAccess.Object,
                locator.Object,
                serializer.Object,
                factory.Object,
                plugin.Object,
                thread.Object);
            dispatcher.Returned += subscriber.Object.OnReturned;
            dispatcher.UnhandledException += subscriber.Object.OnUnhandledException;
            dispatcher.Start();
            channel.Verify();
        }

        [Test]
        public void MicroServiceBindingAutoDeleteAutoAckGrouping_ChannelCreateConsumerIsCalledCorrectly()
        {
            channel = new Mock<IMqChannel>();
            environment = Mock<IEnvironment>();
            environmentCommandLineArgs = new List<string> { "/enableMicroServiceDispatcherLookup" };
            connection = Mock<IMqConnection>();
            factory = Mock<IMicroServicesFactory>();
            lookup = new Mock<IMicroServiceLookup>();
            messageContextAccess = Mock<IMessageContextAccess>();
            myServiceWithMultipleAutoDeleteBindings = new Mock<MyServiceMultipleBindingsWithAndWithoutAutoDelete>();
            myServiceInvoke1Binding = ToBinding(myServiceWithMultipleAutoDeleteBindings.Object, x => x.Invoke1(null));
            myServiceInvoke2Binding = ToBinding(myServiceWithMultipleAutoDeleteBindings.Object, x => x.Invoke2(null));
            myServiceInvoke3Binding = ToBinding(myServiceWithMultipleAutoDeleteBindings.Object, x => x.Invoke3(null));
            myServiceInvoke4Binding = ToBinding(myServiceWithMultipleAutoDeleteBindings.Object, x => x.Invoke4(null));
            myServiceInvoke1Consumer = new Mock<IMqConsumer>();
            myServiceInvoke2Consumer = new Mock<IMqConsumer>();
            myServiceInvoke3Consumer = new Mock<IMqConsumer>();
            myServiceInvoke4Consumer = new Mock<IMqConsumer>();

            myServiceInvoke1Invoker = new Mock<IMicroServiceInvoker>();
            myServiceInvoke2Invoker = new Mock<IMicroServiceInvoker>();
            myServiceInvoke3Invoker = new Mock<IMicroServiceInvoker>();
            myServiceInvoke4Invoker = new Mock<IMicroServiceInvoker>();

            serializer = new Mock<IMicroServiceSerializer>();
            locator = Mock<IMicroServiceLocator>();
            locatorBindings = new List<MicroServiceBinding> { myServiceInvoke1Binding, myServiceInvoke2Binding, myServiceInvoke3Binding, myServiceInvoke4Binding };
            plugin = Mock<IMqPlugin>();
            subscriber = new Mock<IEventSubscriber>();
            thread = Mock<IDispatchedThread>();

            channel.Setup(x => x.CreateConsumers(It.Is<MqConsumerAddress[]>(a => a.Contains(myServiceInvoke1Binding.Address)), myServiceInvoke1Binding.AutoAckDisabled, myServiceInvoke1Binding.AutoDelete)).Returns(new[] { myServiceInvoke1Consumer.Object }).Verifiable();
            channel.Setup(x => x.CreateConsumers(It.Is<MqConsumerAddress[]>(a => a.Contains(myServiceInvoke2Binding.Address)), myServiceInvoke2Binding.AutoAckDisabled, myServiceInvoke2Binding.AutoDelete)).Returns(new[] { myServiceInvoke2Consumer.Object }).Verifiable();
            channel.Setup(x => x.CreateConsumers(It.Is<MqConsumerAddress[]>(a => a.Contains(myServiceInvoke3Binding.Address)), myServiceInvoke3Binding.AutoAckDisabled, myServiceInvoke3Binding.AutoDelete)).Returns(new[] { myServiceInvoke2Consumer.Object }).Verifiable();
            channel.Setup(x => x.CreateConsumers(It.Is<MqConsumerAddress[]>(a => a.Contains(myServiceInvoke4Binding.Address)), myServiceInvoke4Binding.AutoAckDisabled, myServiceInvoke4Binding.AutoDelete)).Returns(new[] { myServiceInvoke2Consumer.Object }).Verifiable();
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
            var dispatcher = new MicroServiceDispatcher(
                connection.Object,
                environment.Object,
                messageContextAccess.Object,
                locator.Object,
                serializer.Object,
                factory.Object,
                plugin.Object,
                thread.Object);
            dispatcher.Returned += subscriber.Object.OnReturned;
            dispatcher.UnhandledException += subscriber.Object.OnUnhandledException;
            dispatcher.Start();
            channel.Verify();
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

        public abstract class MyServiceWithoutAutoDelete
        {
            internal const string RoutingKey = "MicroServiceDispatcher.HelperMicroService";
            [MicroServiceBinding("medseek-util-test", RoutingKey, "Medseek.Util.MicroServices.MicroServiceDispatcherTests.HelperMicroService", AutoDelete = false)]
            public abstract void Invoke1(Stream body);
        }

        public abstract class MyServiceMultipleBindingsWithAndWithoutAutoDelete
        {
            internal const string RoutingKey = "MicroServiceDispatcher.HelperMicroService";
            [MicroServiceBinding("medseek-util-test", RoutingKey, "Medseek.Util.MicroServices.MicroServiceDispatcherTests.HelperMicroService", AutoAckDisabled = true, AutoDelete = false)]
            public abstract void Invoke1(Stream body);

            [MicroServiceBinding("medseek-util-test", RoutingKey + "2", "Medseek.Util.MicroServices.MicroServiceDispatcherTests.HelperMicroService2", AutoAckDisabled = true, AutoDelete = true)]
            public abstract void Invoke2(Stream body);

            [MicroServiceBinding("medseek-util-test", RoutingKey + "3", "Medseek.Util.MicroServices.MicroServiceDispatcherTests.HelperMicroService3", AutoAckDisabled = false, AutoDelete = false)]
            public abstract void Invoke3(Stream body);

            [MicroServiceBinding("medseek-util-test", RoutingKey + "4", "Medseek.Util.MicroServices.MicroServiceDispatcherTests.HelperMicroService4", AutoAckDisabled = false, AutoDelete = true)]
            public abstract void Invoke4(Stream body);
        }
    }
}