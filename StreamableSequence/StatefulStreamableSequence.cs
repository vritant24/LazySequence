using System;
using System.Collections;
using System.Collections.Generic;

namespace StreamableSequence
{
    public class StatefulStreamableSequence<T, U> : IEnumerable<T>
    {
        private readonly StatefulGetNextElementDelegate getNextElement;
        private readonly T firstElement;
        private readonly U initialState;

        public delegate (T nextElement, U currentState, bool isLastElement) StatefulGetNextElementDelegate(
            T previousElement, U state, ulong nextIndex);

        /// <summary>
        /// A class that allows you to lazily generate elements of a sequence
        /// and be iterated on with access to additional state
        /// </summary>
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
        /// <param name="firstElement">
        /// The first element of the sequence
        /// </param>
        public StatefulStreamableSequence(
            T firstElement,
            U initialState,
            StatefulGetNextElementDelegate getNextElement)
        {
            this.getNextElement = getNextElement 
                ?? throw new ArgumentNullException(nameof(getNextElement));
            this.firstElement = firstElement
                ?? throw new ArgumentNullException(nameof(firstElement));
            this.initialState = initialState
                ?? throw new ArgumentNullException(nameof(initialState));
        }

        #region IEnumerable
        public IEnumerator<T> GetEnumerator()
        {
            var isCompleted = false;
            var currentElement = this.firstElement;
            var currentState = this.initialState;
            ulong indexOfCurrentElement = 0;

            while (!isCompleted)
            {
                yield return currentElement;

                indexOfCurrentElement++;
                (currentElement, currentState, isCompleted) =
                    getNextElement(currentElement, currentState, indexOfCurrentElement);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion
    }
}
