using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LazySequence.Test
{
    [TestClass]
    public class StatefulLazySequenceTest
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
                _ = LazySequence<int?, int?>.Create(
                    firstElement, initialState, (i, j, k) => (0, 0, false));
            }
            else
            {
                _ = LazySequence<int?, int?>.Create(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    firstElement, initialState, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [TestMethod]
        public void ShouldReturnCorrectSequence()
        {
            IEnumerable<int> x = LazySequence<int, int>.Create(
                    1, 0, (i, _, _) => (i + 1, 0, false));

            Assert.AreEqual(1, x.First());
            Assert.AreEqual(2, x.Skip(1).First());
            Assert.AreEqual(3, x.Skip(2).First());
        }

        [TestMethod]
        public void ShouldUpdateState()
        {
            IEnumerable<int> x = LazySequence<int, int>.Create(
                    1, 0, (i, j, _) => (i + j, 1, false));

            Assert.AreEqual(1, x.Skip(1).First());
            Assert.AreEqual(2, x.Skip(2).First());
        }

        [TestMethod]
        public void ShouldUpdateIndex()
        {
            IEnumerable<int> x = LazySequence<int, int>.Create(
                    0, 0, (_, _, k) => ((int)k, 0, false));

            Assert.AreEqual(1, x.Skip(1).First());
            Assert.AreEqual(2, x.Skip(2).First());
        }

        [TestMethod]
        public void ShouldComplete()
        {
            IEnumerable<int> x = LazySequence<int, int>.Create(
                    1, 1, (i, j, _) => (i + j, 1, i == 10));

            Assert.AreEqual(10, x.Count());
        }
    }
}
