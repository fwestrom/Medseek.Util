using System;
using System.Globalization;

namespace Medseek.Util.Extensions.Strings
{
    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// Formats a string with the arguments provided using <see cref="CultureInfo.InvariantCulture"/>
        /// </summary>
        /// <param name="value">The string defining the format.</param>
        /// <param name="args">The arguments to be inserted in the string.</param>
        /// <returns></returns>
        public static string FormatWith(this string value, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, value, args);
        }

        /// <summary>
        /// Determines whether the string is null or whitespace.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns>True if the value is null or whitespace.</returns>
        public static bool IsNullOrWhitespace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Determines whether the string is null or empty.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns>True if the value is null or empty.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }


        /// <summary>
        /// Guards against null or whitespace strings.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="stringName">Name of the string to be inserted in <see cref="format"/>.</param>
        /// <param name="format">The message format.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void Guard(this string value, string stringName, string format)
        {
            if (value.IsNullOrWhitespace())
                throw new ArgumentException(format.FormatWith(stringName), stringName);
        }

        /// <summary>
        /// Guards against null or whitespace strings.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="stringName">Name of the string to be inserted in the error message.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void Guard(this string value, string stringName)
        {
            value.Guard(stringName, "{0} cannot be null or whitespace.");
        }
    }
}
