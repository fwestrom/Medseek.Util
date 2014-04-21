namespace Medseek.Util.Messaging.RabbitMq
{
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;
    using RabbitMQ.Client;

    /// <summary>
    /// Tests for the <see cref="RabbitMqConsumer"/> class.
    /// </summary>
    [TestFixture]
    public sealed class RabbitMqConsumerTests : TestFixture<RabbitMqConsumer>
    {
        private MqAddress address;
        private Mock<IRabbitMqPlugin> helper;
        private Mock<IModel> model;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Use(address = new MqAddress());
            helper = Mock<IRabbitMqPlugin>();
            model = Mock<IModel>();
        }

        // TODO: Finish implementing and testing RabbitMqConsumer.
    }
}