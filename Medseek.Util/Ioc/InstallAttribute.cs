namespace Medseek.Util.Ioc
{
    using System;

    /// <summary>
    /// Marks a type for installation as a component to be installed in the 
    /// injection container.
    /// NOTE: Attributed type must implement <see cref="IInstallable" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InstallAttribute : Attribute
    {
        /// <summary>
        /// Converts the attribute to an installable object instance.
        /// </summary>
        /// <param name="attributedType">
        /// The type onto which the registration attribute is applied.
        /// </param>
        /// <returns>
        /// An installable object instance.
        /// </returns>
        public virtual IInstallable ToInstallable(Type attributedType)
        {
            if (attributedType == null)
                throw new ArgumentNullException("attributedType");

            var installable = (IInstallable)Activator.CreateInstance(attributedType);
            return installable;
        }
    }
}