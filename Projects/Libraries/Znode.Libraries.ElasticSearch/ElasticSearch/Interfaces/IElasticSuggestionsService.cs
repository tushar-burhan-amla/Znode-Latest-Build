using Nest;

using System.Collections.Generic;

using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public interface IElasticSuggestionsService
    {
        /// <summary>
        /// Get suggestion term for index
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Return suggest term response of elasticsearch</returns>
        IZnodeSearchResponse SuggestTermsFromExistingIndex(IZnodeSearchRequest request);

        /// <summary>
        /// Get auto-suggestions for search index
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="catalogIdLocaleIdContainerList">catalogIdLocaleIdContainerList</param>
        /// <returns>Return auto-suggestion</returns>
        ISearchResponse<dynamic> GetAutoSuggestions(IZnodeSearchRequest request, List<QueryContainer> catalogIdLocaleIdContainerList);
    }
}
