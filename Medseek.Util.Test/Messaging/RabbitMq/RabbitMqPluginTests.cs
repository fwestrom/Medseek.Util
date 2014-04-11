namespace Medseek.Util.Messaging.RabbitMq
{
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="RabbitMqComponents"/> class.
    /// </summary>
    [TestFixture]
    public sealed class RabbitMqPluginTests
    {
        private IMqPlugin obj;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            obj = RabbitMqComponents.Plugin;
        }

        /// <summary>
        /// Verifies that routing key matching works correctly.
        /// </summary>
        [TestCase("A.B.C.D", "A.B.C.X")]
        [TestCase("direct:///A.B.C.X", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.B.C", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.B.C.D", "A.B.C")]
        [TestCase("topic://medseek-api/A.*.C.D", "A.B.X.D")]
        [TestCase("topic://medseek-api/A.X.C.D", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.#.C.D", "A.B.C.B.D")]
        [TestCase("topic://medseek-api/A.#.C.D", "A1.B.C.D")]
        [TestCase("topic://medseek-api/A.B.#", "A.B")]
        [TestCase("topic://medseek-api/A.B.*", "A.B.X.D")]
        public void IsMatchReturnsFalseCorrectly(string addressText, string routingKey)
        {
            var address = new MqAddress(addressText);
            var properties = new MessageProperties { RoutingKey = routingKey };
            var result = obj.IsMatch(properties, address);
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Verifies that routing key matching works correctly.
        /// </summary>
        [TestCase("A.B.C.D", "A.B.C.D")]
        [TestCase("direct:///A.B.C.D", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.B.C.D", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.#.C.D", "A.B.C.D")]
        [TestCase("topic://medseek-api/A.#.C.D", "A.B1.B2.B3.C.D")]
        [TestCase("topic://medseek-api/A.B.*.D", "A.B.X.D")]
        public void IsMatchReturnsTrueCorrectly(string addressText, string routingKey)
        {
            var address = new MqAddress(addressText);
            var properties = new MessageProperties { RoutingKey = routingKey };
            var result = obj.IsMatch(properties, address);
            Assert.That(result, Is.True);
        }
    }
}