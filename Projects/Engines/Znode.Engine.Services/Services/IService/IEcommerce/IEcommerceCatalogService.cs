using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IEcommerceCatalogService
    {
        /// <summary>
        /// Get the list of all PublishCatalogs.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>PublishCatalogList Model.</returns>
        PublishCatalogListModel GetPublishCatalogList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Method to get catalogs associated with portal as per portalId.
        /// </summary>
        /// <param name="portalId">Portal ID to get associated catalog.</param>
        /// <param name="expands">Expands to be retrieved along with catalog list.</param>
        /// <param name="filters">Filters to be applied on catalog list.</param>
        /// <param name="sorts">Sorting to be applied on catalog list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Portal Catalog ListModel</returns>
        PortalCatalogListModel GetAssociatedPortalCatalogByPortalId(int portalId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Method to get catalogs associated with portal.
        /// </summary>
        /// <param name="filterIds">Filter to be applied on catalog list</param>
        /// <returns>Portal Catalog ListModel</returns>
        PortalCatalogListModel GetAssociatedPortalCatalog(ParameterModel filterIds);

        /// <summary>
        ///Method Update catalog associated with portal.
        /// </summary>
        /// <param name="portalCatalogModel">Portal Catalog Model.</param>
        /// <returns>Portal Catalog Model</returns>
        bool UpdatePortalCatalog(PortalCatalogModel portalCatalogModel);

        /// <summary>
        ///Method Gets Portal catalog.
        /// </summary>
        /// <param name="portalCatalogId">ID of portalCatalog.</param>
        /// <returns>PortalCatalogModel</returns>
        PortalCatalogModel GetPortalCatalog(int portalCatalogId);

        /// <summary>
        ///Method Gets the tree structure for Catalog.
        /// </summary>
        /// <param name="catalogId">ID of catalog to get Catalog tree.</param>
        /// <param name="categoryId">ID of Category to get Catalog tree.</param>
        /// <returns>retruns List of CategoryTreeModel</returns>
        List<CategoryTreeModel> GetCatalogTree(int catalogId, int categoryId);

        /// <summary>
        ///Method Gets Publish Catalog Details
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId to get catalog details</param>
        /// <returns>PublishCatalogModel</returns>
        PublishCatalogModel GetPublishCatalogDetails(int publishCatalogId);

        /// <summary>
        ///Method Gets Publish Category Details
        /// </summary>
        /// <param name="publishCategoryId">publishCategoryId to get category details</param>
        /// <returns>PublishCategoryModel</returns>
        PublishCategoryModel GetPublishCategoryDetails(int publishCategoryId);

        /// <summary>
        ///Method Gets Publish Product Details
        /// </summary>
        /// <param name="publishProductId">publishProductId to get product details</param>
        /// <param name="portalId">portalId to get product inventory and pricing details</param>  
        /// <returns>PublishProductModel</returns>
        PublishProductModel GetPublishProductDetails(int publishProductId, int portalId);
    }
}
