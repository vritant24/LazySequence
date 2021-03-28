using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LazySequence.Test
{
    [TestClass]
    public class LazySequenceTest
    {
        [TestMethod]
        [DataRow(0, false)]
        [DataRow(null, true)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContructorShouldThrowForNullArgs(
           int? firstElement,
           bool getNextElement)
        {
            if (getNextElement)
            {
                _ = LazySequence<int?>.Create(
                    firstElement, (i, k) => (0, false));
            }
            else
            {
                _ = LazySequence<int?>.Create(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    firstElement, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [TestMethod]
        public void ShouldReturnCorrectSequence()
        {
            IEnumerable<int> x = LazySequence<int>.Create(
                    1, (i, k) => (i + 1, false));

            Assert.AreEqual(1, x.First());
            Assert.AreEqual(2, x.Skip(1).First());
            Assert.AreEqual(3, x.Skip(2).First());
        }

        [TestMethod]
        public void ShouldUpdateIndex()
        {
            IEnumerable<int> x = LazySequence<int>.Create(
                    0, (_, k) => ((int)k, false));

            Assert.AreEqual(1, x.Skip(1).First());
            Assert.AreEqual(2, x.Skip(2).First());
        }

        [TestMethod]
        public void ShouldComplete()
        {
            IEnumerable<int> x = LazySequence<int>.Create(
                    1, (i, _) => (i + 1, i == 10));

            Assert.AreEqual(10, x.Count());
        }
    }
}
