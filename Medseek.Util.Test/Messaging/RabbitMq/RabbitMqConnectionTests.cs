namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;
    using RabbitMQ.Client;

    /// <summary>
    /// Tests for the <see cref="RabbitMqConnection"/> class.
    /// </summary>
    [TestFixture]
    public sealed class RabbitMqConnectionTests : TestFixture<RabbitMqConnection>
    {
        private Mock<IMqChannel> channel;
        private Mock<IConnection> connection;
        private Mock<IRabbitMqConnectionFactory> connectionFactory;
        private Mock<IRabbitMqFactory> factory;
        private Mock<IMqPlugin> plugin;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            channel = new Mock<IMqChannel>();
            connection = new Mock<IConnection>();
            connectionFactory = new Mock<IRabbitMqConnectionFactory>();
            Use(connectionFactory.Object);
            factory = Mock<IRabbitMqFactory>();
            plugin = Mock<IMqPlugin>();

            connectionFactory.Setup(x => 
                x.CreateConnection())
                .Returns(connection.Object);

            var obj = Obj;
            factory.Setup(x => 
                x.GetRabbitMqChannel(obj))
                .Returns(channel.Object);
        }

        /// <summary>
        /// Verifies that an exception is thrown if the dependencies are not 
        /// supplied.
        /// </summary>
        [Test]
        public void CtorRequiresDependencies0()
        {
            TestDelegate action = () => new RabbitMqConnection(null, factory.Object, plugin.Object);
            Assert.That(action, Throws.InstanceOf<ArgumentNullException>());
        }

        /// <summary>
        /// Verifies that an exception is thrown if the dependencies are not 
        /// supplied.
        /// </summary>
        [Test]
        public void CtorRequiresDependencies1()
        {
            TestDelegate action = () => new RabbitMqConnection(connectionFactory.Object, null, plugin.Object);
            Assert.That(action, Throws.InstanceOf<ArgumentNullException>());
        }

        /// <summary>
        /// Verifies that creating a channel returns the instance obtained from
        /// the factory.
        /// </summary>
        [Test]
        public void CreateChannelReturnsInstanceFromFactory()
        {
            var result = Obj.CreateChannnel();
            Assert.That(result, Is.SameAs(channel.Object));
        }

        /// <summary>
        /// Verifies that creating a model returns an instance obtained from 
        /// the corresponding RabbitMQ client library method.
        /// </summary>
        [Test]
        public void CreateModelReturnsInstanceFromRabbitMqLibConnection()
        {
            var model = new Mock<IModel>();
            connection.Setup(x => 
                x.CreateModel())
                .Returns(model.Object);

            var result = Obj.CreateModel();

            Assert.That(result, Is.SameAs(model.Object));
        }

        /// <summary>
        /// Verifies that the channel is released to the factory when it is 
        /// disposed.
        /// </summary>
        [Test]
        public void DisposeChannelInvokesFactoryRelease()
        {
            var createdChannel = Obj.CreateChannnel();
            Assume.That(createdChannel, Is.SameAs(channel.Object));

            factory.Setup(x => 
                x.Release(channel.Object))
                .Verifiable();

            channel.Raise(x => 
                x.Disposed += null, EventArgs.Empty);

            factory.Verify();
        }

        /// <summary>
        /// Verifies that the connection is disposed.
        /// </summary>
        [Test]
        public void DisposeDisposesConnection()
        {
            connection.Setup(x => 
                x.Dispose())
                .Verifiable();

            Obj.Dispose();

            connection.Verify();
        }
    }
}