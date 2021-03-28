using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LazySequence
{
    public class AsyncLazySequence<T, U> : IAsyncEnumerable<T>
    {
        private readonly StatefulGetNextElementDelegateAsync getNextElementAsync;
        private readonly T firstElement;
        private readonly U initialState;

        public delegate Task<(T nextElement, U currentState, bool isLastElement)> StatefulGetNextElementDelegateAsync(
            T previousElement, U state, ulong nextIndex);

        /// <summary>
        /// A class that allows you to lazily and asynchronously generate elements
        /// of a sequence that can be iterated on with access to additional state
        /// `T` represents the type of the elements
        /// `U` represents the type of the state
        /// </summary>
        /// <param name="firstElement">
        /// The first element of the sequence
        /// </param>
        /// <param name="initialState">
        /// Initial state during the enumeration of the sequence
        /// </param>
        /// <param name="getNextElementAsync">
        /// A function that is given:
        /// 1. The previous element of the sequence
        /// 2. modifiable state
        /// 3. The index of the requested element
        /// and returns a Task a that resolves to tuple of:
        /// 1. The next element in the sequence
        /// 2. State to use in the next iteration
        /// 3. A bool to indicate the element is the last element
        /// </param>
        public static IAsyncEnumerable<T> Create(
            T firstElement,
            U initialState,
            StatefulGetNextElementDelegateAsync getNextElementAsync)
        {
            getNextElementAsync = getNextElementAsync
                ?? throw new ArgumentNullException(nameof(getNextElementAsync));
            firstElement = firstElement
                ?? throw new ArgumentNullException(nameof(firstElement));
            initialState = initialState
                ?? throw new ArgumentNullException(nameof(initialState));

            return new AsyncLazySequence<T, U>(firstElement, initialState, getNextElementAsync);
        }

        private AsyncLazySequence(
            T firstElement,
            U initialState,
            StatefulGetNextElementDelegateAsync getNextElementAsync)
        {
            this.getNextElementAsync = getNextElementAsync;
            this.firstElement = firstElement;
            this.initialState = initialState;
        }

        #region IAsyncEnumerable
        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var isCompleted = false;
            T? currentElement = this.firstElement;
            U? currentState = this.initialState;
            ulong indexOfCurrentElement = 0;

            while (!isCompleted)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return currentElement;

                indexOfCurrentElement++;
                (currentElement, currentState, isCompleted) = await
                    this.getNextElementAsync(currentElement, currentState, indexOfCurrentElement);
            }
        }
        #endregion
    }
}
