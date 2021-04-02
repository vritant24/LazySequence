using System.Collections.Generic;
using System.Threading.Tasks;
using LazySequence;

namespace Samples
{
    /// <summary>
    /// Examples of using AsyncLazySequence
    /// </summary>
    public class AsyncSample
    {
        /// <summary>
        /// Create a sequence to paginate server requests
        /// </summary>
        /// <returns></returns>
        public static async Task Pagination()
        {
            /*
             * An AsyncLazySequence can be used as a medium to lazily paginate
             * over web requests to your API. 
             */

            var pageSize = 10;
            IAsyncEnumerable<object?> paginatedServerRequestCreator = AsyncLazySequence<object?, int>.Create(
                firstElement: null,
                initialState: 0,
                async (prev, currentOffset, index) =>
                {
                    var serverResponse = await ServerPageRequest(currentOffset, pageSize);
                    return (
                        nextElement: serverResponse,
                        iterationState: currentOffset + pageSize,
                        isLastElement: false);
                });

            // One way is to use the AsyncEnumerator as state and occasionally get the next
            // paged response
            IAsyncEnumerator<object?> paginatedServerRequests = paginatedServerRequestCreator.GetAsyncEnumerator();
            (var hasElement, var pagedResponse1) = await paginatedServerRequests.TryGetNextAsync();

            //The other way is to use the fact that it can be iterated on in a foreach loop
            await foreach (var pagedResponse2 in paginatedServerRequestCreator)
            {
                // use pagedResponse2 here
            }
        }

        /// <summary>
        /// Mock server page request that takes a startIndex and pageSize
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private static async Task<object> ServerPageRequest(int startIndex, int pageSize)
        {
            await Task.Delay(100);
            return Task.FromResult(new object());
        }
    }
}
