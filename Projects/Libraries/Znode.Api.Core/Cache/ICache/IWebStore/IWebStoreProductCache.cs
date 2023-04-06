using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface IWebStoreProductCache
    {
        /// <summary>
        /// Get product list
        /// </summary>
        /// <param name="routeUri">route uri to get from cache</param>
        /// <param name="routeTemplate"></param>
        /// <returns>list of product</returns>
        string ProductList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="productId">publis product id</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>product data</returns>
        string GetProduct(int productId, string routeUri, string routeTemplate);

        /// <summary>
        /// get associated product list.
        /// </summary>
        /// <param name="productIDs">multiple product ids</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>list of product</returns>
        string GetAssociatedProducts(ParameterModel productIDs, string routeUri, string routeTemplate);

        /// <summary>
        /// Get product list by skus.
        /// </summary>
        /// <param name="skus">Multiple skus</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>List of product by skus</returns>
        string GetProductsBySkus(ParameterModel skus, string routeUri, string routeTemplate);

        /// <summary>
        /// Get product highlights.
        /// </summary>
        /// <param name="model">Model with parameters</param>
        /// <param name="productId">Product id.</param>
        /// <param name="localeId">locale id.</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Return product highlights list.</returns>
        string GetProductHighlights(ParameterProductModel model, int productId, int localeId, string routeUri, string routeTemplate);
    }
}