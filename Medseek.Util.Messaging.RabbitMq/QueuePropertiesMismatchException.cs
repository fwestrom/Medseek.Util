namespace Medseek.Util.Messaging.RabbitMq
{
    using System;

    public class QueuePropertiesMismatchException : Exception
    {
        private const string DefaultErrorMessage =
            "Exception caught while attempting to create or connect to a queue. The properties being used to connect to the queue do not match the existing queue. This typically happens because the queue's autodelete flag is set to false but the request to connect to the queue was with auto-delete set to true. Delete the queue and allow it to be re-created to correct this issue.";

        public QueuePropertiesMismatchException() : this(DefaultErrorMessage)
        {
        }

        public QueuePropertiesMismatchException(string message) : base(message)
        {
        }

        public QueuePropertiesMismatchException(Exception inner)
            : base(DefaultErrorMessage, inner)
        {
        }

        public QueuePropertiesMismatchException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
