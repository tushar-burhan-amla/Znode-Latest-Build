using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;

using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Search;

namespace Znode.Engine.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// Gets Index data of the portal.
        /// </summary>
        /// <param name="expands">Expands for Portal Index Data.</param>
        /// <param name="filters">Filters for portal Index Data.</param>
        /// <returns>Portal index data.</returns>
        PortalIndexModel GetCatalogIndexData(NameValueCollection expands, FilterCollection filters);
       
        /// <summary>
        /// Inserts search related data.
        /// </summary>
        /// <param name="portalIndexModel">Index related data.</param>
        /// <returns>Portal index model.</returns>
        PortalIndexModel InsertCreateIndexData(PortalIndexModel portalIndexModel);

        /// <summary>
        /// Creates search Index.
        /// </summary>
        /// <param name="indexName">Index Name.</param>
        /// <param name="revisionType">revisionType for preview or publish.</param>
        /// <param name="catalogId">Portal ID for which index is being made.</param>
        /// <param name="searchIndexMonitorId">Search Index monitor Id for which server status will be maintained while creating index</param>
        /// <param name="searchIndexServerStatusId">Search index server status ID.</param>
        /// <param name="newIndexName">A new name for an index</param>
        /// <param name="isPreviewProductionEnabled">To check whether preview production is enabled or not.</param>
        /// <param name="isPublishDraftProductsOnly">To specify whether to publish only draft products or not.</param>
        void CreateIndex(string indexName, string revisionType, int catalogId, int searchIndexMonitorId, int searchIndexServerStatusId, string newIndexName, bool isPreviewProductionEnabled, bool isPublishDraftProductsOnly);

        /// <summary>
        /// Deletes product data from search index which is removed from catalog.
        /// </summary>
        /// <param name="indexName">Index name from where products will be removed.</param>
        /// <param name="revisionType">revisionType for preview or publish.</param>
        /// <param name="catalogId">Catalog ID where the updated product data is present.</param>
        /// <param name="indexstartTime">current create index start time</param>
        /// <returns>Returns true if unused product data is removed successfully.</returns>
        bool DeleteProductData(string indexName, string revisionType, long indexstartTime);


        /// <summary>
        /// Get Seo Url Detail.
        /// </summary>
        /// <param name="seoUrl">SEO Url.</param>
        /// <param name="filters">Filter Collection.</param>
        /// <returns>Returns SEO Url details.</returns>
        SEOUrlModel GetSEOUrlDetails(string seoUrl, FilterCollection filters);

        /// <summary>
        /// Gets expands for products.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="expands"></param>
        /// <param name="searchResult"></param>
        /// <param name="tableDetails"></param>
        /// <param name="versionId"></param>
        void GetExpands(SearchRequestModel model, NameValueCollection expands, KeywordSearchModel searchResult, DataTable tableDetails = null, int? versionId = null);


        /// <summary>
        /// Get expands and add them to navigation properties
        /// </summary>
        /// <param name="expands"></param>
        /// <returns></returns>
        List<string> GetExpands(NameValueCollection expands);

        /// <summary>
        /// Converts a strings first character to lower-case.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string FirstCharToLower(string input);

        /// <summary>
        /// Saves the boost value of category/Product or Field.
        /// </summary>
        /// <param name="model">Boost data model.</param>
        /// <returns>True if values are saved successfully; False if values are not saved.</returns>
        bool SaveBoostVales(BoostDataModel model);

        /// <summary>
        /// Get list of global level product boost.
        /// </summary>
        /// <param name="expands">Expands for list of global level product boost.</param>
        /// <param name="filters">Filters for list of global level product boost.</param>
        /// <param name="sorts">Sorting for list of global level product boost.</param>
        /// <param name="page">Pagination for list of global level product boost.</param>
        /// <returns>List of global level product boost.</returns>
        SearchGlobalProductBoostListModel GetGlobalProductBoostList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of global level product category boost.
        /// </summary>
        /// <param name="expands">Expands for list of global level product category boost.</param>
        /// <param name="filters">Filters for list of global level product category boost.</param>
        /// <param name="sorts">Sorts for list of global level product category boost.</param>
        /// <param name="page">Pagination for list of global level product category boost.</param>
        /// <returns>List of global level product category boost.</returns>
        SearchGlobalProductCategoryBoostListModel GetGlobalProductCategoryBoostList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets list of boost for fields.
        /// </summary>
        /// <param name="expands">Expands for list of boost for fields.</param>
        /// <param name="filters">Filters for list of boost for fields.</param>
        /// <param name="sorts">Sorts for list of boost for fields.</param>
        /// <param name="page">Pagination for list of boost for fields.</param>
        /// <returns>List of boost for fields.</returns>
        SearchDocumentMappingListModel GetFieldBoostList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets full text result from the keyword.
        /// </summary>
        /// <param name="model">Keyword search model.</param>
        /// <param name="expands">Expands for search result</param>
        /// <param name="filters">Filters required for search</param>
        /// <param name="sorts">Sorts for search result.</param>
        /// <param name="page">Paging information for search result.</param>
        /// <returns>Keyword search model.</returns>
        KeywordSearchModel GetSearchProfileProducts(SearchProfileModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Searchable Attribute Of Profile
        /// </summary>
        /// <param name="searchableAttributesList"></param>
        /// <returns></returns>
        List<ElasticSearchAttributes> GetSearchableAttributeOfProfile(List<SearchAttributesModel> searchableAttributesList);


        /// <summary>
        /// Get Keyword Search Suggestion.
        /// </summary>
        /// <param name="model">Model containing search keys and parameters.</param>
        /// <param name="expands">Expands for search result.</param>
        /// <param name="sortCollection">Sorts for search result.</param>
        /// <param name="filters">Filters for search result.</param>
        /// <param name="page">Page for search result.</param>
        /// <returns>Returns KeywordSearchModel containing suggestions.</returns>
        KeywordSearchModel GetKeywordSearchSuggestion(SearchRequestModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets list of search index monitor list.
        /// </summary>
        /// <param name="expands">Expands for list of search index monitor list.</param>
        /// <param name="filters">Filters for list of search index monitor list.</param>
        /// <param name="sorts">Sorts for list of search index monitor list.</param>
        /// <param name="page">Pagination for list of search index monitor list.</param>
        /// <returns>List of search index server status list.</returns>
        SearchIndexMonitorListModel GetSearchIndexMonitorList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets list of search index server status.
        /// </summary>
        /// <param name="expands">Expands for list of search index server status.</param>
        /// <param name="filters">Filters for list of search index server status.</param>
        /// <param name="sorts">Sorts for list of search index server status.</param>
        /// <param name="page">Page for list of search index server status.</param>
        /// <param name="page">Page for list of search index server status.</param>
        /// <returns>list of search index server status.</returns>
        SearchIndexServerStatusListModel GetSearchIndexServerStatusList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Deletes boost value if it is removed.
        /// </summary>
        /// <param name="model">Boost data model</param>
        /// <returns>Bool value if the data is deleted or not.</returns>
        bool DeleteBoostValue(BoostDataModel model);

        #region Elastic search
        /// <summary>
        /// Gets full text result from the keyword.
        /// </summary>
        /// <param name="model">Keyword search model.</param>
        /// <param name="expands">Expands for search result</param>
        /// <param name="filters">Filters required for search</param>
        /// <param name="sorts">Sorts for search result.</param>
        /// <param name="page">Paging information for search result.</param>
        /// <returns>Keyword search model.</returns>
        KeywordSearchModel FullTextSearch(SearchRequestModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Bind Product Details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="expands"></param>
        /// <param name="searchResponse"></param>
        /// <param name="searchResult"></param>
        /// <param name="versionId"></param>
        void BindProductDetails(SearchRequestModel model, NameValueCollection expands, IZnodeSearchResponse searchResponse, KeywordSearchModel searchResult, int? versionId);

        /// <summary>
        /// Bind details of inventory to product list.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="expands"></param>
        /// <param name="searchResponse"></param>
        /// <param name="searchResult"></param>
        /// <param name="isSendAllLocations"></param>
        void BindInventoryDetails(SearchRequestModel model, NameValueCollection expands, IZnodeSearchResponse searchResponse, KeywordSearchModel searchResult, bool isSendAllLocations = false);

        /// <summary>
        /// Check whether indexing is allowed or not.
        /// </summary>
        /// <param name="publishCatalogId"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        bool IsAllowIndexing(int publishCatalogId, int? versionId);

        /// <summary>
        /// Get facet search result.
        /// </summary>
        /// <param name="model">Keyword search model.</param>
        /// <param name="expands">Expands for search result</param>
        /// <param name="filters">Filters required for search</param>
        /// <param name="sorts">Sorts for search result.</param>
        /// <param name="page">Paging information for search result.</param>
        /// <returns>Keyword search model.</returns>
        KeywordSearchModel FacetSearch(SearchRequestModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create the products in the index.
        /// </summary>
        /// <param name="indexName">Index name.</param>
        /// <param name="productEntities">Products to insert in search.</param>
        void CreateProduct(string indexName, List<ZnodePublishProductEntity> productEntities);

        /// <summary>
        /// Delete the product.
        /// </summary>
        /// <param name="indexName">Index name.</param>
        /// <param name="znodeProductIds">znodeProductIds to delete the products from index.</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteProduct(string indexName, string znodeProductIds, string revisionType);

        /// <summary>
        /// Delete the product.
        /// </summary>
        /// <param name="indexName">Index name.</param>
        /// <param name="znodeProductIds">znodeProductIds to delete the products from index.</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteProduct(string indexName, IEnumerable<object> znodeProductIds, string revisionType, string versionId);
        #endregion

        #region Synonyms
        /// <summary>
        /// Create synonyms for model.
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>Returns model with information.</returns>
        SearchSynonymsModel CreateSearchSynonyms(SearchSynonymsModel model);

        /// <summary>
        /// Get synonyms for search.
        /// </summary>
        /// <param name="searchSynonymsId">Uses id to retrieve data.</param>
        /// <param name="expands">Expands Collection</param>
        /// <returns>Returns model with information.</returns>
        SearchSynonymsModel GetSearchSynonyms(int searchSynonymsId, NameValueCollection expands);

        /// <summary>
        /// Update synonyms data for search.
        /// </summary>
        /// <param name="searchSynonymsModel">Uses search synonyms model with data.</param>
        /// <returns>Returns true if updated successfully else returns false.</returns>
        bool UpdateSearchSynonyms(SearchSynonymsModel searchSynonymsModel);

        /// <summary>
        /// Get synonyms list.
        /// </summary>
        /// <param name="expands">Expands for list</param>
        /// <param name="filters">Filters for list</param>
        /// <param name="sorts">Sorts for list of</param>
        /// <param name="page">Page for list of</param>
        /// <returns>Returns list of synonyms.</returns>
        SearchSynonymsListModel GetSearchSynonymsList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete synonyms by id.
        /// </summary>
        /// <param name="searchSynonymsIds">Uses synonyms ids</param>
        /// <returns>Returns true if deleted successfully else returns false.</returns>
        bool DeleteSearchSynonyms(ParameterModel searchSynonymsIds);
        #endregion

        #region Keywords Redirect
        /// <summary>
        /// Gets list of keywords & url.
        /// </summary>
        /// <param name="expands">Expands for list</param>
        /// <param name="filters">Filters for list</param>
        /// <param name="sorts">Sorts for list of</param>
        /// <param name="page">Page for list of</param>
        /// <returns>list of keywords</returns>
        SearchKeywordsRedirectListModel GetCatalogKeywordsRedirectList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create keywords and its redirected url for search.
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>Returns model with information.</returns>
        SearchKeywordsRedirectModel CreateSearchKeywordsRedirect(SearchKeywordsRedirectModel model);

        /// <summary>
        /// Get keywords for search.
        /// </summary>
        /// <param name="searchKeywordsRedirectId">Uses id to retrieve data.</param>
        /// <param name="expands">Expands Collection</param>
        /// <returns>Returns model with information.</returns>
        SearchKeywordsRedirectModel GetSearchKeywordsRedirect(int searchKeywordsRedirectId, NameValueCollection expands);

        /// <summary>
        /// Update keywords data for search.
        /// </summary>
        /// <param name="searchKeywordsModel">Uses search keywords model with data.</param>
        /// <returns>Returns true if updated successfully else returns false.</returns>
        bool UpdateSearchKeywordsRedirect(SearchKeywordsRedirectModel searchKeywordsModel);

        /// <summary>
        /// Delete keywords by id.
        /// </summary>
        /// <param name="searchKeywordsIds">Uses keywords ids</param>
        /// <returns>Returns true if deleted successfully else returns false.</returns>
        bool DeleteSearchKeywordsRedirect(ParameterModel searchKeywordsRedirectIds);

        /// <summary>
        /// Write synonyms.txt for search.
        /// </summary>
        /// <param name="publishCatalogId">publish catalog id</param>
        /// <param name="isSynonymsFile">if true create synonyms file else keyword file.</param>
        /// <param name="isAllDeleted">if true create synonyms file with blank values.</param>
        /// <returns>true or false.</returns>
        bool WriteSearchFile(int publishCatalogId, bool isSynonymsFile, bool isAllDeleted = false);

        /// <summary>
        /// Delete elastic search index
        /// </summary>
        /// <param name="catalogIndexId">catalog index id.</param>
        /// <returns>true or false</returns>
        bool DeleteIndex(int catalogIndexId);
        #endregion

        /// <summary>
        /// Delete category name in elastic search for given index
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        bool DeleteCategoryForGivenIndex(string indexName, int categoryId);

        /// <summary>
        ///  Delete catalog category product in elastic search for given index
        /// </summary>
        /// <param name="productEntities">Given Index</param>
        /// <param name="publishCatalogId">Publish Catalog Id</param>
        /// <param name="publishCategoryIds">Publish Category Ids i.e The main category contains subcategory as well.</param>
        /// <returns></returns>
        bool DeleteCatalogCategoryProducts(string indexName, int publishCatalogId, List<int> publishCategoryIds, string revisionType, string versionId);

        /// <summary>
        /// Get required input parameters to get the data of products.
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        DataTable GetProductFiltersForSP(List<dynamic> products);

        /// <summary>
        /// Get required indexName.
        /// </summary>
        /// <param name="cataLogId"></param>
        /// <returns></returns>
        string GetCatalogIndexName(int cataLogId);

        /// <summary>
        /// Inserts search related data by checking revision type.
        /// </summary>
        /// <param name="portalIndexModel">Index related data.</param>
        /// <returns>Portal index model.</returns>
        PortalIndexModel InsertCreateIndexDataByRevisionTypes(PortalIndexModel portalIndexModel);

        /// <summary>
        /// Rename index
        /// </summary>
        /// <param name="catalogIndexId"></param>
        /// <param name="oldIndexName"></param>
        /// <param name="newIndexName"></param>
        /// <returns></returns>
        bool RenameIndex(int catalogIndexId, string oldIndexName, string newIndexName);

        /// <summary>
        /// Clears synonyms form index when last synonym is deleted.
        /// </summary>
        /// <param name="searchSynonymsIds"></param>
        /// <returns></returns>
        bool ClearSearchSynonyms(ParameterModel searchSynonymsIds);

        /// <summary>
        /// Creates znode search request.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="isSuggestionList"></param>
        /// <param name="isAllowIndexing"></param>
        /// <returns></returns>
        IZnodeSearchRequest GetZnodeSearchRequest(SearchRequestModel model, FilterCollection filters, NameValueCollection sorts, bool isSuggestionList = false, bool isAllowIndexing = false);

        /// <summary>
        /// Gets the sort criteria.
        /// </summary>
        /// <param name="sortCollection"></param>
        /// <param name="categoryId"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        List<SortCriteria> GetSort(NameValueCollection sortCollection, int categoryId, string searchTerm);

        /// <summary>
        /// Gets expands for search.
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="request"></param>
        void GetFacetExpands(NameValueCollection expands, IZnodeSearchRequest request);

        /// <summary>
        /// Converts search response to keyword search model.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        KeywordSearchModel GetKeywordSearchModel(IZnodeSearchResponse response, IZnodeSearchRequest request = null);

        /// <summary>
        /// The method will return the list of search features which should not be displayed on the search profile screen.
        /// </summary>
        /// <returns>List of string of feature list</returns>
        List<string> GetExcludedSearchFeatureList();

        /// <summary>
        /// The method will return the list of all search query types which should not be displayed on the search profile screen.
        /// </summary>
        /// <returns>List of string of all query type list</returns>
        List<string> GetAllSearchQueryTypeList();

        /// <summary>
        /// The method will return the list of search query types which should not be displayed on the search profile screen.
        /// </summary>
        /// <returns>List of string of query type list</returns>
        List<string> GetExcludedSearchQueryTypeList();

        /// <summary>
        /// Get Search Profile Data
        /// </summary>
        /// <param name="model">Search Request Model</param>
        /// <param name="boostItems">List of Search Item Rule Model</param>
        /// <returns>Search Profile Model</returns>
        SearchProfileModel GetSearchProfileData(SearchRequestModel model, List<SearchItemRuleModel> boostItems);

        /// <summary>
        /// To get the Ids of categories with which products returned as a search result are associated.
        /// </summary>
        /// <param name="searchRequest">IZnodeSearchRequest</param>
        /// <param name="searchResult">KeywordSearchModel</param>
        /// <returns>Category Ids</returns>
        List<int> GetAssociatedCategoryIds(IZnodeSearchRequest searchRequest, KeywordSearchModel searchResult);

        /// <summary>
        /// This method will return the list of category ids based on the isProductInheritanceEnabled flag from store setting.
        /// If the flag is enabled, all categories including parent category ids will be returned else ids of categories directly associated with products will be returned.
        /// </summary>
        /// <param name="searchRequest">IZnodeSearchRequest</param>
        /// <param name="searchResult">KeywordSearchModel</param>
        /// <param name="isProductInheritanceEnabled">Boolean</param>
        /// <returns>Category Ids</returns>
        List<int> GetAssociatedCategoryIds(IZnodeSearchRequest searchRequest, KeywordSearchModel searchResult, bool isProductInheritanceEnabled);

        /// <summary>
        /// Updating Index After Search Profile Publish
        /// </summary>
        /// <param name="catalogId">Catalog Id Will be Used to Update The indexes</param>
        /// <returns>Returns true if index creation is successful else returns false.</returns>
        bool IndexCreationAfterSearchProfilePublish(int catalogId);

        /// <summary>
        /// To append the current timestamp in index name.
        /// </summary>
        /// <param name="indexName">index name</param>
        /// <returns>Returns updated index name</returns>
        string UpdateIndexNameWithTimestamp(string indexName);

        /// <summary>
        /// Get product details by sku.
        /// </summary>
        /// <param name="model">Search request model</param>
        /// <param name="expands">Name value collection for expands</param>
        /// <param name="filters">Filter collection</param>
        /// <param name="sorts">Name value collection for sorts</param>
        /// <param name="page">Name value collection for page</param>
        /// <returns>Published Product Data based on SKU</returns>
        PublishProductModel GetProductDetailsBySKU(SearchRequestModel model, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

    }
}