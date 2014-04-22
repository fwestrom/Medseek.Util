namespace Medseek.Util.MicroServices.Lookup
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Describes a micro-service lookup query.
    /// </summary>
    [DataContract(Name = "query", Namespace = "")]
    public class LookupQuery : LookupMessage
    {
    }
}