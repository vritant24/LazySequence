using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LazySequence.Test
{
    [TestClass]
    public class AsyncStatefulLazySequenceTest
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
                _ = AsyncLazySequence<int?, int?>.Create(
                    firstElement, initialState, (i, j, k) => Task.FromResult((i, j, false)));
            }
            else
            {
                _ = AsyncLazySequence<int?, int?>.Create(
                    firstElement, initialState, null);
            }
        }

        [TestMethod]
        public async Task ShouldReturnCorrectSequenceAsync()
        {
            var x = AsyncLazySequence<int, int>.Create(
                    1, 0, (i, _, _) => Task.FromResult((i + 1, 0, false)));

            Assert.AreEqual(1, await x.FirstAsync());
            Assert.AreEqual(2, await x.Skip(1).FirstAsync());
            Assert.AreEqual(3, await x.Skip(2).FirstAsync());
        }

        [TestMethod]
        public async Task ShouldUpdateStateAsync()
        {
            var x = AsyncLazySequence<int, int>.Create(
                    1, 0, (i, j, _) => Task.FromResult((i + j, 1, false)));

            Assert.AreEqual(1, await x.Skip(1).FirstAsync());
            Assert.AreEqual(2, await x.Skip(2).FirstAsync());
        }

        [TestMethod]
        public async Task ShouldUpdateIndexAsync()
        {
            var x = AsyncLazySequence<int, int>.Create(
                    0, 0, (_, _, k) => Task.FromResult(((int)k, 0, false)));

            Assert.AreEqual(1, await x.Skip(1).FirstAsync());
            Assert.AreEqual(2, await x.Skip(2).FirstAsync());
        }

        [TestMethod]
        public async Task ShouldCompleteAsync()
        {
            var x = AsyncLazySequence<int, int>.Create(
                    1, 1, (i, j, _) => Task.FromResult((i + j, 1, i == 10)));

            Assert.AreEqual(10, await x.CountAsync());
        }
    }
}
