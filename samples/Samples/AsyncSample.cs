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
            IAsyncEnumerable<object?> paginatedServerRequests = AsyncLazySequence<object?, int>.Create(
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

            await foreach (var page in paginatedServerRequests)
            {
                // page available from server asynchronously
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
