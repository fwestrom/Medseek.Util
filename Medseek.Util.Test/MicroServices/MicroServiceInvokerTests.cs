namespace Medseek.Util.MicroServices
{
    using System;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="MicroServiceInvoker"/> class.
    /// </summary>
    [TestFixture]
    public sealed class MicroServiceInvokerTests : TestFixture<MicroServiceInvoker>
    {
        private MicroServiceBinding binding;
        private Mock<IHelperMicroService> instance;
        private Mock<IEventSubscriber> subscriber;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Use(binding = new MicroServiceBinding { Method = typeof(IHelperMicroService).GetMethod("DoIt"), Service = typeof(IHelperMicroService) });
            Use<object>((instance = new Mock<IHelperMicroService>()).Object);
            
            subscriber = new Mock<IEventSubscriber>();
            Obj.Disposing += subscriber.Object.OnDisposing;
        }

        /// <summary>
        /// Verifies that the property value is returned from the constructor 
        /// dependency.
        /// </summary>
        [Test]
        public void BindingReturnsConstructorDependency()
        {
            Assert.That(Obj.Binding, Is.SameAs(binding));
        }

        /// <summary>
        /// Verifies that the disposing notification is raised.
        /// </summary>
        [Test]
        public void DisposeRaisesDisposing()
        {
            subscriber.Setup(x => 
                x.OnDisposing(Obj, EventArgs.Empty))
                .Verifiable();

            Obj.Dispose();

            subscriber.Verify();
        }

        /// <summary>
        /// Verifies that the instance method is invoked.
        /// </summary>
        [Test]
        public void InvokeCausesMethodInvocationOnInstance()
        {
            var parameterValue = new object();
            var returnValue = new object();
            instance.Setup(x => 
                x.DoIt(parameterValue))
                .Returns(returnValue)
                .Verifiable();

            var result = Obj.Invoke(new[] { parameterValue });

            instance.Verify();
            Assert.That(result, Is.SameAs(returnValue));
        }

        /// <summary>
        /// Verifies that an exception is thrown if the invoker was already
        /// disposed.
        /// </summary>
        [Test]
        public void InvokeThrowsIfAlreadyDisposed()
        {
            Obj.Dispose();
            
            TestDelegate action = () => Obj.Invoke(new[] { new object() });
            Assert.That(action, Throws.InstanceOf<ObjectDisposedException>());
        }

        /// <summary>
        /// Helper interface used for verifying event notification behavior.
        /// </summary>
        public interface IEventSubscriber
        {
            void OnDisposing(object sender, EventArgs e);
        }

        /// <summary>
        /// Helper interface for verifying invoker behavior.
        /// </summary>
        public interface IHelperMicroService
        {
            object DoIt(object value);
        }
    }
}