namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Medseek.Util.Ioc;
    using Medseek.Util.Messaging;

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
        /// Gets the message properties.
        /// </summary>
        public IMessageProperties Properties
        {
            get
            {
                return GetCurrent().Properties;
            }
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