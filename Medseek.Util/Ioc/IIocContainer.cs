namespace Medseek.Util.Ioc
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for types that provide the functionality of an inversion of 
    /// control container.
    /// </summary>
    public interface IIocContainer : IDisposable
    {
        /// <summary>
        /// Raised to indicate that a component has been registered with the 
        /// container.
        /// </summary>
        event EventHandler<RegisterComponentEventArgs> RegisteredComponent;

        /// <summary>
        /// Gets the set of components that are registered with the container.
        /// </summary>
        IEnumerable<ComponentInfo> Components
        {
            get;
        }

        /// <summary>
        /// Gets the plugin that was used to obtain the container.
        /// </summary>
        IIocPlugin Plugin
        {
            get;
        }

        /// <summary>
        /// Installs the specified installable types into the container.
        /// </summary>
        /// <param name="installables">
        /// The installable types to install into the container.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The container installation should only be invoked once, and 
        /// subsequent attempts may throw an invalid operation exception.
        /// </exception>
        /// <returns>
        /// The instance of container upon which the method was invoked, to 
        /// enable chained invocations.
        /// </returns>
        IIocContainer Install(params IInstallable[] installables);

        /// <summary>
        /// Registers the components with the inversion of control container.
        /// </summary>
        /// <param name="registrations">
        /// The component descriptions to register with the container.
        /// </param>
        /// <returns>
        /// The instance of container upon which the method was invoked, to 
        /// enable chained invocations.
        /// </returns>
        IIocContainer Register(params Registration[] registrations);

        /// <summary>
        /// Releases a component instance that was previously resolved from 
        /// the inversion of control container.
        /// </summary>
        /// <param name="component">
        /// The component instance to release.
        /// </param>
        /// <seealso cref="Resolve" />
        void Release(object component);

        /// <summary>
        /// Resolves a component by a required service type.
        /// </summary>
        /// <param name="service">
        /// The required service type provided by the component.
        /// </param>
        /// <returns>
        /// The component instance that was resolved.
        /// </returns>
        /// <seealso cref="Release" />
        object Resolve(Type service);

        /// <summary>
        /// Resolves the available components by a required service type.
        /// </summary>
        /// <param name="service">
        /// The required service type provided by the components.
        /// </param>
        /// <returns>
        /// The component instances that were resolved.
        /// </returns>
        /// <seealso cref="Release" />
        IEnumerable<object> ResolveAll(Type service);
    }
}