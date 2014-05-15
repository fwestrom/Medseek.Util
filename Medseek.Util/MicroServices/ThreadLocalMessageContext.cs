namespace Medseek.Util.MicroServices
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Medseek.Util.Ioc;
    using Medseek.Util.Messaging;
    using Medseek.Util.Objects;

    /// <summary>
    /// Provides a message context that provides access to thread-local 
    /// context information.
    /// </summary>
    [Register(typeof(IMessageContextAccess), typeof(IMessageContext), Lifestyle = Lifestyle.Singleton)]
    public class ThreadLocalMessageContext : IMessageContextAccess, IMessageContext, ICloneable
    {
        private readonly ThreadLocal<Stack<IMessageContext>> contextStack = new ThreadLocal<Stack<IMessageContext>>(() => new Stack<IMessageContext>());
        private readonly ThreadLocal<Stack<Disposable>> disposerStack = new ThreadLocal<Stack<Disposable>>(() => new Stack<Disposable>());

        /// <summary>
        /// Raised to indicate that the message acknowledgement is desired.
        /// </summary>
        public event EventHandler Acknowledged
        {
            add
            {
                throw new NotSupportedException();
            }
            remove
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the size in bytes of the message body.
        /// </summary>
        public int BodyLength
        {
            get
            {
                return GetCurrent().BodyLength;
            }
        }

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
        public MessageProperties Properties
        {
            get
            {
                return GetCurrent().Properties;
            }
        }

        /// <summary>
        /// Gets the routing key associated with the message context.
        /// </summary>
        public string RoutingKey
        {
            get
            {
                return GetCurrent().RoutingKey;
            }
        }

        /// <summary>
        /// Causes the message acknowledgement to be send for the context.
        /// </summary>
        public void Ack()
        {
            GetCurrent().Ack();
        }

        /// <summary>
        /// Returns a new cloned copy of the message context (see remarks 
        /// about the body data).
        /// </summary>
        /// <remarks>
        /// The data structures that represent the body data are not 
        /// necessarily cloned by this operation.
        /// </remarks>
        public IMessageContext Clone(bool includeAllProperties)
        {
            return GetCurrent().Clone(includeAllProperties);
        }

        /// <summary>
        /// Pushes a new current message context onto the stack, which will 
        /// be popped from the stack upon disposing the object returned by the 
        /// method.
        /// </summary>
        /// <remarks>
        /// The routing key and body oriented data is not necessarily cloned 
        /// by this operation or even present on the resulting message context.
        /// </remarks>
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

            var value = messageContext ?? Clone(false);
            stack.Push(value);
            disposerStack.Value.Push(disposable);

            return disposable;
        }

        /// <summary>
        /// Gets an array containing the message body data.
        /// </summary>
        public byte[] GetBodyArray()
        {
            return GetCurrent().GetBodyArray();
        }

        /// <summary>
        /// Gets a stream that can be used to read the message body data.
        /// </summary>
        public Stream GetBodyStream()
        {
            return GetCurrent().GetBodyStream();
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
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        object ICloneable.Clone()
        {
            return Clone(false);
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