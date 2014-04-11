namespace Medseek.Util.Ioc
{
    /// <summary>
    /// Interface for types that provide the pluggable functionality for 
    /// integrating with an inversion of control / dependency injection 
    /// container.
    /// </summary>
    public interface IIocPlugin : IInstallable
    {
        /// <summary>
        /// Creates a new instance of the inversion of control container 
        /// supported by the plugin.
        /// </summary>
        /// <returns>
        /// A new instance of the container.
        /// </returns>
        IIocContainer NewContainer();
    }
}