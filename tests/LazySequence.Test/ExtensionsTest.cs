using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LazySequence.Test
{
    [TestClass]
    public class ExtensionsTest
    {
        [TestMethod]
        public void TestTryGetNext()
        {
            IEnumerator<string> lazySequence = LazySequence<string>
                .Create("", (prev, idx) => ($"{idx}", idx >= 2))
                .GetEnumerator();

            Assert.AreEqual(true, lazySequence.TryGetNext(out var el1));
            Assert.AreEqual("", el1);
            Assert.AreEqual(true, lazySequence.TryGetNext(out var el2));
            Assert.AreEqual("1", el2);
            Assert.AreEqual(false, lazySequence.TryGetNext(out _));
        }

        [TestMethod]
        public async Task TestTryGetNextAsync()
        {
            IAsyncEnumerator<string> lazySequence = AsyncLazySequence<string>
                .Create("", (prev, idx) => Task.FromResult(($"{idx}", idx >= 2)))
                .GetAsyncEnumerator();

            (bool hasElement, string? element) res1 = await lazySequence.TryGetNextAsync();
            (bool hasElement, string? element) res2 = await lazySequence.TryGetNextAsync();
            (bool hasElement, string? element) res3 = await lazySequence.TryGetNextAsync();

            Assert.AreEqual(true, res1.hasElement);
            Assert.AreEqual("", res1.element);
            Assert.AreEqual(true, res2.hasElement);
            Assert.AreEqual("1", res2.element);
            Assert.AreEqual(false, res3.hasElement);
        }
    }
}
