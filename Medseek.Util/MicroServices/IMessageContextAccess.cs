namespace Medseek.Util.MicroServices
{
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
    }
}