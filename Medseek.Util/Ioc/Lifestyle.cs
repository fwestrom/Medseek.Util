namespace Medseek.Util.Ioc
{
    /// <summary>
    /// Identifies the lifestyle of a component registered in the injection 
    /// container.
    /// </summary>
    public enum Lifestyle
    {
        /// <summary>
        /// There is one shared instance of the component.
        /// </summary>
        Singleton,

        /// <summary>
        /// A new instance of the component is created each time it is 
        /// resolved.
        /// </summary>
        Transient,

        /// <summary>
        /// A new instance is created for use within the scope of a single web 
        /// request.
        /// </summary>
        WebRequest,
    }
}