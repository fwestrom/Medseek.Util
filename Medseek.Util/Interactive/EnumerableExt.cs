namespace Medseek.Util.Interactive
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides extension methods for working with enumerable sequences and 
    /// for creating side effects for their evaluation.
    /// </summary>
    public static class EnumerableExt
    {
        /// <summary>
        /// Performs an action as each value is produced while enumerating 
        /// the values of a sequence.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of elements in the source sequence.
        /// </typeparam>
        /// <param name="source">
        /// The source sequence.
        /// </param>
        /// <param name="onNext">
        /// An action to invoke as each element is enumerated from the 
        /// sequence.
        /// </param>
        /// <returns>
        /// An enumerable sequence which will perform the side-effects 
        /// specified by <see cref="onNext" /> when the values are enumerated.
        /// </returns>
        public static IEnumerable<TSource> Do<TSource>(this IEnumerable<TSource> source, Action<TSource> onNext)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (onNext == null)
                throw new ArgumentNullException("onNext");

            foreach (var value in source)
            {
                onNext(value);
                yield return value;
            }
        }
    }
}