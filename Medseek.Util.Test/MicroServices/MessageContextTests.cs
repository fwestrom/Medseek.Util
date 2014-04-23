namespace Medseek.Util.MicroServices
{
    using System;
    using System.Linq;
    using Castle.Core.Internal;
    using Medseek.Util.Messaging;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="MessageContext"/> class.
    /// </summary>
    [TestFixture]
    public sealed class MessageContextTests : TestFixture<MessageContext>
    {
        private Mock<IEventSubscriber> subscriber;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Use(new byte[0]);
            Use("routing-key");
            Use(new MessageProperties());
            subscriber = new Mock<IEventSubscriber>();

            Obj.Acknowledged += subscriber.Object.OnAcknowledged;
        }

        /// <summary>
        /// Verifies that the acknowledged notification is raised.
        /// </summary>
        [Test]
        public void AckDoesNotRaiseAcknowledgedMultipleTimes()
        {
            Enumerable.Range(1, 10)
                .ForEach(x => Obj.Ack());

            subscriber.Verify(x =>
                x.OnAcknowledged(It.IsAny<object>(), It.IsAny<EventArgs>()),
                Times.AtMostOnce);
        }

        /// <summary>
        /// Verifies that the acknowledged notification is raised.
        /// </summary>
        [Test]
        public void AckRaisesAcknowledged()
        {
            subscriber.Setup(x =>
                x.OnAcknowledged(Obj, It.IsAny<EventArgs>()))
                .Verifiable();

            Obj.Ack();

            subscriber.Verify();
        }

        /// <summary>
        /// Helper interface used for verifying event notification behavior.
        /// </summary>
        public interface IEventSubscriber
        {
            void OnAcknowledged(object sender, EventArgs e);
        }
    }
}