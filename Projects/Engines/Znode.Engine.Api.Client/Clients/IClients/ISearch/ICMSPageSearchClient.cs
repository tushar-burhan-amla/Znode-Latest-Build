using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ICMSPageSearchClient : IBaseClient
    {

        #region CMS Page search

        /// <summary>
        /// Insert Create index data.
        /// </summary>
        /// <param name="cmsPortalContentPageIndexModel">CMS Content Page index data.</param>
        /// <returns>CMS Page index model.</returns>
        CMSPortalContentPageIndexModel InsertCreateCmsPageIndexData(CMSPortalContentPageIndexModel cmsPortalContentPageIndex);

        /// <summary>
        /// Get search index of CMS page monitor list.
        /// </summary>
        /// <param name="filters">Filter for search index monitor list.</param>
        /// <param name="expands">Expands for search index monitor list.</param>
        /// <param name="sortCollection">Sort collection for search index monitor list.</param>
        /// <param name="page">Pagination values for search index monitor list.</param>
        /// <param name="recordPerPage">Page size for search index monitor list.</param>
        /// <returns>Search index monitor list of CMS pages.</returns>
        CMSSearchIndexMonitorListModel GetCmsPageSearchIndexMonitorList(FilterCollection filters, ExpandCollection expands, SortCollection sortCollection, int page, int recordPerPage);

        /// <summary>
        /// Get CMS pages index data.
        /// </summary>
        /// <param name="expands">Expands for CMS pages Index Data.</param>
        /// <param name="filters">Filters for CMS pages Index Data.</param>
        /// <returns>CMS page index data.</returns>
        CMSPortalContentPageIndexModel GetCmsPageIndexData(ExpandCollection expands, FilterCollection filters);
        #endregion

        #region CMS page search request
        /// <summary>
        ///  Get Cms pages based on keyword search by user
        /// </summary>
        /// <param name="keywordSearchModel">Model containing search keys and parameters.</param>
        /// <param name="filters">Filters for search.</param>
        /// <returns>Return KeywordSearchModel containing search result.</returns>
        CMSKeywordSearchModel FullTextContentPageSearch(CMSPageSearchRequestModel keywordSearchModel, FilterCollection filters);

        #endregion

    }
}
