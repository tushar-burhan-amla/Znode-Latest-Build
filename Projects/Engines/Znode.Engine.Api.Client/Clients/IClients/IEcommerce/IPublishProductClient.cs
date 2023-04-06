using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IPublishProductClient : IBaseClient
    {
        /// <summary>
        /// Get publish product 
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <returns>Publish Product List Model</returns>
        PublishProductListModel GetPublishProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get publish products 
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Publish Product List Model</returns>
        PublishProductListModel GetPublishProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        ///  Get publish product 
        /// </summary>
        /// <param name="publishProductId">Publish Product Id</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Publish Product Model</returns>
        PublishProductModel GetPublishProduct(int publishProductId, FilterCollection filters, ExpandCollection expands);

        /// <summary>
        ///  Get only the extended details of a published product . 
        /// </summary>
        /// <param name="publishProductId">Publish Product Id</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Publish Product Model</returns>
        PublishProductDTO GetExtendedPublishProductDetails(int publishProductId, FilterCollection filters, ExpandCollection expands);

        /// <summary>
        ///  Get only the brief details of a published product .
        /// </summary>
        /// <param name="publishProductId">Publish Product Id</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Publish Product Model</returns>
        PublishProductModel GetPublishProductBrief(int publishProductId, FilterCollection filters, ExpandCollection expands);

        /// <summary>
        /// Get product inventory and price on the basis of sku.
        /// </summary>
        /// <param name="parameterModel">model with sku and portal id.</param>
        /// <returns>ProductInventoryPriceListModel</returns>
        ProductInventoryPriceListModel GetProductPriceAndInventory(ParameterInventoryPriceModel parameterModel);

        /// <summary>
        /// Get product inventory or price on the basis of sku.
        /// </summary>
        /// <param name="parameterModel">model with sku and portal id.</param>
        /// <returns>ProductInventoryPriceListModel</returns>
        ProductInventoryPriceListModel GetPriceWithInventory(ParameterInventoryPriceListModel parameterModel);

        /// <summary>
        /// Get publish catalogs 
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <param name="parameters">Parameter filter for published products.</param>
        /// <returns>Publish Product List Model</returns>
        PublishProductListModel GetPublishProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, ParameterKeyModel parameters);

        /// <summary>
        /// Get product by product sku.
        /// </summary>
        /// <param name="parameterModel">model with Sku of product.</param>
        /// <param name="expands">Expand for getting related data.</param>
        /// <param name="filters">Filter for product.</param>
        /// <returns>Returns Publish product model.</returns>
        PublishProductModel GetPublishProductBySKU(ParameterProductModel parameterModel, ExpandCollection expands, FilterCollection filters);

        /// <summary>
        /// Get configurable product model.
        /// </summary>
        /// <param name="parameterProductModel">Model with attributes data.</param>
        /// <param name="expands">Expands for related data</param>
        /// <returns>Model with product data</returns>
        PublishProductModel GetConfigurableProduct(ParameterProductModel parameterProductModel, ExpandCollection expands);

        /// <summary>
        /// Get parent product            
        /// </summary>
        /// <param name="parentProductId">Configurable Parent product Id.</param>
        /// <param name="filters">Filter for product.</param>
        /// <param name="expands">Expands for related data</param>
        /// <returns>Model with product data</returns>
        PublishProductModel GetParentProduct(int parentProductId, FilterCollection filters, ExpandCollection expands);


        /// <summary>
        /// Get Group Product list.
        /// </summary>
        /// <param name="filters">filter to have group product list</param>
        /// <returns>Return model with bundle product list.</returns>
        WebStoreGroupProductListModel GetGroupProductList(FilterCollection filters);

        /// <summary>
        /// Get bundle product list by product id.
        /// </summary>
        /// <param name="filters">Filter to get bundle products.</param>
        /// <returns>Bundle product list model.</returns>
        WebStoreBundleProductListModel GetBundleProducts(FilterCollection filters);

        /// <summary>
        /// Get product attribute by product id.
        /// </summary>
        /// <param name="productId">Publish product id.</param>
        /// <param name="parameterProductModel">Model with attributes data.</param>
        /// <returns>Product attribute list.</returns>
        ConfigurableAttributeListModel GetProductAttribute(int productId, ParameterProductModel parameterProductModel);

        /// <summary>
        /// Get publish product excluding assigned ids.
        /// </summary>
        /// <param name="assignedIds">already assigned ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Publish Product List Model</returns>
        PublishProductListModel GetUnAssignedPublishProductList(ParameterModel assignedIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Send Compare Product Mail.
        /// </summary>
        /// <param name="productCompareModel">Model with product data.</param>
        /// <returns>True/False</returns>
        bool SendComparedProductMail(ProductCompareModel productCompareModel);

        /// <summary>
        /// Send Email To a Friend.
        /// </summary>
        /// <param name="emailAFriendListModel">emailAFriendListModel</param>
        /// <returns></returns>
        bool SendEmailToFriend(EmailAFriendListModel emailAFriendListModel);

        /// <summary>
        /// Get price for products through ajax async call.
        /// </summary>
        /// <param name="productPrice">parameters to get product price.</param>
        /// <returns>ProductInventoryPriceListModel</returns>
        ProductInventoryPriceListModel GetProductPrice(ParameterInventoryPriceModel productPrice);

        /// <summary>
        ///  Get only the details of a parent published product .
        /// </summary>
        /// <param name="publishProductId">Publish Product Id</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Publish Product Model</returns>
        PublishProductModel GetpublishParentProduct(int publishProductId, FilterCollection filters, ExpandCollection expands);

        /// <summary>
        /// Get list of recently view active product
        /// </summary>
        /// <param name="parentIds">Parent Ids</param>
        /// <param name="catalogId">Catalog Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="versionId">Version Id</param>
        /// <returns></returns>
        List<RecentViewProductModel> GetActiveProducts(string parentIds, int catalogId, int localeId, int versionId);

        /// <summary>
        ///  Get product inventory
        /// </summary>
        /// <param name="publishProductId">Publish Product Id</param>
        /// <param name="filters">Filter collection.</param>        
        /// <returns>Product Inventory Details</returns>
        ProductInventoryDetailModel GetProductInventory(int publishProductId, FilterCollection filters);

        /// <summary>
        /// Get associated configurable product variants.
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <returns>Configurable Product Variants</returns>
        PublishProductListModel GetAssociatedConfigurableVariants(int productId);

        /// <summary>
        /// Submit Stock Request.
        /// </summary>
        /// <param name="stockNotificationModel">stockNotificationModel</param>
        /// <returns>status</returns>
        bool SubmitStockRequest(StockNotificationModel stockNotificationModel);

        /// <summary>
        /// Send stock notification.
        /// </summary>
        /// <param name="stockNotificationModel">stockNotificationModel</param>
        /// <returns>status</returns>
        bool SendStockNotification(StockNotificationModel stockNotificationModel);

        /// <summary>
        /// Get product details by sku. 
        /// </summary>
        /// <param name="searchRequestModel">search request model</param>
        /// <param name="filters">filters list</param>
        /// <param name="expands">expands list</param>
        /// <param name="sortCollection">sort collection</param>
        /// <returns>Published Product Based on SKU</returns>
        PublishProductModel GetProductDetailsBySKU(SearchRequestModel searchRequestModel, FilterCollection filters, ExpandCollection expands, SortCollection sortCollection);

    }
}
