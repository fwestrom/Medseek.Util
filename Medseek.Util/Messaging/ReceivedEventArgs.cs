namespace Medseek.Util.Messaging
{
    using System;
    using Medseek.Util.MicroServices;

    /// <summary>
    /// Event data describing a message received notification.
    /// </summary>
    public class ReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedEventArgs" /> 
        /// class.
        /// </summary>
        public ReceivedEventArgs(IMessageContext messageContext)
        {
            if (messageContext == null)
                throw new ArgumentNullException("messageContext");

            MessageContext = messageContext;
        }

        internal ReceivedEventArgs()
        {
        }

        /// <summary>
        /// Gets the message context associated with the notification.
        /// </summary>
        public IMessageContext MessageContext
        {
            get;
            internal set;
        }
    }
}