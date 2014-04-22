namespace Medseek.Util.MicroServices.Lookup
{
    /// <summary>
    /// Identifies the type of action result that occurred to generate an 
    /// entry in the micro-service lookup cache.
    /// </summary>
    public enum LookupResultType
    {
        Unknown,
        Reply,
        Returned,
        Update,
    }
}