using System;
using System.Collections;
using System.Collections.Generic;

namespace StreamableSequence
{
    public class StreamableSequence<T> : IEnumerable<T>
    {
        private readonly GetNextElementDelegate getNextElement;
        private readonly T firstElement;

        public delegate (T nextElement, bool isLastElement) GetNextElementDelegate(
            T previousElement, ulong nextIndex);

        /// <summary>
        /// A class that allows you to lazily generate elements of a sequence
        /// and be iterated on
        /// </summary>
        /// <param name="getNextElement">
        /// A function that is given:
        /// 1. The previous element of the sequence
        /// 2. The index of the requested element
        /// and returns a tuple of:
        /// 1. The next element in the sequence
        /// 2. A bool to indicate the element is the last element
        /// </param>
        /// <param name="firstElement">
        /// The first element of the sequence
        /// </param>
        public StreamableSequence(
            T firstElement,
            GetNextElementDelegate getNextElement)
        {
            this.getNextElement = getNextElement
                ?? throw new ArgumentNullException(nameof(getNextElement));
            this.firstElement = firstElement
                ?? throw new ArgumentNullException(nameof(firstElement));
        }

        #region IEnumerable
        public IEnumerator<T> GetEnumerator()
        {
            var isCompleted = false;
            var currentElement = this.firstElement;
            ulong indexOfCurrentElement = 0;

            while (!isCompleted)
            {
                yield return currentElement;

                indexOfCurrentElement++;
                (currentElement, isCompleted) = 
                    getNextElement(currentElement, indexOfCurrentElement);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion
    }
}
