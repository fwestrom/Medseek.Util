namespace Medseek.Util.Ioc
{
    using System;

    /// <summary>
    /// Describes the data associated with a component registration event 
    /// notification.
    /// </summary>
    public class RegisterComponentEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterComponentEventArgs"/> class.
        /// </summary>
        public RegisterComponentEventArgs(string id, object detail)
        {
            Detail = detail;
            Id = id;
        }

        /// <summary>
        /// Gets the component identifier.
        /// </summary>
        public string Id
        {
            get; 
            private set;
        }

        /// <summary>
        /// Gets an object describing the registration.
        /// </summary>
        public object Detail
        {
            get; 
            private set;
        }
    }
}