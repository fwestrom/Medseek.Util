namespace Medseek.Util.Ioc.Castle
{
    using System.Web;
    using System.Web.Http;
    using Medseek.Util.Testing;
    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="WindsorIntegrationHttpModule" /> class.
    /// </summary>
    public class WindsorIntegrationHttpModuleTests : TestFixture<WindsorIntegrationHttpModule>
    {
        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            WindsorBootstrapper.InstallFromConfiguration = false;
        }

        /// <summary>
        /// Verifies that the correct dependency resolver is set on the global 
        /// configuration.
        /// </summary>
        [Test]
        public void InitSetsGlobalConfigurationDependencyReolver()
        {
            Obj.Init(new HttpApplication());
            Assert.That(GlobalConfiguration.Configuration.DependencyResolver, Is.InstanceOf<WindsorDependencyResolver>());
        }
    }
}