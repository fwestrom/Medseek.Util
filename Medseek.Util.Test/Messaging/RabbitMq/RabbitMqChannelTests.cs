namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;
    using RabbitMQ.Client;

    /// <summary>
    /// Tests for the <see cref="RabbitMqChannel"/> class.
    /// </summary>
    [TestFixture]
    public sealed class RabbitMqChannelTests : TestFixture<RabbitMqChannel>
    {
        private Mock<IConnection> connection;
        private Mock<IMqConsumer> consumer;
        private Mock<IRabbitMqFactory> factory;
        private Mock<IModel> model;
        private Mock<IMqPublisher> publisher;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            connection = Mock<IConnection>();
            consumer = new Mock<IMqConsumer>();
            factory = Mock<IRabbitMqFactory>();
            model = new Mock<IModel>();
            publisher = new Mock<IMqPublisher>();

            connection.Setup(x => 
                x.CreateModel())
                .Returns(model.Object);

            factory.Setup(x => 
                x.GetRabbitMqConsumer(model.Object, It.IsAny<MqAddress>(), It.IsAny<bool>()))
                .Returns(consumer.Object);
            factory.Setup(x =>
                x.GetRabbitMqPublisher(model.Object, It.IsAny<MqAddress>()))
                .Returns(publisher.Object);
        }

        /// <summary>
        /// Verifies that the channel describes itself as capable of being 
        /// paused.
        /// </summary>
        [Test]
        public void CanPauseReturnsTrue()
        {
            Assert.That(Obj.CanPause, Is.True);
        }

        /// <summary>
        /// Verifies that the component instance is retrieved from the factory.
        /// </summary>
        [Theory]
        public void CreateConsumerReturnsInstanceFromFactory(bool autoDelete)
        {
            var address = new MqAddress();
            var result = Obj.CreateConsumer(address, autoDelete);

            factory.Verify(x => 
                x.GetRabbitMqConsumer(model.Object, address, autoDelete));
            Assert.That(result, Is.SameAs(consumer.Object));
        }

        /// <summary>
        /// Verifies that the component instance is retrieved from the factory.
        /// </summary>
        [Test]
        public void CreatePublisherReturnsInstanceFromFactory()
        {
            var address = new MqAddress();
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
        public void DisposeConsumerReleasesInstanceToFactory(bool autoDelete)
        {
            var address = new MqAddress();
            var createdConsumer = Obj.CreateConsumer(address, autoDelete);
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
            var address = new MqAddress();
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
    }
}