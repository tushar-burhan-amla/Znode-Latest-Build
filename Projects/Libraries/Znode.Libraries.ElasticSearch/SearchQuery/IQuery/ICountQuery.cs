using Nest;

using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public interface ICountQuery
    {
        /// <summary>
        /// Generate count query for elasticsearch api to get count
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Return request query to get count</returns>
        CountRequest<dynamic> GenerateCountQuery(IZnodeSearchRequest request);
    }
}