using System;
using Medseek.Util.Extensions.Strings;

namespace Medseek.Util.Extensions.Objects
{
    /// <summary>
    /// 
    /// </summary>
    public static class Objects
    {
        /// <summary>
        /// Guards against null objects.
        /// Throws <see cref="System.ArgumentNullException"/> when <see cref="value"/> is null.
        /// </summary>
        /// <param name="value">The object to .</param>
        /// <param name="objectName">Name of the object to be inserted in to <see cref="format"/>.</param>
        /// <param name="format">The message format.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void Guard(this object value, string objectName, string format)
        {
            if (value == null)
                throw new ArgumentNullException(objectName, format.FormatWith(objectName));
        }

        /// <summary>
        /// Guards against null objects.
        /// Throws <see cref="System.ArgumentNullException"/> when <see cref="value"/> is null.
        /// </summary>
        /// <param name="value">The object to .</param>
        /// <param name="objectName">Name of the object to be inserted in to the error message.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void Guard(this object value, string objectName)
        {
            if (value == null)
                throw new ArgumentNullException(objectName);
        }
    }
}
