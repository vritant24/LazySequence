using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StreamableSequence
{
    public class AsyncLazySequence<T> : IAsyncEnumerable<T>
    {
        private readonly GetNextElementDelegateAsync getNextElementAsync;
        private readonly T firstElement;

        public delegate Task<(T nextElement, bool isLastElement)> GetNextElementDelegateAsync(
            T previousElement, ulong nextIndex);

        /// <summary>
        /// A class that allows you to lazily and asynchronously generate
        /// elements of a sequence rhar can be iterated on
        /// `T` represents the type of the elements
        /// </summary>
        /// <param name="firstElement">
        /// The first element of the sequence
        /// </param>
        /// <param name="getNextElementAsync">
        /// A function that is given:
        /// 1. The previous element of the sequence
        /// 2. The index of the requested element
        /// and returns a Task that resolves to a tuple of:
        /// 1. The next element in the sequence
        /// 2. A bool to indicate the element is the last element
        /// </param>
        public static IAsyncEnumerable<T> Create(
            T firstElement,
            GetNextElementDelegateAsync getNextElementAsync)
        {
            getNextElementAsync = getNextElementAsync
                ?? throw new ArgumentNullException(nameof(getNextElementAsync));
            firstElement = firstElement
                ?? throw new ArgumentNullException(nameof(firstElement));

            return new AsyncLazySequence<T>(firstElement, getNextElementAsync);
        }

        private AsyncLazySequence(
            T firstElement,
            GetNextElementDelegateAsync getNextElementAsync)
        {
            this.getNextElementAsync = getNextElementAsync
                ?? throw new ArgumentNullException(nameof(getNextElementAsync));
            this.firstElement = firstElement
                ?? throw new ArgumentNullException(nameof(firstElement));
        }

        #region IAsyncEnumerable
        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var isCompleted = false;
            var currentElement = this.firstElement;
            ulong indexOfCurrentElement = 0;

            while (!isCompleted)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return currentElement;

                indexOfCurrentElement++;
                (currentElement, isCompleted) = await
                    getNextElementAsync(currentElement, indexOfCurrentElement);
            }
        }
        #endregion
    }
}
