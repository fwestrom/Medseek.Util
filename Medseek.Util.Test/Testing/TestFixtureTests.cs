namespace Medseek.Util.Testing
{
    using System;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="TestFixture{T}" /> class.
    /// </summary>
    [TestFixture]
    public class TestFixtureTests
    {
        private Mock<IEventSubscriber> subscriber;
        private TestFixture<MySubject> obj;

        /// <summary>
        /// Sets up before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            subscriber = new Mock<IEventSubscriber>();
            obj = new TestFixture<MySubject>();
            obj.ObjCreated += subscriber.Object.OnObjCreated;
            obj.BaseSetup();
        }

        /// <summary>
        /// Cleans up after each test.
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            obj.BaseTeardown();
        }

        /// <summary>
        /// Verifies that the default mock is returned and passed to the 
        /// test fixture constructor.
        /// </summary>
        [Test]
        public void MockReturnsMockWhosValueIsDependency()
        {
            var mock = obj.Mock<IMyService>();
            Assert.That(obj.Obj.MyService, Is.SameAs(mock.Object));
        }

        /// <summary>
        /// Verifies that the object created notification is raised.
        /// </summary>
        [Test]
        public void ObjGetRaisesOnObjCreated()
        {
            var subject = obj.Obj;
            subscriber.Verify(x => 
                x.OnObjCreated(subject));
        }

        /// <summary>
        /// Verifies that the default mock is set when passed to the test 
        /// fixture method.
        /// </summary>
        [Test]
        public void UseMakesSubjectGetValueAsDependency()
        {
            var mock = new Mock<IMyService>();
            obj.Use(mock.Object);
            Assert.That(obj.Obj.MyService, Is.SameAs(mock.Object));
        }

        /// <summary>
        /// Helper interface for verifying event behavior.
        /// </summary>
        public interface IEventSubscriber
        {
            void OnObjCreated(object value);
        }

        /// <summary>
        /// Helper interface for verifying test fixture behavior.
        /// </summary>
        public interface IMyService
        {
        }

        /// <summary>
        /// Helper class for verifying test fixture behavior.
        /// </summary>
        public class MySubject
        {
            /// <summary>
            /// Initializes a new instance of the <see 
            /// cref="MySubject" /> class.
            /// </summary>
            public MySubject(IMyService myService)
            {
                MyService = myService;
            }

            /// <summary>
            /// Gets the instance dependency.
            /// </summary>
            public IMyService MyService
            {
                get; 
                private set;
            }
        }
    }
}