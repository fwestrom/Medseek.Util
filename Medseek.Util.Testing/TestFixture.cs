namespace Medseek.Util.Testing
{
    using System;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Provides common functionality needed in test fixtures.
    /// </summary>
    /// <typeparam name="TSubject">
    /// The type of the test fixture subject. 
    /// </typeparam>
    public class TestFixture<TSubject> where TSubject : class
    {
        private AutoMockContainer autoMockContainer;
        private TSubject subjectObj;

        /// <summary>
        /// Raised when the test subject object is created.
        /// </summary>
        public event Action<TSubject> ObjCreated;

        /// <summary>
        /// Gets the object that is the subject of the test.
        /// </summary>
        public TSubject Obj
        {
            get
            {
                if (subjectObj == null)
                {
                    subjectObj = autoMockContainer.Create<TSubject>();

                    var objCreated = ObjCreated;
                    if (objCreated != null)
                        objCreated(subjectObj);
                }

                return subjectObj;
            }
        }

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void BaseSetup()
        {
            var mockRepository = new MockRepository(MockBehavior.Default);
            autoMockContainer = new AutoMockContainer(mockRepository);
            subjectObj = null;
        }

        /// <summary>
        /// Cleans up after each test is executed.
        /// </summary>
        [TearDown]
        public void BaseTeardown()
        {
            autoMockContainer = null;
            subjectObj = null;
        }

        /// <summary>
        /// Gets the default mock for the specified type.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of service provided by the mock.
        /// </typeparam>
        /// <returns>
        /// The service mock.
        /// </returns>
        public Mock<TService> Mock<TService>() where TService : class
        {
            var mock = autoMockContainer.GetMock<TService>();
            return mock;
        }

        /// <summary>
        /// Sets the default mock for the specified type.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of service for which the object is to be used by default.
        /// </typeparam>
        /// <param name="serviceObj">
        /// An object that provides an implementation of the service.
        /// </param>
        /// <returns>
        /// The same object that was passed in as <see cref="serviceObj" />.
        /// </returns>
        public TService Use<TService>(TService serviceObj)
        {
            autoMockContainer.Register(serviceObj);
            return serviceObj;
        }
    }
}