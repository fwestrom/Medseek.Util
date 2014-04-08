namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Medseek.Util.Ioc;
    using Medseek.Util.Messaging;
    using Medseek.Util.Objects;

    /// <summary>
    /// Provides a message context that provides access to thread-local 
    /// context information.
    /// </summary>
    [Register(typeof(IMessageContextAccess), typeof(IMessageContext), Lifestyle = Lifestyle.Singleton)]
    public class ThreadLocalMessageContext : IMessageContextAccess, IMessageContext
    {
        private readonly ThreadLocal<Stack<IMessageContext>> contextStack = new ThreadLocal<Stack<IMessageContext>>(() => new Stack<IMessageContext>());

        /// <summary>
        /// Gets the current message context.
        /// </summary>
        public IMessageContext Current
        {
            get
            {
                return GetCurrent(false);
            }
        }

        /// <summary>
        /// Gets the message properties.
        /// </summary>
        public IMessageProperties Properties
        {
            get
            {
                return GetCurrent().Properties;
            }
        }

        /// <summary>
        /// Pops the current message context from the stack.
        /// </summary>
        public void Pop()
        {
            var stack = contextStack.Value;
            stack.Pop();
        }

        /// <summary>
        /// Pushes a new message context onto the stack, making it the new 
        /// current context.
        /// </summary>
        /// <param name="context">
        /// The message context to push.
        /// </param>
        public void Push(IMessageContext context)
        {
            var stack = contextStack.Value;
            stack.Push(context);
        }

        /// <summary>
        /// Pushes a new current message context onto the stack, which will 
        /// be popped from the stack upon disposing the object returned by the 
        /// method.
        /// </summary>
        /// <param name="messageContext">
        /// The new current message context, or null to use a copy of the 
        /// current message context.
        /// </param>
        /// <returns>
        /// An object that will cause the message context to be popped when it 
        /// is disposed.
        /// </returns>
        public IDisposable Enter(IMessageContext messageContext)
        {
            var disposable = new Disposable();
            var stack = contextStack.Value;
            disposable.Disposing += (sender, e) => stack.Pop();

            var value = messageContext ?? (IMessageContext)Clone();
            stack.Push(value);

            return disposable;
        }

        /// <summary>
        /// Creates an independent copy of the message context.
        /// </summary>
        /// <returns>
        /// The new message context that was created from the original.
        /// </returns>
        public object Clone()
        {
            return GetCurrent().Clone();
        }

        // ReSharper disable once UnusedParameter.Local
        private IMessageContext GetCurrent(bool throwIfNone = true)
        {
            var stack = contextStack.Value;
            var result = stack.Count > 0 ? stack.Peek() : null;
            if (result == null && throwIfNone)
                throw new InvalidOperationException("No current message context exists.");
            return result;
        }
    }
}