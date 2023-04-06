namespace Znode.Libraries.Search
{
    public interface IZnodeSearchProvider
    {
        /// <summary>
        /// Find terms matching the given partial word that appear in the highest number of documents
        /// </summary>
        /// <param name="request">A word or part of a word</param>
        /// <returns>A list of suggested completions</returns>
        IZnodeSearchResponse SuggestTermsFor(IZnodeSearchRequest request);

        /// <summary>
        /// Full text search for a search keyword
        /// </summary>
        /// <param name="request">Request for elasticsearch</param>
        /// <returns>Elasticsearch Response</returns>
        IZnodeSearchResponse FullTextSearch(IZnodeSearchRequest request);

        /// <summary>
        /// Method to check if elastic search is working
        /// </summary>
        /// <returns></returns>
        string CheckElasticSearch();

        #region CMS page search request

        /// <summary>
        /// Get cms pages for a search keyword. 
        /// </summary>
        /// <param name="request">Request model</param>
        /// <returns>Return cms pages from Elastic search</returns>
        IZnodeCMSPageSearchResponse FullTextContentPageSearch(IZnodeCMSPageSearchRequest request);

        /// <summary>
        /// Get cms page count for a search keyword. 
        /// </summary>
        /// <param name="request">Request model</param>
        /// <returns>Return Count of search cms page</returns>
        IZnodeCMSPageSearchResponse GetSearchContentPageCount(IZnodeCMSPageSearchRequest request);

        #endregion
    }
}
