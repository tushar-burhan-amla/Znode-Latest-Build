using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ISearchClient : IBaseClient
    {
      
        /// <summary>
        /// Saves the boost value of category/Product or Field.
        /// </summary>
        /// <param name="boostDataModel">Boost data model.</param>
        /// <returns>True if values are saved successfully; False if values are not saved.</returns>
        bool SaveBoostValues(BoostDataModel boostDataModel);

        /// <summary>
        /// Gets portal index data.
        /// </summary>
        /// <param name="expands">Expands for Portal Index Data.</param>
        /// <param name="filters">Filters for portal Index Data.</param>
        /// <returns>Portal index data.</returns>
        PortalIndexModel GetCatalogIndexData(ExpandCollection expands, FilterCollection filters);

        /// <summary>
        /// Get Facet Search Result.
        /// </summary>
        /// <param name="keyWordSearchModel">Model containing search keys and parameters.</param>
        /// <param name="expands">Expands for search result.</param>
        /// <param name="filters">Filters for search.</param>
        /// <param name="sortCollection">Sorting for search result.</param>
        /// <returns>KeywordSearchModel containing search result.</returns>
        KeywordSearchModel FacetSearch(SearchRequestModel keywordSearchModel, ExpandCollection expandCollection, FilterCollection filters, SortCollection sortCollection);

        /// <summary>
        /// Inserts Create index data.
        /// </summary>
        /// <param name="portalIndex">Portal index data.</param>
        /// <returns>Portal index model.</returns>
        PortalIndexModel InsertCreateIndexData(PortalIndexModel portalIndex);

        /// <summary>
        /// Search Record Model.
        /// </summary>
        /// <param name="keyWordSearchModel">Model containing search keys and parameters.</param>
        /// <param name="expands">Expands for search result.</param>
        /// <param name="filters">Filters for search.</param>
        /// <param name="sortCollection">Sorting for search result.</param>
        /// <returns>KeywordSearchModel containing search result.</returns>
        KeywordSearchModel FullTextSearch(SearchRequestModel keywordSearchModel, ExpandCollection expandCollection, FilterCollection filters, SortCollection sortCollection);

        /// <summary>
        /// Creates index for search
        /// </summary>
        /// <param name="indexName">Index Name.</param>
        /// <param name="revisionType">revisionType for preview or publish.</param>
        /// <param name="catalogId">Catalog ID for which search index is being crated.</param>
        /// <param name="searchIndexMonitorId">search index monitor ID.</param>
        void CreateIndex(string indexName, string revisionType, int catalogId, int searchIndexMonitorId);

        /// <summary>
        /// Get SEO Url details on the basis of SEO Url.
        /// </summary>
        /// <param name="seoUrl">Seo Url.</param>
        /// <param name="filters">Filter Collection.</param>
        /// <returns>Returns SEO Url details in SEOUrlModel.</returns>
        SEOUrlModel GetSEOUrlDetails(string seoUrl, FilterCollection filters);

        /// <summary>
        /// Gets Global product level boost list.
        /// </summary>
        /// <param name="filters">Filter for Global product level boost list.</param>
        /// <param name="expands">Expands for Global product level boost list.</param>
        /// <param name="sortCollection">Sorts for Global product level boost list.</param>
        /// <param name="page">Page values for Global product level boost list.</param>
        /// <param name="recordPerPage">Page Size for Global product level boost list.</param>
        /// <returns>Global product level boost list.</returns>
        SearchGlobalProductBoostListModel GetGlobalProductBoostList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Gets Global level product category boost list.
        /// </summary>
        /// <param name="filters">Filters for Global level product category boost list.</param>
        /// <param name="expands">Expands for Global level product category boost list.</param>
        /// <param name="sortCollection">Sorts for Global level product category boost list.</param>
        /// <param name="page">Page for Global level product category boost list.</param>
        /// <param name="recordPerPage">Page size for Global level product category boost list.</param>
        /// <returns>Global level product category boost list.</returns>
        SearchGlobalProductCategoryBoostListModel GetGlobalProductCategoryBoostList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Gets field level boost list.
        /// </summary>
        /// <param name="filters">Filter for field level boost list.</param>
        /// <param name="expands">Expands for field level boost list.</param>
        /// <param name="sortCollection">Sorts for field level boost list.</param>
        /// <param name="page">Page for field level boost list.</param>
        /// <param name="recordPerPage">Page size for field level boost list.</param>
        /// <returns>Field level boost list.</returns>
        SearchDocumentMappingListModel GetFieldLevelBoostList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Get Keyword Search Suggestion.
        /// </summary>
        /// <param name="model">Model containing search keys and parameters.</param>
        /// <param name="expands">Expands for search result.</param>
        /// <param name="filters">Filters for search result.</param>
        /// <param name="sortCollection">Sorts for search result.</param>
        /// <param name="page">Page for search result.</param>
        /// <param name="recordPerPage">Page size for search result.</param>
        /// <returns>Returns KeywordSearchModel containing suggestions.</returns>
        KeywordSearchModel GetKeywordSearchSuggestion(SearchRequestModel model, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Get search index monitor list.
        /// </summary>
        /// <param name="filters">Filter for search index monitor list.</param>
        /// <param name="expands">Expands for search index monitor list.</param>
        /// <param name="sortCollection">Sort collection for search index monitor list.</param>
        /// <param name="page">Pagination values for search index monitor list.</param>
        /// <param name="recordPerPage">Page size for search index monitor list.</param>
        /// <returns>Search index monitor list.</returns>
        SearchIndexMonitorListModel GetSearchIndexMonitorList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Gets list of Search index server status list.
        /// </summary>
        /// <param name="filters">Filters for list of Search index server status list.</param>
        /// <param name="expands">Expands for list of Search index server status list.</param>
        /// <param name="sortCollection">Sort collection for list of Search index server status list.</param>
        /// <param name="page">Pagination for list of Search index server status list.</param>
        /// <param name="recordPerPage">Pagesize for list of Search index server status list.</param>
        /// <returns>List of Search index server status list.</returns>
        SearchIndexServerStatusListModel GetSearchIndexServerStatusList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Deletes the boost value of a product, product-category or field if it is removed.
        /// </summary>
        /// <param name="boostDataModel">Boost data model.</param>
        /// <returns>Bool value if the data is deleted or not.</returns>
        bool DeleteBoostValue(BoostDataModel boostDataModel);

        #region Synonyms
        /// <summary>
        /// Create new synonyms for search.
        /// </summary>
        /// <param name="model">Uses search synonyms model with data.</param>
        /// <returns>Returns synonyms model with data.</returns>
        SearchSynonymsModel CreateSearchSynonyms(SearchSynonymsModel model);

        /// <summary>
        /// Get synonyms details by synonyms id.
        /// </summary>
        /// <param name="searchSynonymsId">Uses id to retrieve data.</param>
        /// <param name="expands">Expands Collection.</param>
        /// <returns>Returns model with data.</returns>
        SearchSynonymsModel GetSearchSynonyms(int searchSynonymsId, ExpandCollection expands);

        /// <summary>
        /// Update synonyms data for search.
        /// </summary>
        /// <param name="searchSynonymsModel">Uses searchModel with data.</param>
        /// <returns>Returns updated model.</returns>
        SearchSynonymsModel UpdateSearchSynonyms(SearchSynonymsModel searchSynonymsModel);

        /// <summary>
        /// Get synonyms list.
        /// </summary>
        /// <param name="filters">Filters for search result.</param>
        /// <param name="expands">Expands for search result</param>
        /// <param name="sortCollection">Sorts for search result.</param>
        /// <param name="page">Page for search result</param>
        /// <param name="recordPerPage">Page size for search result.</param>
        /// <returns>Returns list of synonyms.</returns>
        SearchSynonymsListModel GetSearchSynonymsList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Delete synonyms by ids.
        /// </summary>
        /// <param name="SearchSynonymsId">Uses synonyms ids.</param>
        /// <returns>Returns true if deleted successfully else returns false.</returns>
        bool DeleteSearchSynonyms(ParameterModel searchSynonymsId);
        #endregion

        #region Keywords Redirect
        /// <summary>
        /// Get catalog keywords redirect list.
        /// </summary>
        /// <param name="model">Model containing search keys and parameters.</param>
        /// <param name="expands">Expands for search result.</param>
        /// <param name="filters">Filters for search result.</param>
        /// <param name="sortCollection">Sorts for search result.</param>
        /// <param name="page">Page for search result.</param>
        /// <param name="recordPerPage">Page size for search result.</param>
        /// <returns>List of keywords and url.</returns>
        SearchKeywordsRedirectListModel GetCatalogKeywordsRedirectList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Create new keywords and url for search.
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>Returns model with information.</returns>
        SearchKeywordsRedirectModel CreateSearchKeywordsRedirect(SearchKeywordsRedirectModel model);

        /// <summary>
        /// Get keywords details by keywords id.
        /// </summary>
        /// <param name="searchKeywordsRedirectId">Uses id to retrieve data.</param>
        /// <param name="expands">Expands Collection.</param>
        /// <returns>Returns model with data.</returns>
        SearchKeywordsRedirectModel GetSearchKeywordsRedirect(int searchKeywordsRedirectId, ExpandCollection expands);

        /// <summary>
        /// Update keywords data for search.
        /// </summary>
        /// <param name="searchKeywordsRedirectModel">Uses search keywords model with data.</param>
        /// <returns>Returns updated model.</returns>
        SearchKeywordsRedirectModel UpdateSearchKeywordsRedirect(SearchKeywordsRedirectModel searchKeywordsRedirectModel);

        /// <summary>
        /// Delete keywords by ids.
        /// </summary>
        /// <param name="searchKeywordsRedirectId">Uses keywords ids.</param>
        /// <returns>Returns true if deleted successfully else returns false.</returns>
        bool DeleteSearchKeywordsRedirect(ParameterModel searchKeywordsRedirectId);

        /// <summary>
        /// Write synonyms.txt for search.
        /// </summary>
        /// <param name="publishCatalogId">publish catalog id</param>
         /// <param name="isSynonymsFile">if true create synonyms file else keyword file.</param>
        /// <returns>true or false.</returns>
        bool WriteSearchFile(int publishCatalogId,bool isSynonymsFile);

        /// <summary>
        /// Delete elastic search index
        /// </summary>
        /// <param name="catalogIndexId">catalog index id.</param>
        /// <returns>true or false</returns>
        bool DeleteIndex(int catalogIndexId);
        #endregion
    }
}
