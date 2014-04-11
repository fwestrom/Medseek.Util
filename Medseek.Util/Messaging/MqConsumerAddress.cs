namespace Medseek.Util.Messaging
{
    /// <summary>
    /// Describes an address that corresponds to a messaging system entity, 
    /// with additional information of interest to message consumers.
    /// </summary>
    public class MqConsumerAddress : MqAddress
    {
        private readonly object sourceKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqConsumerAddress" /> 
        /// class.
        /// </summary>
        public MqConsumerAddress(string value, object sourceKey)
            : base(value)
        {
            this.sourceKey = sourceKey;
        }

        /// <summary>
        /// Gets an object that can be used as a unique identifier for the 
        /// queue (or otherwise) that is used as the source of messages 
        /// received by a message consumer (see <see cref="IMqConsumer"/>).
        /// </summary>
        public object SourceKey
        {
            get
            {
                return sourceKey;
            }
        }
    }
}