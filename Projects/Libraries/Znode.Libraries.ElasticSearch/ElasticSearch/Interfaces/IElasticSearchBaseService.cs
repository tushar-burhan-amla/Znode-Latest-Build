using Nest;

using System.Collections.Generic;

using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public interface IElasticSearchBaseService
    {
        /// <summary>
        ///  Full text search (which contains the text of several text fields at once).
        /// </summary>
        /// <param name="request">Elasticsearch request</param>
        /// <returns>Elasticsearch response</returns>
        IZnodeSearchResponse FullTextSearch(IZnodeSearchRequest request);

        /// <summary>
        /// //Get elasticsearch response based on search request
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Return elasticsearch response /returns>
        ISearchResponse<dynamic> GetSearchResponse(IZnodeSearchRequest request);

        /// <summary>
        /// Get field value list
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="fieldType">fieldType</param>
        /// <returns>Return field value list</returns>
        List<string> FieldValueList(string indexName, string fieldType = "");

        /// <summary>
        /// Get Auto suggestion for boost and bury.
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="fieldName">fieldName</param>
        /// <param name="searchTerm">searchTerm</param>
        /// <param name="catalogVersionId">catalogVersionId</param>
        /// <param name="localeId">localeId</param>
        /// <returns>Return list</returns>
        List<string> GetBoostAndBuryAutoSuggestion(string indexName, int publishCatalogId, string fieldName, string searchTerm, int? catalogVersionId, int localeId);

        /// <summary>
        /// Method to check if elastic search is working
        /// </summary>
        /// <returns>Return status of elasticsearch</returns>
        string CheckElasticSearch();

        /// <summary>
        /// Get cms page count from elastic search count api
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Return search result count</returns>
        CountResponse GetCountSearchResponse(IZnodeSearchRequest request);

        /// <summary>
        /// Get default filters for portalId, locale ID, isActive, profileId, versionId
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Return filters</returns>
        List<QueryContainer> SetDefaultFilters(IZnodeCMSPageSearchRequest request);
    }
}
