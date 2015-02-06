namespace Medseek.Util.MicroServices
{
    using System.IO;

    using Medseek.Util.Extensions;
    using Medseek.Util.Testing;

    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="MicroServiceDispatcher"/> class.
    /// </summary>
    [TestFixture]
    public sealed class MicroServiceBindingAttributeTests : TestFixture<MicroServiceBindingAttribute>
    {
        private const string Exchange = "medseek-util-test";
        private const string BindingKey = "MicroServiceDispatcher.HelperMicroService";
        private const string Queue = "Medseek.Util.MicroServices.MicroServiceDispatcherTests.HelperMicroService";
        private Mock<MyService> myService;
        private string addressString;
        private string addressStringWithoutQueue;
        

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            myService = new Mock<MyService>();
            addressString = string.Format("topic://{0}/{1}", Exchange, BindingKey) + "/" + Queue;
            addressStringWithoutQueue = string.Format("topic://{0}/{1}", Exchange, BindingKey);
        }

        [Test]
        public void QueueIsNull_ConstructedProperly()
        {
            var attribute = new MicroServiceBindingAttribute(Exchange, BindingKey, null);
            Assert.AreEqual(addressStringWithoutQueue, attribute.Address.Value);
            Assert.IsTrue(attribute.AutoDelete);
        }

        [Test]
        public void QueueIsSpecified_ConstructedProperly()
        {
            var attribute = new MicroServiceBindingAttribute(Exchange, BindingKey, Queue);
            Assert.AreEqual(addressString, attribute.Address.Value);
            Assert.IsTrue(attribute.AutoDelete);
        }

        [Test]
        public void FullAddressSpecified_ConstructedProperly()
        {
            var attribute = new MicroServiceBindingAttribute(addressString);
            Assert.AreEqual(addressString, attribute.Address.Value);
            Assert.IsTrue(attribute.AutoDelete);
        }

        [Test]
        public void ToBinding_BindingPropertiesAreExpected()
        {
            var attribute = new MicroServiceBindingAttribute(addressString);
            attribute.AutoAckDisabled = false;
            attribute.AutoDelete = false;
            attribute.IsOneWay = false;

            var methodInfo = TypeExt.GetMethod(myService.Object, x => x.Invoke1(null));

            var actualResult = attribute.ToBinding<MicroServiceBinding>(methodInfo, typeof(MyService));

            Assert.AreEqual(attribute.AutoDelete, actualResult.AutoDelete);
            Assert.AreEqual(attribute.AutoAckDisabled, actualResult.AutoAckDisabled);
            Assert.AreEqual(attribute.IsOneWay, actualResult.IsOneWay);
            Assert.AreEqual(attribute.Address, actualResult.Address);
            Assert.AreEqual(typeof(MyService), actualResult.Service);
            Assert.AreEqual(methodInfo, actualResult.Method);
        }

        /// <summary>
        /// Helper class used for verifying micro-service operation invocation 
        /// behavior.
        /// </summary>
        public abstract class MyService
        {
            [MicroServiceBinding(Exchange, BindingKey, Queue, AutoAckDisabled = false, AutoDelete = false)]
            public abstract void Invoke1(Stream body);
        }
    }
}