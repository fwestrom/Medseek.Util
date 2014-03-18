namespace Medseek.Util.Messaging
{
    /// <summary>
    /// Describes an address that corresponds to a messaging system entity to 
    /// which messages can be sent or from which messages can be received.
    /// </summary>
    public class MqAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MqAddress"/> class.
        /// </summary>
        public MqAddress(string value = null)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the string value describing to the address.
        /// </summary>
        public string Value
        { 
            get;
            set;
        }

        /// <summary>
        /// Gets a string representation of the address.
        /// </summary>
        /// <returns>
        /// A string representing the address.
        /// </returns>
        public override string ToString()
        {
            return Value;
        }
    }
}