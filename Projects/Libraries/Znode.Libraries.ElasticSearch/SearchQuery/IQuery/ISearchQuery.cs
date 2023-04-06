using Nest;

using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public interface ISearchQuery
    {
        /// <summary>
        /// Generate elasticsearch request query as per passed search request values
        /// </summary>
        /// <param name="request">Search request</param>
        /// <returns>Elasticsearch request query</returns>
        SearchRequest<dynamic> GenerateQuery(IZnodeSearchRequest request);
    }
}
