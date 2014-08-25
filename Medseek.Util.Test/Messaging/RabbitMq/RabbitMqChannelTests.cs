namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using System.Linq;
    using Medseek.Util.MicroServices;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// Tests for the <see cref="RabbitMqChannel"/> class.
    /// </summary>
    [TestFixture]
    public sealed class RabbitMqChannelTests : TestFixture<RabbitMqChannel>
    {
        private Mock<IRabbitMqConnection> connection;
        private Mock<IMqConsumer> consumer;
        private Mock<IRabbitMqFactory> factory;
        private Mock<IModel> model;
        private Mock<IRabbitMqPlugin> plugin;
        private Mock<IMqPublisher> publisher;
        private Mock<IEventSubscriber> subscriber;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            connection = Mock<IRabbitMqConnection>();
            consumer = new Mock<IMqConsumer>();
            factory = Mock<IRabbitMqFactory>();
            model = new Mock<IModel>();
            plugin = Mock<IRabbitMqPlugin>();
            publisher = new Mock<IMqPublisher>();
            subscriber = new Mock<IEventSubscriber>();

            connection.Setup(x => 
                x.CreateModel())
                .Returns(model.Object);

            factory.Setup(x => 
                x.GetRabbitMqConsumer(model.Object, It.IsAny<RabbitMqAddress[]>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(consumer.Object);
            factory.Setup(x =>
                x.GetRabbitMqPublisher(model.Object, It.IsAny<MqAddress>()))
                .Returns(publisher.Object);

            plugin.Setup(x => 
                x.ToConsumerAddress(It.IsAny<MqAddress>()))
                .Returns((MqAddress a) => a as RabbitMqAddress ?? RabbitMqAddress.Parse(a.Value));

            Obj.Returned += subscriber.Object.OnReturned;
        }

        /// <summary>
        /// Verifies that the channel describes itself as capable of being 
        /// paused.
        /// </summary>
        [Test]
        [Ignore("Flow control seems to be broken is some versions of RabbitMQ.")]
        public void CanPauseReturnsTrue()
        {
            Assert.That(Obj.CanPause, Is.True);
        }

        /// <summary>
        /// Verifies that the component instance is retrieved from the factory.
        /// </summary>
        [Theory]
        public void CreateConsumerReturnsInstanceFromFactory(bool autoAckDisabled, bool autoDelete)
        {
            var address = new RabbitMqAddress("topic", "exchange-name", "routing-key", "queue-name");
            var result = Obj.CreateConsumer(address, autoAckDisabled, autoDelete);

            factory.Verify(x => 
                x.GetRabbitMqConsumer(model.Object, It.Is<RabbitMqAddress[]>(a => a.Contains(address)), autoAckDisabled, autoDelete));
            Assert.That(result, Is.SameAs(consumer.Object));
        }

        /// <summary>
        /// Verifies that the component instance is retrieved from the factory.
        /// </summary>
        [Test]
        public void CreatePublisherReturnsInstanceFromFactory()
        {
            var address = new RabbitMqAddress("topic", "exchange", "routing-key");
            var result = Obj.CreatePublisher(address);

            factory.Verify(x =>
                x.GetRabbitMqPublisher(model.Object, address));
            Assert.That(result, Is.SameAs(publisher.Object));
        }

        /// <summary>
        /// Verifies that the model instance is disposed.
        /// </summary>
        [Test]
        public void DisposeChannelInvokesModelDispose()
        {
            model.Setup(x => 
                x.Dispose())
                .Verifiable();

            Obj.Dispose();

            model.Verify();
        }

        /// <summary>
        /// Verifies that the component instance is released to the factory.
        /// </summary>
        [Theory]
        public void DisposeConsumerReleasesInstanceToFactory(bool autoAckDisabled, bool autoDelete)
        {
            var address = RabbitMqAddress.Parse(string.Empty);
            var createdConsumer = Obj.CreateConsumer(address, autoAckDisabled, autoDelete);
            Assume.That(createdConsumer, Is.SameAs(consumer.Object));

            factory.Setup(x => 
                x.Release(consumer.Object))
                .Verifiable();

            consumer.Raise(x => 
                x.Disposed += null, EventArgs.Empty);

            factory.Verify();
        }

        /// <summary>
        /// Verifies that the component instance is released to the factory.
        /// </summary>
        [Test]
        public void DisposePublisherReleasesInstanceToFactory()
        {
            var address = RabbitMqAddress.Parse(string.Empty);
            var createdPublisher = Obj.CreatePublisher(address);
            Assume.That(createdPublisher, Is.SameAs(publisher.Object));

            factory.Setup(x =>
                x.Release(publisher.Object))
                .Verifiable();

            publisher.Raise(x =>
                x.Disposed += null, EventArgs.Empty);

            factory.Verify();
        }

        /// <summary>
        /// Verifies that the channel describes itself as being paused 
        /// depending on whether the property was last set or cleared.
        /// </summary>
        [Theory]
        public void IsPausedReturnsLastSetValue(bool expectedValue)
        {
            Obj.IsPaused = expectedValue;
            Assert.That(Obj.IsPaused, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Verifies that setting or clearing the pause flag causes the model 
        /// object to be invoked.
        /// </summary>
        [Theory]
        public void IsPausedSetInvokesModelChannelFlow(bool isPaused)
        {
            Obj.IsPaused = !isPaused;

            model.Setup(x =>
                x.ChannelFlow(!isPaused))
                .Verifiable();

            Obj.IsPaused = isPaused;

            model.Verify();
        }

        /// <summary>
        /// Verifies that the message return notification is raised.
        /// </summary>
        [TestCase("direct")]
        [TestCase("topic")]
        public void ReturnedIsRaisedWhenModelBasicReturnIsRaised(string exchangeType)
        {
            var address = new RabbitMqAddress(exchangeType, "test-exchange", "routing-key");
            Obj.CreatePublisher(address);

            var basicReturnEventArgs = new BasicReturnEventArgs { Exchange = address.ExchangeName };
            var messageContext = new Mock<IMessageContext>();
            messageContext.Setup(x => 
                x.Properties)
                .Returns(new MessageProperties());
            var returnedEventArgs = new ReturnedEventArgs(messageContext.Object, address, 0, string.Empty);
            plugin.Setup(x =>
                x.ToReturnedEventArgs(address.ExchangeType, basicReturnEventArgs))
                .Returns(returnedEventArgs);

            subscriber.Setup(x => 
                x.OnReturned(Obj, returnedEventArgs))
                .Verifiable();

            model.Raise(x => 
                x.BasicReturn += null, basicReturnEventArgs);

            subscriber.Verify();
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