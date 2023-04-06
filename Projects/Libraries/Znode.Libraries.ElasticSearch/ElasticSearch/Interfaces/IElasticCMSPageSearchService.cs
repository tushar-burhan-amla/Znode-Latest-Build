using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public interface IElasticCMSPageSearchService
    {
        #region CMS page request

        /// <summary>
        /// Get cms page search response of cms page request
        /// </summary>
        /// <param name="request">IZnodeCMSPageSearchRequest</param>
        /// <returns>Return cms pages for the search keyword</returns>
        IZnodeCMSPageSearchResponse FullTextContentPageSearch(IZnodeCMSPageSearchRequest request);

        /// <summary>
        /// Get cms page count response of cms page request
        /// </summary>
        /// <param name="request">IZnodeCMSPageSearchRequest</param>
        /// <returns>Return cms pages count for the search keyword</returns>
        IZnodeCMSPageSearchResponse GetSearchContentPageCount(IZnodeCMSPageSearchRequest request);

        #endregion
    }
}
