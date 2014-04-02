namespace Medseek.Util.Ioc
{
    using System;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="IocBootstrapper"/> class.
    /// </summary>
    [TestFixture]
    public sealed class IocBootstrapperTests : TestFixture<IocBootstrapper>
    {
        private Mock<IIocContainer> container;
        private Mock<IIocPlugin> plugin;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            container = new Mock<IIocContainer>();
            plugin = Mock<IIocPlugin>();
            plugin.Setup(x => 
                x.NewContainer())
                .Returns(container.Object);

            // ReSharper disable once UnusedVariable
            var obj = Obj;
        }

        /// <summary>
        /// Verifies that the container is disposed.
        /// </summary>
        [Test]
        public void DisposeDisposesTheContainer()
        {
            container.Setup(x => 
                x.Dispose())
                .Verifiable();
            
            Obj.Dispose();

            container.Verify();
        }

        /// <summary>
        /// Verifies that attempting to get the container after installing 
        /// plugins causes the container to be resolved.
        /// </summary>
        [Test]
        public void GetContainerReturnsContainerAfterInstall()
        {
            var installables = new[] { new Mock<IInstallable>().Object };
            Obj.Install(installables);

            var result = Obj.GetContainer();

            Assert.That(result, Is.SameAs(container.Object));
        }

        /// <summary>
        /// Verifies that an exception is thrown if the bootstrapper has been 
        /// disposed.
        /// </summary>
        [Test]
        public void GetContainerThrowsIfDisposed()
        {
            Obj.Dispose();

            TestDelegate action = () => Obj.GetContainer();
            Assert.That(action, Throws.InstanceOf<ObjectDisposedException>());
        }

        /// <summary>
        /// Verifies that attempting to get the container without installing 
        /// any installable plugins causes an exception to be thrown.
        /// </summary>
        [Test]
        public void GetContainerThrowsIfNotYetInstalled()
        {
            TestDelegate action = () => Obj.GetContainer();
            Assert.That(action, Throws.InvalidOperationException);
        }

        /// <summary>
        /// Verifies that installing plugins causes the container to be invoked
        /// to perform the installation.
        /// </summary>
        [Test]
        public void InstallInvokesContainerInstall()
        {
            var installables = new[] { new Mock<IInstallable>().Object };
            container.Setup(x => 
                x.Install(installables))
                .Verifiable();

            Obj.Install(installables);

            container.Verify();
        }

        /// <summary>
        /// Verifies that attempting to install multiple times causes an 
        /// exception to be thrown.
        /// </summary>
        [Test]
        public void InstallThrowsIfAlreadyInstalled()
        {
            Obj.Install(new Mock<IInstallable>().Object);

            TestDelegate action = () => Obj.Install(new Mock<IInstallable>().Object);
            Assert.That(action, Throws.InvalidOperationException);
        }

        /// <summary>
        /// Verifies that an exception is thrown if the bootstrapper has been 
        /// disposed.
        /// </summary>
        [Test]
        public void InstallThrowsIfDisposed()
        {
            Obj.Dispose();

            TestDelegate action = () => Obj.Install(new[] { new Mock<IInstallable>().Object });
            Assert.That(action, Throws.InstanceOf<ObjectDisposedException>());
        }

        /// <summary>
        /// Verifies that the plugin property returns the plugin passed to the 
        /// constructor.
        /// </summary>
        [Test]
        public void PluginGetReturnsCtorPlugin()
        {
            var result = Obj.Plugin;
            Assert.That(result, Is.SameAs(plugin.Object));
        }

        /// <summary>
        /// Verifies that the component registration events are raised.
        /// </summary>
        [Test]
        public void RegisteringComponentIsRaisedWhenContainerRaisesRegisteringComponent()
        {
            var subscriber = new Mock<IEventSubscriber>();
            Obj.RegisteringComponent += subscriber.Object.OnRegisteringComponent;

            var e = new RegisteringComponentEventArgs();
            subscriber.Setup(x => 
                x.OnRegisteringComponent(Obj, e))
                .Verifiable();

            container.Raise(x => 
                x.RegisteringComponent += null, e);

            subscriber.Verify();
        }

        /// <summary>
        /// Helper interface used for verifying event notification behavior.
        /// </summary>
        public interface IEventSubscriber
        {
            void OnRegisteringComponent(object sender, RegisteringComponentEventArgs e);
        }
    }
}