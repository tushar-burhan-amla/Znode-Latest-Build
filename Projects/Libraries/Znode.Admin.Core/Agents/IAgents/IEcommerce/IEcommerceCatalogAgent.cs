using System.Collections.Generic;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IEcommerceCatalogAgent
    {
        /// <summary>
        /// Method to get catalogs associated with portal as per portal Id.
        /// </summary>
        /// <param name="portalId">Portal ID to get associated catalog</param>
        /// <param name="expands">Expands to be retrieved along with catalog list.</param>
        /// <param name="filters">Filters to be applied on catalog list.</param>
        /// <param name="sorts">Sorting to be applied on catalog list.</param>
        /// <param name="pageIndex">Start page index of catalog list.</param>
        /// <param name="pageSize">page size of catalog list.</param>
        /// <returns>Portal Catalog List View Model</returns>
        PortalCatalogListViewModel GetAssociatedPortalCatalogByPortalId(int portalId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Method Gets Portal catalog.
        /// </summary>
        /// <param name="portalCatalogId">portalCatalogId</param>
        /// <returns>Portal Catalog View Model</returns>
        PortalCatalogViewModel GetPortalCatalog(int portalCatalogId);

        /// <summary>
        /// Method Update catalog associated with portal.
        /// </summary>
        /// <param name="portalCatalogViewModel">Portal Catalog Model</param>
        /// <returns>True if updated successfully else returns false</returns>
        bool UpdatePortalCatalog(PortalCatalogViewModel portalCatalogViewModel);

        /// <summary>
        ///  Method Gets the tree structure for Catalog.
        /// </summary>
        /// <param name="catalogId">ID of catalog to get Catalog tree.</param>
        /// <param name="categoryId">ID of Category to get Catalog tree.</param>
        /// <returns>List of Category Tree ViewModel</returns>
        List<CategoryTreeViewModel> GetCatalogTree(int catalogId, int categoryId);

        /// <summary>
        ///Method Get Publish Catalog Details.
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId to get catalog details</param>
        /// <returns>PublishDetailsViewModel</returns>
        PublishDetailsViewModel GetPublishCatalogDetails(int publishCatalogId);

        /// <summary>
        /// Method Get Publish Category Details.
        /// </summary>
        /// <param name="publishCategoryId">publishCategoryId to get category details</param>
        /// <returns>PublishDetailsViewModel</returns>
        PublishDetailsViewModel GetPublishCategoryDetails(int publishCategoryId);

        /// <summary>
        /// Method Get Publish Product Details.
        /// </summary>
        /// <param name="publishProductId">publishProductId to get product details</param>
        /// <param name="portalId">portalId to get product inventory and pricing details</param>
        /// <returns>PublishDetailsViewModel</returns>
        PublishDetailsViewModel GetPublishProductDetails(int publishProductId, int portalId);
    }
}
