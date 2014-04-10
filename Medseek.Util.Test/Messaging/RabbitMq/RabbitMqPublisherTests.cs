namespace Medseek.Util.Messaging.RabbitMq
{
    using System;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;
    using RabbitMQ.Client;

    /// <summary>
    /// Tests for the <see cref="RabbitMqPublisher"/> class.
    /// </summary>
    [TestFixture]
    public sealed class RabbitMqPublisherTests : TestFixture<RabbitMqPublisher>
    {
        private MqAddress address;
        private Mock<IRabbitMqHelper> helper;
        private Mock<IModel> model;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Use(address = new MqAddress("address"));
            helper = Mock<IRabbitMqHelper>();
            model = Mock<IModel>();
        }

        /// <summary>
        /// Verifies that the address provided to the constructor is returned 
        /// from the property.
        /// </summary>
        [Test]
        public void AddressReturnsAddressFromConstructor()
        {
            Assert.That(Obj.Address, Is.SameAs(address));
        }

        /// <summary>
        /// Verifies that the channel is invoked to publish the message.
        /// </summary>
        [Test]
        public void PublishInvokesModelBasicPublish()
        {
            var basicProperties = new Mock<IBasicProperties>();
            var correlationId = "CorrelationId-" + Guid.NewGuid();
            var exchange = "Exchange-" + Guid.NewGuid();
            var replyTo = new MqAddress("address");
            var routingKey = "RoutingKey-" + Guid.NewGuid();
            var pa = new PublicationAddress("topic", exchange, routingKey);
            helper.Setup(x =>
                x.CreateBasicProperties(
                    model.Object,
                    It.Is<MessageProperties>(a => 
                        a.CorrelationId == correlationId && 
                        a.ReplyTo == replyTo)))
                .Returns(basicProperties.Object);
            helper.Setup(x => 
                x.ToPublicationAddress(address))
                .Returns(pa);

            var body = new byte[0];
            model.Setup(x => 
                x.BasicPublish(pa, basicProperties.Object, body))
                .Verifiable();

            Obj.Publish(body, correlationId, replyTo);

            model.Verify();
        }
    }
}