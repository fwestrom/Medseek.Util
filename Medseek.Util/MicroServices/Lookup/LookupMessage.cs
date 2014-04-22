namespace Medseek.Util.MicroServices.Lookup
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Describes the common properties of a micro-service lookup message.
    /// </summary>
    [DataContract(Name = "message", Namespace = "")]
    public abstract class LookupMessage
    {
        /// <summary>
        /// Gets or sets the identifier associated with the lookup query.
        /// </summary>
        [DataMember(Name = "id")]
        public string Id
        {
            get;
            set;
        }
    }
}