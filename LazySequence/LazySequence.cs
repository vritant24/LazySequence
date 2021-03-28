using System;
using System.Collections;
using System.Collections.Generic;

namespace LazySequence
{
    public class LazySequence<T> : IEnumerable<T>
    {
        private readonly GetNextElementDelegate getNextElement;
        private readonly T firstElement;

        public delegate (T nextElement, bool isLastElement) GetNextElementDelegate(
            T previousElement, ulong nextIndex);

        /// <summary>
        /// A class that allows you to lazily generate elements of a sequence
        /// that can be iterated on
        /// `T` represents the type of the elements
        /// </summary>
        /// <param name="firstElement">
        /// The first element of the sequence
        /// </param>
        /// <param name="getNextElement">
        /// A function that is given:
        /// 1. The previous element of the sequence
        /// 2. The index of the requested element
        /// and returns a tuple of:
        /// 1. The next element in the sequence
        /// 2. A bool to indicate the element is the last element
        /// </param>
        public static IEnumerable<T> Create(
            T firstElement,
            GetNextElementDelegate getNextElement)
        {
            getNextElement = getNextElement
                ?? throw new ArgumentNullException(nameof(getNextElement));
            firstElement = firstElement
                ?? throw new ArgumentNullException(nameof(firstElement));

            return new LazySequence<T>(firstElement, getNextElement);
        }

        private LazySequence(
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
            var currentElement = firstElement;
            ulong indexOfCurrentElement = 0;

            while (!isCompleted)
            {
                yield return currentElement;

                indexOfCurrentElement++;
                (currentElement, isCompleted) =
                    getNextElement(currentElement, indexOfCurrentElement);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
