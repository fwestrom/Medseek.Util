namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Linq;
    using global::Castle.MicroKernel;
    using global::Castle.Windsor;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="WindsorDependencyResolver" /> class.
    /// </summary>
    [TestFixture]
    public class WindsorDependencyResolverTests : TestFixture<WindsorDependencyResolver>
    {
        private static readonly Random Rand = new Random();
        private Mock<IWindsorContainer> container;
        private Mock<IKernel> kernel;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            kernel = Mock<IKernel>();
            container = Mock<IWindsorContainer>();
            container.Setup(x =>
                x.Kernel)
                .Returns(kernel.Object);
        }

        /// <summary>
        /// Verifies that the container is not disposed when the dependency 
        /// resolver is disposed.
        /// </summary>
        [Test]
        public void BeginScopeDisposeScopeDoesNotDisposeContainer()
        {
            var scope = Obj.BeginScope();
            scope.Dispose();

            container.Verify(x =>
                x.Dispose(),
                Times.Never());
        }

        /// <summary>
        /// Verifies that an exception is thrown if the dependency resolver was
        /// previously disposed.
        /// </summary>
        [Test]
        public void BeginScopeThrowsIfAlreadyDisposed()
        {
            Obj.Dispose();
            TestDelegate action = () => Obj.BeginScope();
            Assert.That(action, Throws.TypeOf<ObjectDisposedException>());
        }

        /// <summary>
        /// Verifies that the constructor requires the container to be 
        /// provided.
        /// </summary>
        [Test]
        public void ConstructorRequiresContainerDependency()
        {
            TestDelegate action = () => new WindsorDependencyResolver(null);
            Assert.That(action, Throws.TypeOf<ArgumentNullException>());
        }

        /// <summary>
        /// Verifies that the container is not disposed when the dependency 
        /// resolver is disposed.
        /// </summary>
        [Test]
        public void DisposeDoesNotDisposeContainer()
        {
            Obj.Dispose();

            container.Verify(x => 
                x.Dispose(),
                Times.Never());
        }

        /// <summary>
        /// Verifies that the service is returned from the container.
        /// </summary>
        [Test]
        public void GetServiceResolvesFromContainer()
        {
            var myService = new Mock<IMyService>().Object;
            container.Setup(x =>
                x.Resolve(typeof(IMyService)))
                .Returns(myService);
            kernel.Setup(x => 
                x.HasComponent(typeof(IMyService)))
                .Returns(true);

            var result = Obj.GetService(typeof(IMyService));

            Assert.That(result, Is.SameAs(myService));
        }
        
        /// <summary>
        /// Verifies that the correct value is returned when the container can 
        /// not resolve a dependency.
        /// </summary>
        [Test]
        public void GetServiceReturnsNullIfNotRegistered()
        {
            kernel.Setup(x =>
                x.HasComponent(typeof(IMyService)))
                .Returns(false);

            var result = Obj.GetService(typeof(IMyService));

            Assert.That(result, Is.Null);
            container.Verify(x =>
                x.Resolve(typeof(IMyService)),
                Times.Never());
        }

        /// <summary>
        /// Verifies that an exception is thrown if the dependency resolver was
        /// previously disposed.
        /// </summary>
        [Test]
        public void GetServiceThrowsIfAlreadyDisposed()
        {
            Obj.Dispose();
            TestDelegate action = () => Obj.GetService(typeof(IMyService));
            Assert.That(action, Throws.TypeOf<ObjectDisposedException>());
        }

        /// <summary>
        /// Verifies that the services are returned from the container.
        /// </summary>
        [Test]
        public void GetServicesResolvesFromContainer()
        {
            var myServices = Enumerable.Range(0, Rand.Next(1, 100)).Select(x => new Mock<IMyService>().Object).ToArray();
            container.Setup(x =>
                x.ResolveAll(typeof(IMyService)))
                .Returns(myServices);
            kernel.Setup(x =>
                x.HasComponent(typeof(IMyService)))
                .Returns(true);

            var result = Obj.GetServices(typeof(IMyService));

            Assert.That(result, Is.EquivalentTo(myServices));
        }

        /// <summary>
        /// Verifies that the correct value is returned when the container can 
        /// not resolve a dependency.
        /// </summary>
        [Test]
        public void GetServicesReturnsEmptyEnumerableIfNotRegistered()
        {
            kernel.Setup(x =>
                x.HasComponent(typeof(IMyService)))
                .Returns(false);

            var result = Obj.GetServices(typeof(IMyService));

            Assert.That(result, Is.Empty.And.Not.Null);
            container.Verify(x =>
                x.ResolveAll(typeof(IMyService)),
                Times.Never());
        }

        /// <summary>
        /// Verifies that an exception is thrown if the dependency resolver was
        /// previously disposed.
        /// </summary>
        [Test]
        public void GetServicesThrowsIfAlreadyDisposed()
        {
            Obj.Dispose();
            TestDelegate action = () => Obj.GetServices(typeof(IMyService));
            Assert.That(action, Throws.TypeOf<ObjectDisposedException>());
        }

        /// <summary>
        /// Helper interface for mocking container usages.
        /// </summary>
        public interface IMyService
        {
        }
    }
}