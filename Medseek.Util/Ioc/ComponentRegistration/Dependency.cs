namespace Medseek.Util.Ioc.ComponentRegistration
{
    /// <summary>
    /// Describes components dependencies.
    /// </summary>
    public abstract class Dependency
    {
        /// <summary>
        /// Specifies that a specified value should be used to fulfil any
        /// dependencies with a specific key or dependency name.
        /// </summary>
        public static Dependency OnValue(string key, object value)
        {
            return new ValueDependency(key, value);
        }
    }
}