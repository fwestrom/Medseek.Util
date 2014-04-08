namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Reflection;
    using global::Castle.DynamicProxy;
    using Medseek.Util.Logging;
    using Medseek.Util.MicroServices;

    /// <summary>
    /// Provides interceptor-oriented implementation of micro-service proxies
    /// so that the micro-services can be invoked conveniently.
    /// </summary>
    [Register(Name = UtilComponents.MicroServiceProxyInterceptor, Lifestyle = Lifestyle.Transient)]
    public class MicroServiceProxyInterceptor : IInterceptor
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IMicroServiceDispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceProxyInterceptor"/> class.
        /// </summary>
        public MicroServiceProxyInterceptor(IMicroServiceDispatcher dispatcher)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");

            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// Intercepts micro-service operations and sends appropriate messages.
        /// </summary>
        public void Intercept(IInvocation invocation)
        {
            var attribute = invocation.Method.GetCustomAttribute<MicroServiceBindingAttribute>();
            if (attribute == null)
                throw new NotSupportedException("Operations must be marked with [MicroServiceBinding] to support proxy functionality.");

            var binding = attribute.ToBinding<MicroServiceBinding>(invocation.Method, invocation.Method.DeclaringType);
            if (!binding.IsOneWay)
                throw new NotSupportedException("Only one-way binding is supported.");

            Log.DebugFormat("Invoking remote micro-service; Address = {0}, Method = {1}, Arguments = {2}.", binding.Address, binding.Method, string.Join(", ", invocation.Arguments));
            var invoker = dispatcher.RemoteMicroServiceInvoker;
            invoker.Send(binding, invocation.Arguments);
        }
    }
}