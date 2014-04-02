

namespace Medseek.Util.Ioc.Castle
{

    using global::Castle.Facilities.Startable;
    using global::Castle.Facilities.TypedFactory;
    using global::Castle.Facilities.WcfIntegration;
    using global::Castle.MicroKernel;

    using Medseek.Util.Testing;

    using NUnit.Framework;
    [TestFixture]
    public class WindsorIocContainerTests : TestFixture<WindsorIocContainer>
    {
        
        [SetUp]
        public void SetUp()
        {
            Use(new CastlePlugin
            {
                AddStartableFacility = true,
                AddTypedFactoryFacility = true,
                AddWcfFacility = true,
                InstallFromConfiguration = false,
                RegisterIWindsorContainer = true
            });
        }

        [Test]
        public void InstallTwiceDoesntThrowOnAddFacility()
        {
            
            Obj.Install(new IInstallable[0]);

            TestDelegate action = () => Obj.Install(new IInstallable[0]);

            Assert.That(action, Throws.Nothing);
        }

        [Test]
        public void InstallShouldInstallFacilities()
        {
            Obj.Install(new IInstallable[0]);

            var facilities = Obj.Kernel.GetFacilities();

            Assert.That(facilities, Has.Some.Matches<IFacility>(m => m is StartableFacility));
            Assert.That(facilities, Has.Some.Matches<IFacility>(m => m is TypedFactoryFacility));
            Assert.That(facilities, Has.Some.Matches<IFacility>(m => m is WcfFacility));
        }
    }
}
