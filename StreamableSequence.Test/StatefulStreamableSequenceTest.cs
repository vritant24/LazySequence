using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace StreamableSequence.Test
{
    [TestClass]
    public class StatefulStreamableSequenceTest
    {
        [TestMethod]
        [DataRow(0, 0, false)]
        [DataRow(null, 0, true)]
        [DataRow(0, null, true)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContructorShouldThrowForNullArgs(
            int? firstElement,
            int? initialState,
            bool getNextElement)
        {
            if (getNextElement)
            {
                _ = new StatefulStreamableSequence<int?, int?>(
                    firstElement, initialState, (i, j, k) => (0, 0, false));
            }
            else
            {
                _ = new StatefulStreamableSequence<int?, int?>(
                    firstElement, initialState, null);
            }
        }

        [TestMethod]
        public void ShouldReturnCorrectSequence()
        {
            var x = new StatefulStreamableSequence<int, int>(
                    1, 0, (i, _, _) => (i + 1, 0, false));

            Assert.AreEqual(1, x.First());
            Assert.AreEqual(2, x.Skip(1).First());
            Assert.AreEqual(3, x.Skip(2).First());
        }

        [TestMethod]
        public void ShouldUpdateState()
        {
            var x = new StatefulStreamableSequence<int, int>(
                    1, 0, (i, j, _) => (i + j, 1, false));

            Assert.AreEqual(1, x.Skip(1).First());
            Assert.AreEqual(2, x.Skip(2).First());
        }

        [TestMethod]
        public void ShouldUpdateIndex()
        {
            var x = new StatefulStreamableSequence<int, int>(
                    0, 0, (_, _, k) => ((int)k, 0, false));

            Assert.AreEqual(1, x.Skip(1).First());
            Assert.AreEqual(2, x.Skip(2).First());
        }

        [TestMethod]
        public void ShouldComplete()
        {
            var x = new StatefulStreamableSequence<int, int>(
                    1, 1, (i, j, _) => (i + j, 1, i == 10));

            Assert.AreEqual(10, x.Count());
        }
    }
}
