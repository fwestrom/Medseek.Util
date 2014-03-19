namespace Medseek.Util.Ioc.Castle
{
    using System;
    using System.Linq;
    using System.Reflection;
    using global::Castle.Core.Internal;
    using global::Castle.Facilities.TypedFactory;
    using global::Castle.MicroKernel;
    using global::Castle.Windsor;
    using Medseek.Util.Ioc;
    using Medseek.Util.MicroServices;

    /// <summary>
    /// A typed factory component selector that chooses micro-service 
    /// implementation components to resolve from a factory.
    /// </summary>
    [Register(typeof(MicroServiceComponentSelector), Lifestyle = Lifestyle.Transient)]
    public class MicroServiceComponentSelector : DefaultTypedFactoryComponentSelector
    {
        private readonly IWindsorContainer container;

        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="MicroServiceComponentSelector" /> class.
        /// </summary>
        public MicroServiceComponentSelector(IWindsorContainer container)
            : base(false)
        {
            if (container == null) 
                throw new ArgumentNullException("container");
            
            this.container = container;
        }

        /// <summary>
        /// Selects name of the component to resolve.
        /// </summary>
        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            var handler = GetHandler(method);
            var result = handler != null ? handler.ComponentModel.Name : null;
            return result;
        }

        /// <summary>
        /// Selects type of the component to resolve.
        /// </summary>
        protected override Type GetComponentType(MethodInfo method, object[] arguments)
        {
            var handler = GetHandler(method);
            var result = handler != null ? handler.ComponentModel.Services.First() : null;
            return result;
        }

        private IHandler GetHandler(MethodInfo method)
        {
            var assignableHandlers = container.Kernel.GetAssignableHandlers(method.ReturnType);
            var handlers = assignableHandlers
                .SelectMany(handler => handler.ComponentModel.Implementation
                    .GetAttributes<RegisterMicroServiceAttribute>()
                    .SelectMany(attribute => attribute.MicroServiceContracts
                        .Select(contract => new { attribute, contract, handler })))
                .OrderBy(x => x.handler.Supports(x.contract) ? 0 : 1)
                .Select(x => x.handler);
            return handlers.First();
        }
    }
}