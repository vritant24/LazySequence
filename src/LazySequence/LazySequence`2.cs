﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace LazySequence
{
    /// <summary>
    /// Allows you to lazily generate elements of a sequence
    /// that can be iterated on with access to additional state.
    /// </summary>
    /// <typeparam name="T">The type of element in the sequence.</typeparam>
    /// <typeparam name="U">
    /// The type of state given to <see cref="StatefulGetNextElementDelegate"/>.
    /// </typeparam>
    public class LazySequence<T, U> : IEnumerable<T>
    {
        private readonly StatefulGetNextElementDelegate getNextElement;
        private readonly T firstElement;
        private readonly U initialState;

        /// <summary>
        /// A <see cref="Delegate"/> to generate next element in the sequence.
        /// </summary>
        /// <param name="previousElement">Previous element in the sequence.</param>
        /// <param name="iterationState">State in the current iteration of the sequence.</param>
        /// <param name="nextIndex">Index of element to be generated by this delegate.</param>
        /// <returns>
        /// A <see cref="Tuple"/> of:
        /// <list type="number">
        /// <item>The next element in the sequence.</item>
        /// <item>The updated state for the current iteration.</item>
        /// <item>A bool to indicate whether the returned element is the last element.</item>
        /// </list>
        /// </returns>
        public delegate (T nextElement, U iterationState, bool isLastElement) StatefulGetNextElementDelegate(
            T previousElement, U iterationState, ulong nextIndex);

        /// <summary>
        /// Creates a <see cref="LazySequence{T, U}"/>
        /// </summary>
        /// <param name="firstElement">The first element of the sequence.</param>
        /// <param name="initialState">
        /// Initial state during the enumeration of the sequence.
        /// </param>
        /// <param name="getNextElement">
        /// <see cref="StatefulGetNextElementDelegate"/>
        /// </param>
        public static IEnumerable<T> Create(
            T firstElement,
            U initialState,
            StatefulGetNextElementDelegate getNextElement)
        {
            firstElement = firstElement
                ?? throw new ArgumentNullException(nameof(firstElement));
            initialState = initialState
                ?? throw new ArgumentNullException(nameof(initialState));
            getNextElement = getNextElement
                ?? throw new ArgumentNullException(nameof(getNextElement));

            return new LazySequence<T, U>(firstElement, initialState, getNextElement);
        }

        private LazySequence(
            T firstElement,
            U initialState,
            StatefulGetNextElementDelegate getNextElement)
        {
            this.firstElement = firstElement;
            this.initialState = initialState;
            this.getNextElement = getNextElement;
        }

        #region IEnumerable
        /// <summary>
        /// Lazily iterates on the sequence.
        /// </summary>
        /// <returns>Enumerator that can be iterated on.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            var isCompleted = false;
            T? currentElement = this.firstElement;
            U? currentState = this.initialState;
            ulong indexOfCurrentElement = 0;

            while (!isCompleted)
            {
                yield return currentElement;

                indexOfCurrentElement++;
                (currentElement, currentState, isCompleted) =
                    this.getNextElement(currentElement, currentState, indexOfCurrentElement);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion
    }
}
