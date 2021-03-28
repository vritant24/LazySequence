using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LazySequence.Test
{
    [TestClass]
    public class AsyncLazySequence
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
                _ = AsyncLazySequence<int?>.Create(
                    firstElement, (i, k) => Task.FromResult((i, false)));
            }
            else
            {
                _ = AsyncLazySequence<int?>.Create(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    firstElement, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [TestMethod]
        public async Task ShouldReturnCorrectSequenceAsync()
        {
            IAsyncEnumerable<int> x = AsyncLazySequence<int>.Create(
                    1, (i, k) => Task.FromResult((i + 1, false))); ;

            Assert.AreEqual(1, await x.FirstAsync());
            Assert.AreEqual(2, await x.Skip(1).FirstAsync());
            Assert.AreEqual(3, await x.Skip(2).FirstAsync());
        }

        [TestMethod]
        public async Task ShouldUpdateIndexAsync()
        {
            IAsyncEnumerable<int> x = AsyncLazySequence<int>.Create(
                    0, (_, k) => Task.FromResult(((int)k, false)));

            Assert.AreEqual(1, await x.Skip(1).FirstAsync());
            Assert.AreEqual(2, await x.Skip(2).FirstAsync());
        }

        [TestMethod]
        public async Task ShouldCompleteAsync()
        {
            IAsyncEnumerable<int> x = AsyncLazySequence<int>.Create(
                    1, (i, _) => Task.FromResult((i + 1, i == 10)));

            Assert.AreEqual(10, await x.CountAsync());
        }
    }
}
