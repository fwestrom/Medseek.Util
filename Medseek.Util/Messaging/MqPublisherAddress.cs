namespace Medseek.Util.Messaging
{
    /// <summary>
    /// Describes an address that corresponds to a messaging system entity, 
    /// with additional information of interest to message publishers.
    /// </summary>
    public class MqPublisherAddress : MqAddress
    {
        private readonly string routingKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqPublisherAddress" /> 
        /// class.
        /// </summary>
        public MqPublisherAddress(string value, string routingKey)
            : base(value)
        {
            this.routingKey = routingKey;
        }

        /// <summary>
        /// Gets the routing key that is used as part of the destination 
        /// description for for publishing messages (see <see 
        /// cref="IMqPublisher"/>).
        /// </summary>
        public string RoutingKey
        {
            get
            {
                return routingKey;
            }
        }
    }
}