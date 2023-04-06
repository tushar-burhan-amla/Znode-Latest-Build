using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.V2;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPublishProductServiceV2 : IPublishProductService
    {
        /// <summary>
        /// Get published product by attributes
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>Publish Product List Model</returns>
        PublishProductListModel GetPublishProductsByAttribute(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get price for products through ajax async call.
        /// </summary>
        /// <param name="parameter">parameters to get product price</param>
        /// <returns>ProductInventoryPriceListModel</returns>
        ProductInventoryPriceListModel GetProductPriceV2(ParameterInventoryPriceModel parameter);

        /// <summary>
        /// Get publish product 
        /// </summary>
        /// <param name="publishProductId">publish Product Id</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand collection</param>
        /// <returns>Publish Product Model</returns>
        PublishProductModelV2 GetPublishProductV2(int publishProductId, FilterCollection filters, NameValueCollection expands);

        /// <summary>
        /// Get the list of published products for V2 Apis
        /// </summary>
        /// <param name="expands">Expand collection</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">Paging Parameters.</param>
        /// <param name="requiredAttrFilter">Required Attribute Filter</param>
        /// <returns>Publish Product List Model</returns>
        PublishProductListModel GetPublishProductListV2(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, string requiredAttrFilter);

        /// <summary>
        /// Get publish product by SKU
        /// </summary>
        /// <param name="model">Comma separated SKUs</param>
        /// <param name="expands">Expand collection</param>
        /// <param name="filters">Filter collection</param>
        /// <returns>Publish Product Model</returns>
        PublishProductModel GetPublishProductBySkuV2(ParameterProductModel model, NameValueCollection expands, FilterCollection filters);
    }
}
