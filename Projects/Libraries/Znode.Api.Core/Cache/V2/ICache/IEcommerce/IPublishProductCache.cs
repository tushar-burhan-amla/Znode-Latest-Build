using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface IPublishProductCacheV2 : IPublishProductCache
    {
        /// <summary>
        /// Get Publish Products list based on attributes
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishProductsByAttribute(string routeUri, string routeTemplate);

        /// <summary>
        /// Get price for products through ajax async call.
        /// </summary>
        /// <param name="productPriceModel">parameters to get product price</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetProductPriceV2(ParameterInventoryPriceModel productPriceModel, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Publish Product 
        /// </summary>
        /// <param name="publishProductId">publish Product Id</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishProductV2(int publishProductId, string routeUri, string routeTemplate);
    }
}
