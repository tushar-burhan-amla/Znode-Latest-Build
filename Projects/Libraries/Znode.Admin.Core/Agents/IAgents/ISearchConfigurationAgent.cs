using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;
using System;

namespace Znode.Engine.Admin.Agents
{
    public interface ISearchConfigurationAgent
    {
        /// <summary>
        /// Gets Index data of the portal.
        /// </summary>/// 
        /// <param name="portalId">Portal ID for which index data will be retrieved.</param>
        /// <returns>Portal index data.</returns>
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because filters will be required along with publishCatalogId" +
        " Please use overload of this method which have filters and publishCatalogId as parameters ")]
        PortalIndexViewModel GetPortalIndexData(int portalId);

        /// <summary>
        /// Gets Index data of the portal against the filters passed to this method.
        /// </summary>
        /// <param name="filters">FilterCollection</param>
        /// <param name="portalId">Portal Id</param>
        /// <returns>PortalIndexViewModel</returns>
        PortalIndexViewModel GetPortalIndexData(FilterCollection filters, int portalId);

        /// <summary>
        /// Creates index for search.
        /// </summary>
        /// <param name="portalIndexViewModel">Portal index model.</param>
        /// <returns>Portal index model.</returns>
        PortalIndexViewModel InsertCreateIndexData(PortalIndexViewModel portalIndexViewModel);

        /// <summary>
        /// Saves the boost value of category/Product or Field.
        /// </summary>
        /// <param name = "boostData" > Boost data model.</param>
        /// <returns>True if values are saved successfully; False if values are not saved.</returns>
        bool SaveBoostValues(BoostDataViewModel boostData);

        /// <summary>
        /// Get global product boost list.
        /// </summary>
        /// <param name="expands">Expands for global product boost list.</param>
        /// <param name="filters">Filters for global product boost list.</param>
        /// <param name="sortCollection">Sorts for global product boost list.</param>
        /// <param name="page">Page for global product boost list.</param>
        /// <param name="recordPerPage">Page size for global product boost list.</param>
        /// <returns>Global product boost list.</returns>
        SearchGlobalProductBoostListViewModel GetGlobalProductBoostList(int catalogId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Get global product category boost list.
        /// </summary>
        /// <param name="expands">Expands for global product boost list.</param>
        /// <param name="filters">Filters for global product boost list.</param>
        /// <param name="sortCollection">Sorts for global product boost list.</param>
        /// <param name="page">Start page for global product boost list.</param>
        /// <param name="recordPerPage">Page Size for global product boost list.</param>
        /// <returns>Global product boost list.</returns>
        SearchGlobalProductCategoryBoostListViewModel GetGlobalProductCategoryBoostList(int catalogId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Gets field level boost list.
        /// </summary>
        /// <param name="expands">Expands for field level boost list.</param>
        /// <param name="filters">Filters for field level boost list.</param>
        /// <param name="sortCollection">Sorts for field level boost list.</param>
        /// <param name="page">Start page for field level boost list.</param>
        /// <param name="recordPerPage">Page size for field level boost list.</param>
        /// <returns>Field level boost list.</returns>
        SearchDocumentMappingListViewModel GetFieldLevelBoostList(int catalogId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Get search index monitor list.
        /// </summary>
        /// <param name="portalId">Portal ID.</param>
        /// <param name="expands">Expands for search index monitor list.</param>
        /// <param name="filters">Filters for search index monitor list.</param>
        /// <param name="sortCollection">Sorts for search index monitor list.</param>
        /// <param name="page">Start page for search index monitor list.</param>
        /// <param name="recordPerPage">Record per page of search index monitor list.</param>
        /// <returns>Search index monitor list.</returns>
        SearchIndexMonitorListViewModel GetSearchIndexMonitorList(int portalId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Gets search index server status list.
        /// </summary>
        /// <param name="searchIndexMonitorId">Search index monitor ID.</param>
        /// <param name="expands">Expands for search index server status list.</param>
        /// <param name="filters">Filters for search index server status list.</param>
        /// <param name="sortCollection">Sorts for search index server status list.</param>
        /// <param name="page">Pagination for search index server status list.</param>
        /// <param name="recordPerPage">Record per page for search index server status list.</param>
        /// <returns>Search index server status list.</returns>
        SearchIndexServerStatusListViewModel GetSearchIndexServerStatusList(int searchIndexMonitorId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Get all Store list.
        /// </summary>
        /// <param name="filters">Filters for store list.</param>
        /// <param name="sortCollection">Sorts for store list.</param>
        /// <param name="page">Pagination for store list.</param>
        /// <param name="recordPerPage">Record per page for store list.</param>
        /// <returns>store list.</returns>
        StoreListViewModel GetStoreList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = default(int?), int? pageSize = default(int?));

        /// <summary>
        ///  Get all publish catalog list.
        /// </summary>
        /// <param name="expands">Expands for catalog list.</param>
        /// <param name="filters">Filters for catalog list.</param>
        /// <param name="sortCollection">Sorts for catalog list.</param>
        /// <param name="page">Pagination for catalog list.</param>
        /// <param name="recordPerPage">Record per page for catalog list.</param>
        /// <returns>Publish catalog list.</returns>
        PortalCatalogListViewModel GetPublishCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage);

        #region Synonyms
        /// <summary>
        /// Create synonyms for search.
        /// </summary>
        /// <param name="viewModel">Uses model with data.</param>
        /// <returns>Returns model with synonyms information.</returns>
        SearchSynonymsViewModel CreateSearchSynonyms(SearchSynonymsViewModel viewModel);

        /// <summary>
        /// Get synonyms data for search.
        /// </summary>
        /// <param name="searchSynonymsId">Uses id to retrieve data.</param>
        /// <returns>Returns model with data.</returns>
        SearchSynonymsViewModel GetSearchSynonyms(int searchSynonymsId);

        /// <summary>
        /// Update synonyms data for search.
        /// </summary>
        /// <param name="viewModel">Uses viewModel with data.</param>
        /// <returns>Returns updated model with synonyms data.</returns>
        SearchSynonymsViewModel UpdateSearchSynonyms(SearchSynonymsViewModel viewModel);

        /// <summary>
        ///Get catalog synonyms list. 
        /// </summary>
        /// <param name="catalogId">catalog id.</param>
        /// <param name="expands">expands for synonyms list</param>
        /// <param name="filters">filters for synonyms list</param>
        /// <param name="sortCollection">sorts for synonyms list</param>
        /// <param name="page">page for synonyms list</param>
        /// <param name="recordPerPage">paging for synonyms list</param>
        /// <returns>Returns list of synonyms.</returns>
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because catalogId will be fetched from the filters" +
        " Please use overload of this method which does not have catalogId as parameters ")]
        SearchSynonymsListViewModel GetSearchSynonymsList(int catalogId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        ///Get catalog synonyms list. 
        /// </summary>
        /// <param name="expands">expands for synonyms list</param>
        /// <param name="filters">filters for synonyms list</param>
        /// <param name="sortCollection">sorts for synonyms list</param>
        /// <param name="page">page for synonyms list</param>
        /// <param name="recordPerPage">paging for synonyms list</param>
        /// <returns>Returns list of synonyms.</returns>
        SearchSynonymsListViewModel GetSearchSynonymsList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Delete synonyms by id.
        /// </summary>
        /// <param name="searchSynonymsIds">Uses synonyms ids.</param>
        /// <param name="publishcatalogId">Publish catalog id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteSearchSynonyms(string searchSynonymsIds, int publishcatalogId = 0);
        #endregion

        #region Keywords Redirect
        /// <summary>
        ///get catlog keywords list. 
        /// </summary>
        /// <param name="catalogId">catalog id.</param>
        /// <param name="expands">expands for keywords list</param>
        /// <param name="filters">filters for keywords list</param>
        /// <param name="sortCollection">sorts  for keywords list</param>
        /// <param name="page">page for keywords list</param>
        /// <param name="recordPerPage">paging for keywords list</param>
        /// <returns>Returns list of keywords and their urls.</returns>
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because catalogId will be fetched from the filters" +
        " Please use overload of this method which does not have catalogId as parameters ")]
        SearchKeywordsRedirectListViewModel GetCatalogKeywordsList(int catalogId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        ///get catlog keywords list. 
        /// </summary>
        /// <param name="expands">expands for keywords list</param>
        /// <param name="filters">filters for keywords list</param>
        /// <param name="sortCollection">sorts  for keywords list</param>
        /// <param name="page">page for keywords list</param>
        /// <param name="recordPerPage">paging for keywords list</param>
        /// <returns>Returns list of keywords and their urls.</returns>
        SearchKeywordsRedirectListViewModel GetCatalogKeywordsList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Create keywords for search.
        /// </summary>
        /// <param name="viewModel">Uses model with data.</param>
        /// <returns>Returns model with keywords information.</returns>
        SearchKeywordsRedirectViewModel CreateSearchKeywordsRedirect(SearchKeywordsRedirectViewModel viewModel);

        /// <summary>
        /// Get keywords data for search.
        /// </summary>
        /// <param name="searchSynonymsId">Uses id to retrieve data.</param>
        /// <returns>Returns model with data.</returns>
        SearchKeywordsRedirectViewModel GetSearchKeywordsRedirect(int searchKeywordsRedirectId);

        /// <summary>
        /// Update keywords data for search.
        /// </summary>
        /// <param name="viewModel">Uses viewModel with data.</param>
        /// <returns>Returns updated model with keywords data.</returns>
        SearchKeywordsRedirectViewModel UpdateSearchKeywordsRedirect(SearchKeywordsRedirectViewModel viewModel);

        /// <summary>
        /// Delete keywords by id.
        /// </summary>
        /// <param name="searchKeywordsRedirectIds">Uses keywords ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteSearchKeywordsRedirect(string searchKeywordsRedirectIds);

        /// <summary>
        ///  Write synonyms.txt for search.
        /// </summary>
        /// <param name="publishCataLogId">publish catalog id.</param>
        /// <param name="isSynonymsFile">if true create synonyms file else keyword file.</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>true or false.</returns>
        bool WriteSearchFile(int publishCataLogId,bool isSynonymsFile, out string errorMessage);

        /// <summary>
        /// Delete elastic search index
        /// </summary>
        /// <param name="catalogIndexId">catalog index id.</param>
        /// <param name="errorMessage">Error message if failed to delete index.</param>
        /// <returns>true or false</returns>
        bool DeleteIndex(int catalogIndexId, ref string errorMessage);
        #endregion

        #region CMS page search index
        /// <summary>
        /// Get CMS page search index monitor list.
        /// </summary>
        /// <param name="cmsSearchIndexId">CMSSearchIndex ID.</param>
        /// <param name="portalId">Portal ID.</param>
        /// <param name="expands">Expands for search index monitor list.</param>
        /// <param name="filters">Filters for search index monitor list.</param>
        /// <param name="sortCollection">Sorts for search index monitor list.</param>
        /// <param name="page">Start page for search index monitor list.</param>
        /// <param name="recordPerPage">Record per page of search index monitor list.</param>
        /// <returns>CMS page Search index monitor list.</returns>
        CMSSearchIndexMonitorListViewModel GetCmsPageSearchIndexMonitorList(int cmsSearchIndexId, int portalId, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int page, int recordPerPage);
        
        /// <summary>
        /// Create index for CMS page search.
        /// </summary>
        /// <param name="cmsPortalContentPageIndexViewModel">PortalContentPage index model.</param>
        /// <returns>PortalContentPage index model.</returns>
        CMSPortalContentPageIndexViewModel InsertCreateCmsPageIndexData(CMSPortalContentPageIndexViewModel cmsPortalContentPageIndexViewModel);        

        /// <summary>
        /// Get Index data of the of CMS pages.
        /// </summary> 
        /// <param name="portalId">Portal ID for which index data will be retrieved.</param>
        /// <returns>CMS pages content index data.</returns>
        CMSPortalContentPageIndexViewModel GetCmsPageIndexData(FilterCollection filters, int portalId);     

        #endregion
    }
}
