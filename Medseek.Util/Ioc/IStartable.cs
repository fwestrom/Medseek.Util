namespace Medseek.Util.Ioc
{
    /// <summary>
    /// Interface for types that should be started when the components have 
    /// been installed in the container and setup is complete.
    /// </summary>
    public interface IStartable
    {
        /// <summary>
        /// Invoked by the container to start the component.
        /// </summary>
        void Start();
    }
}