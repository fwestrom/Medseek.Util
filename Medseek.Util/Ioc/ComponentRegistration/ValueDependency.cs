namespace Medseek.Util.Ioc.ComponentRegistration
{
    /// <summary>
    /// Describes a dependency on a specific value.
    /// </summary>
    public class ValueDependency : Dependency
    {
        private readonly string key;
        private readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueDependency"/> 
        /// class.
        /// </summary>
        public ValueDependency(string key, object value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Gets the key associated with the property.
        /// </summary>
        public string Key
        {
            get
            {
                return key;
            }
        }

        /// <summary>
        /// Gets the value associated with the property.
        /// </summary>
        public object Value
        {
            get
            {
                return value;
            }
        }
    }
}