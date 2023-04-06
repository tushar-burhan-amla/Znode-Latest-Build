using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ISiteMapClient:IBaseClient
    {
        /// <summary>
        /// Get Category List for sitemap.
        /// </summary>
        /// <param name="includeAssociatedCategories">This flag is used to fetch the child categories.</param>
        /// <param name="expands">Expands to be retrieved along with the list.</param>
        /// <param name="filters">Filters for associated item.</param>
        /// <param name="sorts">Sorts for associated item.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns SiteMapCategory List Model.</returns>
        SiteMapCategoryListModel GetSitemapCategoryList(bool includeAssociatedCategories, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Brand List for sitemap.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with the list.</param>
        /// <param name="filters">Filters for associated item.</param>
        /// <param name="sorts">Sorts for associated item.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns SiteMapCategory List Model.</returns>
        SiteMapBrandListModel GetSitemapBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Publish Product List.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with the list.</param>
        /// <param name="filters">Filters for associated item.</param>
        /// <param name="sorts">Sorts for associated item.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns SiteMapProduct List Model.</returns>
        SiteMapProductListModel GetSitemapProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get product feed by portal Id.
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <returns>ProductFeedModel</returns>
        List<ProductFeedModel> GetProductFeedByPortalId(int portalId);

    }
}
