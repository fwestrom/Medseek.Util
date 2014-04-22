namespace Medseek.Util.MicroServices.Lookup
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Describes a micro-service lookup record.
    /// </summary>
    [DataContract(Name = "record", Namespace = "")]
    public class LookupRecord : LookupMessage
    {
        /// <summary>
        /// Gets or sets the identifier associated with the lookup record.
        /// </summary>
        [DataMember(Name = "address")]
        public string Address
        {
            get;
            set;
        }
    }
}