namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Reflection;
    using global::Castle.DynamicProxy;
    using Medseek.Util.MicroServices;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="MicroServiceProxyInterceptor"/> class.
    /// </summary>
    [TestFixture]
    public sealed class MicroServiceProxyInterceptorTests : TestFixture<MicroServiceProxyInterceptor>
    {
        private Mock<IMicroServiceDispatcher> dispatcher;
        private Mock<IRemoteMicroServiceInvoker> invoker;
        private MicroServiceBinding helperDoIt1Binding;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            dispatcher = Mock<IMicroServiceDispatcher>();
            invoker = new Mock<IRemoteMicroServiceInvoker>();

            var helperDoIt1Method = typeof(IHelperService).GetMethod("DoIt1");
            helperDoIt1Binding = helperDoIt1Method.GetCustomAttribute<MicroServiceBindingAttribute>().ToBinding<MicroServiceBinding>(helperDoIt1Method, helperDoIt1Method.DeclaringType);

            dispatcher.Setup(x => 
                x.RemoteMicroServiceInvoker)
                .Returns(invoker.Object);
        }

        /// <summary>
        /// Verifies that a one-way method causes the remote micro-service 
        /// invoker to be called to perform the operation.
        /// </summary>
        [Test]
        public void InterceptOneWayMethodUsesInvoker()
        {
            var parameters = new[] { new object() };
            var invocation = NewInvocation("DoIt1", parameters);

            invoker.Setup(x => 
                x.Send(
                    It.Is<MicroServiceBinding>(a => 
                        a.Address.Value == helperDoIt1Binding.Address.Value &&
                        a.Method == helperDoIt1Binding.Method && 
                        a.Service == helperDoIt1Binding.Service &&
                        a.IsOneWay == helperDoIt1Binding.IsOneWay),
                    parameters))
                .Verifiable();

            Obj.Intercept(invocation);

            invoker.Verify();
        }

        /// <summary>
        /// Verifies that an exception is thrown for two-way methods.
        /// </summary>
        [Test]
        public void InterceptTwoWayMethodThrows()
        {
            var invocation = NewInvocation("DoIt2", new object());
            TestDelegate action = () => Obj.Intercept(invocation);
            Assert.That(action, Throws.InstanceOf<NotSupportedException>());
        }

        /// <summary>
        /// Verifies that an exception is thrown for unmarked methods.
        /// </summary>
        [Test]
        public void InterceptUnmarkedMethodThrows()
        {
            var invocation = NewInvocation("Unmarked");
            TestDelegate action = () => Obj.Intercept(invocation);
            Assert.That(action, Throws.InstanceOf<NotSupportedException>());
        }

        private static IInvocation NewInvocation(string methodName, params object[] arguments)
        {
            var method = typeof(IHelperService).GetMethod(methodName);
            var mock = new Mock<IInvocation>();
            mock.Setup(x => 
                x.Arguments)
                .Returns(arguments);
            mock.Setup(x => 
                x.Method)
                .Returns(method);
            return mock.Object;
        }

        /// <summary>
        /// Helper interface for verifying interceptor behavior.
        /// </summary>
        public interface IHelperService
        {
            [MicroServiceBinding("medseek-api", "IHelperService.DoIt1", "IHelperService", IsOneWay = true)]
            void DoIt1(object argument);

            [MicroServiceBinding("medseek-api", "IHelperService.DoIt2", "IHelperService", IsOneWay = false)]
            object DoIt2(object argument);

            void Unmarked();
        }
    }
}