using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace LazySequence
{
    /// <summary>
    /// Useful extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the next element in the enumerator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator">
        /// <see cref="IEnumerator{T}"/> returned from
        /// <see cref="IEnumerable{T}.GetEnumerator"/>
        /// </param>
        /// <param name="element">
        /// The next element found in the sequence.
        /// `default` if the enumeration has ended.
        /// </param>
        /// <returns>
        /// True if the enumeration has not ended,
        /// False if it has
        /// </returns>
        public static bool TryGetNext<T>(
            this IEnumerator<T> enumerator,
            [MaybeNullWhen(false)] out T element)
        {
            if (enumerator.MoveNext())
            {
                element = enumerator.Current;
                return true;
            }

            element = default;
            return false;
        }

        /// <summary>
        /// Gets the next element in the async enumerator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator">
        /// <see cref="IAsyncEnumerator{T}"/> returned from
        /// <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator(System.Threading.CancellationToken)"/>
        /// </param>
        /// <returns>
        /// A <see cref="Tuple"/> of
        /// <list type="number">
        /// <item>A bool indicating whether there is an element in the enumeration</item>
        /// <item>The next element of the enumeration if any</item>
        /// </list>
        /// </returns>
        public static async Task<(bool hasElement, T? element)> TryGetNextAsync<T>(
            this IAsyncEnumerator<T> enumerator)
        {
            if (await enumerator.MoveNextAsync())
            {
                return (true, enumerator.Current);
            }

            return (false, default);
        }
    }
}
