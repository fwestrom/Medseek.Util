namespace Medseek.Util.Ioc
{
    using System;

    /// <summary>
    /// Describes the data associated with a component registration event 
    /// notification.
    /// </summary>
    public class RegisteringComponentEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the registration is 
        /// canceled.
        /// </summary>
        public bool Cancel
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the component registration description.
        /// </summary>
        public Registration Registration
        {
            get; 
            set;
        }
    }
}