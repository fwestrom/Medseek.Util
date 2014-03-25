namespace Medseek.Util.MicroServices
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class MicroServiceInstance
    {
        private readonly InvokeHelper invokeHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceInstance" 
        /// /> class.
        /// </summary>
        public MicroServiceInstance(MicroServiceDescriptor descriptor, object instance)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");
            if (instance == null)
                throw new ArgumentNullException("instance");

            Descriptor = descriptor;
            Instance = instance;

            invokeHelper = InvokeHelper.Create(instance, descriptor.DefaultMethod);
        }

        public MicroServiceDescriptor Descriptor
        {
            get; 
            private set;
        }

        public object Instance
        {
            get; 
            private set;
        }

        public object Invoke(MethodInfo method, object parameter)
        {
            var result = method != Descriptor.DefaultMethod
                ? method.Invoke(Instance, new[] { parameter }) 
                : invokeHelper.Invoke(parameter);
            return result;
        }

        private abstract class InvokeHelper
        {
            internal static InvokeHelper Create(object target, MethodInfo method)
            {
                var parameterType = method.GetParameters().Single().ParameterType;
                var helperType = method.ReturnType == typeof(void)
                                     ? typeof(InvokeHelperVoid<>).MakeGenericType(parameterType)
                                     : typeof(InvokeHelper<,>).MakeGenericType(parameterType, method.ReturnType);
                var constructor = helperType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single();
                var helper = (InvokeHelper)constructor.Invoke(new[] { target, method });
                return helper;
            }

            internal abstract object Invoke(object parameter);
        }

        private abstract class InvokeHelperBase<TInvoke> : InvokeHelper where TInvoke : class
        {
            protected readonly TInvoke invoke;

            protected InvokeHelperBase(object target, MethodInfo method)
            {
                if (method == null)
                    throw new ArgumentNullException("method");

                invoke = (TInvoke)(object)method.CreateDelegate(typeof(TInvoke), target);
                if (invoke == null)
                    throw new ArgumentException("Unable to create delegate for invoking method.", "method");
            }
        }

        private class InvokeHelperVoid<TParameter> : InvokeHelperBase<Action<TParameter>>
        {
            internal InvokeHelperVoid(object target, MethodInfo method)
                : base(target, method)
            {
            }

            internal override object Invoke(object parameter)
            {
                var parameterT = (TParameter)parameter;
                invoke(parameterT);
                return null;
            }
        }

        private class InvokeHelper<TParameter, TReturn> : InvokeHelperBase<Func<TParameter, TReturn>>
        {
            internal InvokeHelper(object target, MethodInfo method)
                : base(target, method)
            {
            }

            internal override object Invoke(object parameter)
            {
                var parameterT = (TParameter)parameter;
                var result = invoke(parameterT);
                return result;
            }
        }
    }
}