namespace Medseek.Util.Plugin.WebApi
{
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Dependencies;
    using Medseek.Util.Ioc;
    using Medseek.Util.Ioc.Castle;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="IocIntegrationHttpModule" /> class.
    /// </summary>
    public class IocIntegrationHttpModuleTests
    {
        private Mock<IIocContainer> container;
        private IocIntegrationHttpModule obj;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            WindsorBootstrapper.InstallFromConfiguration = false;

            container = new Mock<IIocContainer>();
            obj = new IocIntegrationHttpModule(() => container.Object);
        }

        /// <summary>
        /// Verifies that the container is not disposed when the HTTP module is
        /// disposed.
        /// </summary>
        [Test]
        public void DisposeDoesNotDisposeContainer()
        {
            obj.Dispose();

            container.Verify(x =>
                x.Dispose(),
                Times.Never());
        }

        /// <summary>
        /// Verifies that the correct dependency resolver is set on the global 
        /// configuration.
        /// </summary>
        [Test]
        public void InitSetsGlobalConfigurationDependencyReolver()
        {
            var dependencyResolver = new Mock<IDependencyResolver>();
            container.Setup(x =>
                x.Resolve(typeof(IDependencyResolver)))
                .Returns(dependencyResolver.Object);

            obj.Init(new HttpApplication());
            
            Assert.That(GlobalConfiguration.Configuration.DependencyResolver, Is.SameAs(dependencyResolver.Object));
        }
    }
}