namespace Medseek.Util.MicroServices.Lookup
{
    using System;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Describes an entry in the micro-service lookup cache.
    /// </summary>
    public class LookupEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookupEntry"/> class.
        /// </summary>
        public LookupEntry(MqAddress address, string id, LookupResultType type)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (type == LookupResultType.Unknown)
                throw new ArgumentException("Unknown lookup result type is not allowed.");

            Address = address;
            Id = id;
            Type = type;
        }

        internal LookupEntry()
        {
        }

        /// <summary>
        /// Gets the resolved address associated with the entry.
        /// </summary>
        public MqAddress Address
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the unresolved identifier associated with the entry.
        /// </summary>
        public string Id
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value identifying the type of action result that 
        /// occurred to generate entry.
        /// </summary>
        public LookupResultType Type
        {
            get;
            internal set;
        }
    }
}