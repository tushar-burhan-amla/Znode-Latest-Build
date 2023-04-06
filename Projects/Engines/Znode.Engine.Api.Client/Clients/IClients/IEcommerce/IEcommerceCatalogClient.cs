using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IEcommerceCatalogClient : IBaseClient
    {
        /// <summary>
        /// Get the list of all publish catalog.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>PublishCatalogListModel.</returns>
        PublishCatalogListModel GetPublishCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get catalogs associated with portal as per portalId.
        /// </summary>
        /// <param name="portalId">Portal ID to get associated catalog.</param>
        /// <param name="expands">Expands to be retrieved along with Category list.</param>
        /// <param name="filters">Filters to be applied on Category list.</param>
        /// <param name="sorts">Sorting to be applied on Category list.</param>
        /// <returns>Portal Catalog ListModel.</returns>
        PortalCatalogListModel GetAssociatedPortalCatalogByPortalId(int portalId, ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get catalogs associated with portal as per portalId.
        /// </summary>
        /// <param name="portalId">Portal ID to get associated catalog.</param>
        /// <param name="expands">Expands to be retrieved along with Category list.</param>
        /// <param name="filters">Filters to be applied on Category list.</param>
        /// <param name="sorts">Sorting to be applied on Category list.</param>
        /// <param name="pageIndex">Start page index of Category list.</param>
        /// <param name="pageSize">page size of Category list.</param>
        /// <returns>Portal Catalog ListModel.</returns>
        PortalCatalogListModel GetAssociatedPortalCatalogByPortalId(int portalId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get catalogs associated with portal.
        /// </summary>
        /// <param name="filterIds">Filter to be applied on CatalogList.</param>
        /// <returns>Portal Catalog ListModel.</returns>
        PortalCatalogListModel GetAssociatedPortalCatalog(ParameterModel filterIds);

        /// <summary>
        /// Update catalog associated with portal.
        /// </summary>
        /// <param name="portalCatalogModel">Portal Catalog Model.</param>
        /// <returns>Portal Catalog Model.</returns>
        PortalCatalogModel UpdatePortalCatalog(PortalCatalogModel portalCatalogModel);

        /// <summary>
        /// Get Portal catalog.
        /// </summary>
        /// <param name="portalCatalogId">portalCatalogId</param>
        /// <returns>PortalCatalogModel</returns>
        PortalCatalogModel GetPortalCatalog(int portalCatalogId);

        /// <summary>
        /// Get the tree structure for Catalog.
        /// </summary>
        /// <param name="catalogId">ID of catalog to get Catalog tree.</param>
        /// <param name="categoryId">ID of Category to get Category tree.</param>
        /// <returns>List of CategoryTreeModel</returns>
        List<CategoryTreeModel> GetCatalogTree(int catalogId, int categoryId);

        /// <summary>
        /// Get Publish Catalog Details
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId to get catalog details</param>
        /// <returns>PublishCatalogModel</returns>
        PublishCatalogModel GetPublishCatalogDetails(int publishCatalogId);

        /// <summary>
        /// Get Publish Category Details
        /// </summary>
        /// <param name="publishCategoryId">publishCategoryId to get category details</param>
        /// <returns>PublishCategoryModel</returns>
        PublishCategoryModel GetPublishCategoryDetails(int publishCategoryId);

        /// <summary>
        /// Get Publish Product Details
        /// </summary>
        /// <param name="publishProductId">publishProductId to get product details</param>
        /// <param name="portalId">portalId to get product inventory and pricing details</param>
        /// <returns>PublishProductModel</returns>
        PublishProductModel GetPublishProductDetails(int publishProductId, int portalId);
    }
}
