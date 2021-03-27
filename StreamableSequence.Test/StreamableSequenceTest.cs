using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace StreamableSequence.Test
{
    [TestClass]
    public class StreamableSequenceTest
    {
        [TestMethod]
        [DataRow(0,false)]
        [DataRow(null, true)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContructorShouldThrowForNullArgs(
           int? firstElement,
           bool getNextElement)
        {
            if (getNextElement)
            {
                _ = new StreamableSequence<int?>(
                    firstElement, (i, k) => (0, false));
            }
            else
            {
                _ = new StreamableSequence<int?>(
                    firstElement, null);
            }
        }

        [TestMethod]
        public void ShouldReturnCorrectSequence()
        {
            var x = new StreamableSequence<int>(
                    1, (i, k) => (i + 1, false));

            Assert.AreEqual(1, x.First());
            Assert.AreEqual(2, x.Skip(1).First());
            Assert.AreEqual(3, x.Skip(2).First());
        }

        [TestMethod]
        public void ShouldUpdateIndex()
        {
            var x = new StreamableSequence<int>(
                    0, (_, k) => ((int)k,false));

            Assert.AreEqual(1, x.Skip(1).First());
            Assert.AreEqual(2, x.Skip(2).First());
        }

        [TestMethod]
        public void ShouldComplete()
        {
            var x = new StreamableSequence<int>(
                    1, (i, _) => (i + 1, i == 10));

            Assert.AreEqual(10, x.Count());
        }
    }
}
