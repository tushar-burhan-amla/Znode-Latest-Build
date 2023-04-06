using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface IPublishProductCache
    {
        /// <summary>
        /// Get Publish Products 
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishProductList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Publish Product 
        /// </summary>
        /// <param name="publishProductId">publish Product Id</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishProduct(int publishProductId,string routeUri, string routeTemplate);

        /// <summary>
        /// Get Publish Product 
        /// </summary>
        /// <param name="parentProductId">Parent publish Product Id</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetParentProduct(int parentProductId, string routeUri, string routeTemplate); 

        /// <summary>
        /// Get only the brief details of a published product .
        /// </summary>
        /// <param name="publishProductId">publish Product Id</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetPublishProductBrief(int publishProductId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get only the details of a parent published product .
        /// </summary>
        /// <param name="publishProductId">publish Product Id</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetpublishParentProduct(int publishProductId, string routeUri, string routeTemplate);
        
		/// <summary>
		/// Get only the extended details of a published product .
		/// </summary>
		/// <param name="publishProductId">publish Product Id</param>
		/// <param name="routeUri">route uri.</param>
		/// <param name="routeTemplate">route template.</param>
		/// <returns></returns>
		string GetExtendedPublishProductDetails(int publishProductId, string routeUri, string routeTemplate);

		/// <summary>
		/// Get Publish Products 
		/// </summary>
		/// <param name="parameters">Parameter</param>
		/// <param name="routeUri">route uri.</param>
		/// <param name="routeTemplate">route template.</param>
		/// <returns>String Data.</returns>
		string GetPublishProductList(ParameterKeyModel parameters, string routeUri, string routeTemplate);


        /// <summary>
        /// Get Publish Products price and inventory.
        /// </summary>
        /// <param name="parameters">Parameter</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>String Data.</returns>
        string GetProductPriceAndInventory(ParameterInventoryPriceModel parameters, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Products price or inventory.
        /// </summary>
        /// <param name="parameters">Parameter</param>
        /// <param name="routeUri">route Uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>String Data.</returns>
        string GetPriceWithInventory(ParameterInventoryPriceListModel parameters, string routeUri, string routeTemplate);

        /// <summary>
        /// Get product by product sku.
        /// </summary>
        /// <param name="parameters">Product parameter model.</param>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Get product data.</returns>
        string GetPublishProductBySKU(ParameterProductModel parameters, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Configurable product.
        /// </summary>
        /// <param name="productAttributes"></param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Return product data.</returns>
        string GetConfigurableProduct(ParameterProductModel productAttributes, string routeUri, string routeTemplate);

        /// <summary>
        /// get bundle products
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>bundle product list</returns>
        string GetBundleProducts(string routeUri, string routeTemplate);

        /// <summary>
        /// Get product attribute by id.
        /// </summary>
        /// <param name="productId">Publish product id.</param>
        /// <param name="model">Parameter product model</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Return product attributes list.</returns>
        string GetProductAttribute(int productId, ParameterProductModel model, string routeUri, string routeTemplate);

        /// <summary>
        /// get group products
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>group product list</returns>
        string GetGroupProducts(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Publish Product excluding assigned ids.
        /// </summary>
        /// <param name="assignedIds">route uriassigned ids.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetUnAssignedPublishProductList(ParameterModel assignedIds, string routeUri, string routeTemplate);

        /// <summary>
        /// Get price for products through ajax async call.
        /// </summary>
        /// <param name="productPriceModel">parameters to get product price</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetProductPrice(ParameterInventoryPriceModel productPriceModel, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Publish Products .
        /// </summary>
        /// <param name="parameters">Parameter</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>String Data.</returns>
        string GetPublishProductForSiteMap(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Mongo Publish product count.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Total product count.</returns>
        string GetPublishProductCount(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Mongo Publish product list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Total product count.</returns>
        string GetPublishedProductsListData(string routeUri, string routeTemplate);

        /// <summary>
        /// Get inventory of product 
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetProductInventory(int publishProductId, string routeUri, string routeTemplate);


        /// <summary>
        /// Get associated configurable variants.
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route template</param>
        /// <returns>Configurable Variants</returns>
        string GetAssociatedConfigurableVariants(int productId, string routeUri, string routeTemplate);

    }
}
