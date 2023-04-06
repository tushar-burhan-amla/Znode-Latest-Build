using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICMSPageSearchService
    {
        #region CMS page search index

        /// <summary>
        /// Get list of search index monitor list.
        /// </summary>
        /// <param name="expands">Expands for list of search index monitor list.</param>
        /// <param name="filters">Filters for list of search index monitor list.</param>
        /// <param name="sorts">Sorts for list of search index monitor list.</param>
        /// <param name="page">Pagination for list of search index monitor list.</param>
        /// <returns>List of Cms Page search index server status list.</returns>
        CMSSearchIndexMonitorListModel GetCmsPageSearchIndexMonitorList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Insert CMS page search related data.
        /// </summary>
        /// <param name="cmsPortalContentPageIndex">Index related data.</param>
        /// <returns>CMS page index model.</returns>
        CMSPortalContentPageIndexModel InsertCreateCmsPageIndexData(CMSPortalContentPageIndexModel cmsPortalContentPageIndex);

        /// <summary>
        /// Insert CMS page search related data.
        /// </summary>
        /// <param name="cmsPortalContentPageIndex">Index related data.</param>
        /// <returns>CMS page index model.</returns>
        CMSPortalContentPageIndexModel InsertCreateCmsPageIndexDataByRevisionTypes(CMSPortalContentPageIndexModel cmsPortalContentPageIndex);

        /// <summary>
        /// Get CMS Pages Index data of the portal.
        /// </summary>
        /// <param name="expands">Expands for CMS Pages Index Data.</param>
        /// <param name="filters">Filters for CMS Pages Index Data.</param>
        /// <returns>CMS Page index data.</returns>
        CMSPortalContentPageIndexModel GetCmsPagesIndexData(NameValueCollection expands, FilterCollection filters);

        /// <summary>
        /// After content publish it creates index for CMS pages of store.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="targetPublishState">Revision type.</param>
        /// <param name="IsFromStorePublish">Is from store publish.</param>
        /// <param name="publishContent">Publish content</param>
        /// <returns></returns>        
        void CreateIndexForPortalCMSPages(int portalId, string targetPublishState, bool IsFromStorePublish = false, string publishContent = null);
        #endregion

        #region CMS page search request

        /// <summary>
        /// Get CMS full text result for the search keyword.
        /// </summary>
        /// <param name="model">Keyword search model.</param>
        /// <param name="filters">Filters required for search</param>
        /// <returns>Return cms page for search keyword</returns>
        CMSKeywordSearchModel FullTextContentPageSearch(CMSPageSearchRequestModel model, FilterCollection filters);

        /// <summary>
        /// Get CMS full text result count for the search keyword.
        /// </summary>
        /// <param name="model">Keyword search model</param>
        /// <returns>Count of cms pages for the search keyword</returns>
        int GetSearchContentPageCount(SearchRequestModel model);

        /// <summary>
        /// Create filters for cms page search
        /// </summary>
        /// <param name="model">SearchRequestModel</param>
        /// <returns>Filter colletion required for cms page search</returns>
        FilterCollection CreateFilterForCMSPages(SearchRequestModel model);
        #endregion
    }
}
