using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface ICatalogCache
    {
        /// <summary>
        /// Get a list of all catalogs.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCatalogs(string routeUri, string routeTemplate);

        /// <summary>
        /// Get catalog using catalogId.
        /// </summary>
        /// <param name="pimCatalogId">CatalogId use to retrieve catalog</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetCatalog(int pimCatalogId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get a list of categories which are associated to catalog.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetAssociatedCategories(string routeUri, string routeTemplate);

        /// <summary>
        /// Get a list of products which are associated to category.
        /// </summary>
        /// <param name="catalogAssociationModel">catalog Association Model having values for CatalogId CategoryId and LocaleId.</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format.</returns>
        string GetCategoryAssociatedProducts(CatalogAssociationModel catalogAssociationModel, string routeUri, string routeTemplate);

        /// <summary>
        /// Get details(Display order, active status, etc.)of category associated to catalog.
        /// </summary>
        /// <param name="catalogAssociateCategoryModel">Catalog Associate Category Model.</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format.</returns>
        string GetAssociateCategoryDetails(CatalogAssociateCategoryModel catalogAssociateCategoryModel, string routeUri, string routeTemplate);

        /// <summary>
        ///Get Catalog Publish Status.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Publish Catalog Log List Model</returns>
        PublishCatalogLogListModel GetCatalogPublishStatus(string routeUri, string routeTemplate);

        /// <summary>
        /// Get catalog details by catalog code.
        /// </summary>
        /// <param name="catalogCode">catalogcode</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>catalog details</returns>
        string GetCatalogByCatalogCode(string catalogCode, string routeUri, string routeTemplate);

    }
}