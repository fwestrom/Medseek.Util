namespace Medseek.Util.RabbitMq
{
    using System;
    using Medseek.Util.Messaging.RabbitMq;

    using Moq;
    using NUnit.Framework;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// Tests for the <see cref="MicroServiceDispatcher"/> class.
    /// </summary>
    [TestFixture]
    public sealed class RabbitMqConsumerTests
    {
        private const string Exchange = "medseek-util-test";
        private const string ExchangeType = "topic";
        private const string BindingKey = "MicroServiceDispatcher.HelperMicroService";
        private const string Queue = "Medseek.Util.MicroServices.MicroServiceDispatcherTests.HelperMicroService";
        private string addressString;

        private Mock<IModel> model;
        private Mock<IRabbitMqPlugin> plugin;
        private RabbitMqConsumer consumer;
        private RabbitMqAddress[] addresses;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            addresses = new[] { new RabbitMqAddress(ExchangeType, Exchange, BindingKey, Queue) };
            model = new Mock<IModel>();
            model.Setup(m => m.QueueDeclare(addresses[0].QueueName, false, false, false, null)).Verifiable();
            plugin = new Mock<IRabbitMqPlugin>();
            consumer = new RabbitMqConsumer(addresses, false, false, model.Object, plugin.Object);
        }

        [Test]
        public void CtorMoreThanOneDistinctQueueArgumentExceptionIsThrown()
        {
            addresses = new[]
                {
                    new RabbitMqAddress("topic", "exchange1", "routingKey1", "queueName1"),
                    new RabbitMqAddress("topic", "exchange2", "routingKey2", "queueName2")
                };
            Assert.Throws<ArgumentException>(() => new RabbitMqConsumer(addresses, false, false, model.Object, plugin.Object));
        }

        [Test]
        public void CtorModelIsNullArgumentNullExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMqConsumer(addresses, false, false, null, plugin.Object));
        }

        [Test]
        public void CtorPluginIsNullArgumentNullExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() => new RabbitMqConsumer(addresses, false, false, model.Object, null));
        }

        [Test]
        public void CtorModelQueueDeclaredIsCalled()
        {
            model.Verify();   
        }

        [Test]
        public void CtorThrowsOperationInterruptedExceptionMessageDoesNotMatchExceptionRethrown()
        {
            var exception = new RabbitMQ.Client.Exceptions.OperationInterruptedException(null);
            model.Setup(m => m.QueueDeclare(addresses[0].QueueName, false, false, false, null)).Throws(exception);
            Assert.Throws<RabbitMQ.Client.Exceptions.OperationInterruptedException>(() => new RabbitMqConsumer(addresses, false, false, model.Object, plugin.Object));
        }

        [Test]
        public void CtorhrowsOperationInterruptedExceptionMessageMatchesNewExceptionThrown()
        {
            var shutdownReason = new ShutdownEventArgs(ShutdownInitiator.Peer, 406, "PRECONDITION_FAILED");

            var exception = new RabbitMQ.Client.Exceptions.OperationInterruptedException(shutdownReason);
            model.Setup(m => m.QueueDeclare(addresses[0].QueueName, false, false, false, null)).Throws(exception);
            Assert.Throws<QueuePropertiesMismatchException>(() => new RabbitMqConsumer(addresses, false, false, model.Object, plugin.Object));
        }

        [Test]
        public void DoBindQueueIsFalseOnlyExpectedMethodsOnModelAreCalled()
        {
            addresses = new[] { new RabbitMqAddress(ExchangeType, null, BindingKey, Queue) };
            model = new Mock<IModel>(MockBehavior.Strict);
            model.Setup(m => m.QueueDeclare(addresses[0].QueueName, false, false, false, null)).Returns(new QueueDeclareOk(addresses[0].QueueName, 0, 0)).Verifiable();
            model.Setup(m => m.BasicConsume(addresses[0].QueueName, true, It.IsAny<EventingBasicConsumer>())).Returns("string").Verifiable();
            consumer = new RabbitMqConsumer(addresses, false, false, model.Object, plugin.Object);
            model.Verify();
        }

        [Test]
        public void DoBindQueueIsTrueModelExchangeDeclareAndQueueBindAreCalled()
        {
            model.Setup(m => m.ExchangeDeclare(addresses[0].ExchangeName, addresses[0].ExchangeType)).Verifiable();
            model.Setup(m => m.QueueBind(addresses[0].QueueName, addresses[0].ExchangeName, addresses[0].RoutingKey)).Verifiable();
            consumer = new RabbitMqConsumer(addresses, false, false, model.Object, plugin.Object);
            model.Verify();
        }
    }
}