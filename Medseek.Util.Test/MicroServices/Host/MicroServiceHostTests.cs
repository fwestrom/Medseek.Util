namespace Medseek.Util.MicroServices.Host
{
    using System.Linq;
    using System.Reflection;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="MicroServiceHost"/> class.
    /// </summary>
    [TestFixture]
    public sealed class MicroServiceHostTests : TestFixture<MicroServiceHost>
    {
        private static readonly Mock<IIocPlugin> IocPlugin = new Mock<IIocPlugin>();
        private static readonly Mock<ILoggingPlugin> LoggingPlugin = new Mock<ILoggingPlugin>();
        private static readonly IInstallable[] StartInstallInstallables = { UtilComponents.Framework, IocPlugin.Object, LoggingPlugin.Object };
        private Mock<IIocContainer> container;
        private Mock<ILogManager> logManager;
        private Mock<ILog> log;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Use(IocPlugin.Object);
            Use(LoggingPlugin.Object);
            Use(new Assembly[0]);
            container = new Mock<IIocContainer>();
            log = new Mock<ILog>();
            logManager = new Mock<ILogManager>();

            IocPlugin.Setup(x => 
                x.NewContainer())
                .Returns(container.Object);
            LoggingPlugin.Setup(x => 
                x.GetLogManager())
                .Returns(logManager.Object);
            logManager.Setup(x => 
                x.GetLogger(typeof(MicroServiceHost)))
                .Returns(log.Object);
        }

        /// <summary>
        /// Verifies that the container is disposed.
        /// </summary>
        [Test]
        public void DisposeInvokesContainerDispose()
        {
            container.Setup(x => 
                x.Dispose())
                .Verifiable();

            Obj.Dispose();
            
            container.Verify();
        }

        /// <summary>
        /// Verifies that the inversion of control plugin is installed to the 
        /// container.
        /// </summary>
        [TestCaseSource("StartInstallInstallables")]
        public void StartInvokesContainerInstallWithInstallable(IInstallable expectedInstallable)
        {
            container.Setup(x =>
                x.Install(It.Is<IInstallable[]>(a => a.Contains(expectedInstallable))))
                .Verifiable();

            Obj.Start();

            container.Verify();
        }
    }
}