namespace Medseek.Util.Ioc
{
    /// <summary>
    /// Interface for types that be installed into an inversion of control 
    /// container.
    /// </summary>
    public interface IInstallable
    {
        /// <summary>
        /// Installs the installable type into an inversion of control 
        /// container.
        /// </summary>
        /// <param name="container">
        /// The inversion of control container.
        /// </param>
        void Installing(IIocContainer container);
    }
}