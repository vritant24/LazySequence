﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LazySequence
{
    /// <summary>
    /// A class that allows you to lazily and asynchronously generate elements
    /// of a sequence that can be iterated on with access to additional state.
    /// </summary>
    /// <typeparam name="T">The type of element in the sequence.</typeparam>
    /// <typeparam name="U">
    /// The type of state given to <see cref="StatefulGetNextElementDelegateAsync"/>.
    /// </typeparam>
    public class AsyncLazySequence<T, U> : IAsyncEnumerable<T>
    {
        private readonly StatefulGetNextElementDelegateAsync getNextElementAsync;
        private readonly T firstElement;
        private readonly U initialState;

        /// <summary>
        /// Delegate to asynchronously generate next element in the sequence.
        /// </summary>
        /// <param name="previousElement">Previous element in the sequence.</param>
        /// <param name="iterationState">State in the current iteration of the sequence.</param>
        /// <param name="nextIndex">Index of element to be generated by this delegate.</param>
        /// <returns>
        /// A <see cref="Task"/> that resolves to a <see cref="Tuple"/> of:
        /// <list type="number">
        /// <item>The next element in the sequence.</item>
        /// <item>The updated state for the current iteration.</item>
        /// <item>A bool to indicate whether the returned element is the last element.</item>
        /// </list>
        /// </returns>
        public delegate Task<(T nextElement, U iterationState, bool isLastElement)> StatefulGetNextElementDelegateAsync(
            T previousElement, U iterationState, ulong nextIndex);

        /// <summary>
        /// Creates <see cref="AsyncLazySequence{T, U}"/>
        /// </summary>
        /// <param name="firstElement">The first element of the sequence.</param>
        /// <param name="initialState">
        /// Initial state during the enumeration of the sequence.
        /// </param>
        /// <param name="getNextElementAsync">
        /// <see cref="StatefulGetNextElementDelegateAsync"/>
        /// </param>
        public static IAsyncEnumerable<T> Create(
            T firstElement,
            U initialState,
            StatefulGetNextElementDelegateAsync getNextElementAsync)
        {
            firstElement = firstElement
                ?? throw new ArgumentNullException(nameof(firstElement));
            initialState = initialState
                ?? throw new ArgumentNullException(nameof(initialState));
            getNextElementAsync = getNextElementAsync
                ?? throw new ArgumentNullException(nameof(getNextElementAsync));

            return new AsyncLazySequence<T, U>(firstElement, initialState, getNextElementAsync);
        }

        private AsyncLazySequence(
            T firstElement,
            U initialState,
            StatefulGetNextElementDelegateAsync getNextElementAsync)
        {
            this.firstElement = firstElement;
            this.initialState = initialState;
            this.getNextElementAsync = getNextElementAsync;
        }

        #region IAsyncEnumerable
        /// <summary>
        /// Lazily and asynchronously iterates on the sequence.
        /// </summary>
        /// <returns>Enumerator that can be iterated on.</returns>
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