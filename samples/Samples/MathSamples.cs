using System.Collections.Generic;
using System.Linq;
using LazySequence;

namespace Samples
{
    /// <summary>
    /// A class consisting of examples of Sequence usage with
    /// Arithmetic and Geometric progressions
    /// </summary>
    public class MathSamples
    {
        /// <summary>
        /// Create a sequence of all positive integers.
        /// Store a subset of the sequence.
        /// </summary>
        public static void ArithmeticProgression()
        {
            /*
             * An arithmetic progression is a series of numbers where each number is
             * is derived from the previous sumber by adding it with a constant.
             * example: [1, 2, 3, 4, ...]
             * this is a sequence where each number is 1 plus the previous number.
             * 2 = 1 + 1
             * 3 = 2 + 1
             * and so on
             */

            // Create a sequence of all positive integers.
            IEnumerable<int> allPositiveIntegers = LazySequence<int>.Create(
                firstElement: 1,
                (prev, index) => (prev + 1, false));

            // Store a subset as a list.
            // [1, 2, 3, ..., 50]
            var firstFiftyPositiveIntegers = allPositiveIntegers.Take(50).ToList();
        }

        /// <summary>
        /// Create a sequence of all powers of 2.
        /// Store a substet of the sequence.
        /// </summary>
        public static void GeometricProgression()
        {
            /*
             * A geometric progression is a series of numbers where each number is
             * is derived from the previous sumber by multiplying it with a constant.
             * example: [1, 2, 4, 8, 16, ...]
             * this is a sequence where each number is 2 times the previous number.
             * 2 = 1 * 2
             * 4 = 2 * 2
             * and so on
             */

            // Create a sequence of all powers of 2.
            IEnumerable<int> allPowersOfTwo = LazySequence<int>.Create(
                firstElement: 1,
                (prev, index) => (prev *2, false));

            // Store a subset as a list.
            // [1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024]
            var firstTenPowersOfTwo = allPowersOfTwo.Take(10).ToList();
        }

        /// <summary>
        /// Create a fibbionacci sequence
        /// Store a substet of the sequence.
        /// </summary>
        public static void FibionacciSequence()
        {
            /*
             * A fibionacci sequence is a series of numbers where each number
             * is the sum of the two previous numbers. The series begins with [0,1]
             * example: [0, 1, 1, 2,]
             * where the third element 1 = the sum of the first two elements (1 + 0)
             * and the fourth element 2 = the sum of the previous two elements (1, 1)
             */

            // Create the fibbionacci sequence where the state is used to store
            // the second last number.
            IEnumerable<int> fibionacciSequence = LazySequence<int, int>.Create(
                firstElement: 0,
                initialState: 1,
                (prevElement, secondPrevElement, index) =>
                    (prevElement + secondPrevElement, prevElement, false));

            // Store a subset as a list.
            // [0, 1, 1, 2, 3, 5, 8, 13, 21, 34]
            var firstTenFibbionacciSequence = fibionacciSequence.Take(10).ToList();
        }
    }
}
