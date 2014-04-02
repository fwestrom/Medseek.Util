

namespace Medseek.Util.Ioc.Castle
{
    using NUnit.Framework;

    [TestFixture]
    public class WindsorIocContainerTests
    {
        [Test]
        public void InstallTwiceDoesntThrowOnAddFacility()
        {
            var plugin = new CastlePlugin()
            {
                AddStartableFacility = true,
                AddTypedFactoryFacility = true,
                AddWcfFacility = true,
                InstallFromConfiguration = false,
                RegisterIWindsorContainer = true
            };
            var container = new WindsorIocContainer(plugin);
            container.Install(new IInstallable[0]);

            TestDelegate action = () => container.Install(new IInstallable[0]);

            Assert.That(action, Throws.Nothing);
        }
    }
}
