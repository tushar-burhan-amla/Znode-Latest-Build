namespace Znode.Engine.Api.Cache
{
    public interface IEcommerceCatalogCache
    {
        /// <summary>
        /// Get the list of all Publish Catalogs.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>List of PublishCatalog in string format by serializing it.</returns>
        string GetPublishCatalogList(string routeUri, string routeTemplate);

        /// <summary>
        /// Method to get catalogs associated with portal as per portalId.
        /// </summary>
        /// <param name="portalId">Portal ID to get associated catalog.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetAssociatedPortalCatalogByPortalId(int portalId, string routeUri, string routeTemplate);

        /// <summary>
        ///  Method Gets Portal catalog. 
        /// </summary>
        /// <param name="portalCatalogId">portalCatalogId.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPortalCatalog(int portalCatalogId, string routeUri, string routeTemplate);

        /// <summary>
        ///  Method Gets Publish Catalog Details
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId to get catalog details</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishCatalogDetails(int publishCatalogId, string routeUri, string routeTemplate);

        /// <summary>
        ///  Method Gets Publish Category Details
        /// </summary>
        /// <param name="publishCategoryId">publishCategoryId to get category details</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishCategoryDetails(int publishCategoryId, string routeUri, string routeTemplate);

        /// <summary>
        ///  Method Gets Publish Product Details
        /// </summary>
        /// <param name="publishProductId">publishProductId to get product details</param>
        /// <param name="portalId">portalId to get product inventory and pricing details</param> 
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishProductDetails(int publishProductId, int portalId, string routeUri, string routeTemplate);
    }
}