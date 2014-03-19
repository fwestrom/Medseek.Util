namespace Medseek.Util.MicroServices
{
    using Medseek.Util.Messaging;

    /// <summary>
    /// Interface for factories that can provide instances of micro-services.
    /// </summary>
    /// <typeparam name="T">
    /// The service type of the component.
    /// </typeparam>
    public interface IMicroServiceInstanceFactory<T>
    {
        /// <summary>
        /// Gets a component instance.
        /// </summary>
        /// <returns>
        /// The component instance.
        /// </returns>
        T Resolve(IMqConnection connection);

        /// <summary>
        /// Releases a component that was previously obtained from the factory.
        /// </summary>
        /// <param name="component">
        /// The component to be released.
        /// </param>
        void Release(T component);
    }
}