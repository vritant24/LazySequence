using System;
using System.Collections;
using System.Collections.Generic;

namespace LazySequence
{
    public class LazySequence<T, U> : IEnumerable<T>
    {
        private readonly StatefulGetNextElementDelegate getNextElement;
        private readonly T firstElement;
        private readonly U initialState;

        public delegate (T nextElement, U currentState, bool isLastElement) StatefulGetNextElementDelegate(
            T previousElement, U state, ulong nextIndex);

        /// <summary>
        /// A class that allows you to lazily generate elements of a sequence
        /// that can be iterated on with access to additional state
        /// `T` represents the type of the elements
        /// `U` represents the type of the state
        /// </summary>
        /// <param name="firstElement">
        /// The first element of the sequence
        /// </param>
        /// <param name="initialState">
        /// Initial state during the enumeration of the sequence
        /// </param>
        /// <param name="getNextElement">
        /// A function that is given:
        /// 1. The previous element of the sequence
        /// 2. modifiable state
        /// 3. The index of the requested element
        /// and returns a tuple of:
        /// 1. The next element in the sequence
        /// 2. State to use in the next iteration
        /// 3. A bool to indicate the element is the last element
        /// </param>
        public static IEnumerable<T> Create(
            T firstElement,
            U initialState,
            StatefulGetNextElementDelegate getNextElement)
        {
            getNextElement = getNextElement
                ?? throw new ArgumentNullException(nameof(getNextElement));
            firstElement = firstElement
                ?? throw new ArgumentNullException(nameof(firstElement));
            initialState = initialState
                ?? throw new ArgumentNullException(nameof(initialState));

            return new LazySequence<T, U>(firstElement, initialState, getNextElement);
        }

        private LazySequence(
            T firstElement,
            U initialState,
            StatefulGetNextElementDelegate getNextElement)
        {
            this.getNextElement = getNextElement;
            this.firstElement = firstElement;
            this.initialState = initialState;
        }

        #region IEnumerable
        public IEnumerator<T> GetEnumerator()
        {
            var isCompleted = false;
            var currentElement = firstElement;
            var currentState = initialState;
            ulong indexOfCurrentElement = 0;

            while (!isCompleted)
            {
                yield return currentElement;

                indexOfCurrentElement++;
                (currentElement, currentState, isCompleted) =
                    getNextElement(currentElement, currentState, indexOfCurrentElement);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
