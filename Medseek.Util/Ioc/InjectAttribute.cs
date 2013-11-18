namespace Medseek.Util.Ioc
{
    using System;

    /// <summary>
    /// Attribute indicating the details of an injected component constructor parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public class InjectAttribute : Attribute
    {
        private readonly string componentName;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttribute"/> 
        /// class.
        /// </summary>
        /// <param name="componentName">
        /// The name of the component to inject, or null to use the name of the
        /// parameter as the desired component name.
        /// </param>
        public InjectAttribute(string componentName = null)
        {
            this.componentName = componentName;
        }

        /// <summary>
        /// Gets the name of the component that should be injected.
        /// </summary>
        public string ComponentName
        {
            get
            {
                return componentName;
            }
        }
    }
}