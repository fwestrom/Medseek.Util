namespace Medseek.Util.MicroServices
{
    using System;

    /// <summary>
    /// Interface for types that provide access to the message context and 
    /// management of the message context.
    /// </summary>
    public interface IMessageContextAccess
    {
        /// <summary>
        /// Gets the current message context.
        /// </summary>
        IMessageContext Current
        {
            get;
        }

        /// <summary>
        /// Pops the current message context from the stack.
        /// </summary>
        void Pop();

        /// <summary>
        /// Pushes a new message context onto the stack, making it the new 
        /// current context.
        /// </summary>
        /// <param name="context">
        /// The message context to push.
        /// </param>
        void Push(IMessageContext context);

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
        IDisposable Enter(IMessageContext messageContext = null);
    }
}