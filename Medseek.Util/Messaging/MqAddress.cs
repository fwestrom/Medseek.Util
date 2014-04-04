namespace Medseek.Util.Messaging
{
    using System;

    /// <summary>
    /// Describes an address that corresponds to a messaging system entity to 
    /// which messages can be sent or from which messages can be received.
    /// </summary>
    public class MqAddress : ICloneable
    {
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqAddress"/> class.
        /// </summary>
        public MqAddress(string value = null)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.value = value;
        }

        /// <summary>
        /// Gets the string value describing to the address.
        /// </summary>
        internal string Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            var result = MemberwiseClone();
            return result;
        }

        /// <summary>
        /// Determines whether another object is equal to the address.
        /// </summary>
        public override bool Equals(object obj)
        {
            var other = obj as MqAddress;
            return other != null && Value.Equals(other.Value);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
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