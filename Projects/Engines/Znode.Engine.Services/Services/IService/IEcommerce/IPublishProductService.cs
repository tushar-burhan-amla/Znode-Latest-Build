using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPublishProductService
    {
        /// <summary>
        /// Get publish product
        /// </summary>
        /// <param name="publishProductId">publish Product Id</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand collection</param>
        /// <returns>Publish Product Model</returns>
        PublishProductModel GetPublishProduct(int publishProductId, FilterCollection filters, NameValueCollection expands);


        /// <summary>
        /// To get active products for recent viewed products
        /// </summary>
        /// <param name="parentIds"></param>
        /// <param name="catalogId"></param>
        /// <param name="localeId"></param>
        /// <param name="versionId"></param>
        /// <returns></returns>
        List<RecentViewProductModel> GetActiveProducts(List<int> parentIds, int catalogId, int localeId, int versionId);
        

        /// <summary>
        /// This method only returns the brief details of a published product.
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="filters"></param>
        /// <param name="expands"></param>
        /// <returns></returns>
        PublishProductModel GetPublishProductBrief(int publishProductId, FilterCollection filters, NameValueCollection expands);
        /// <summary>
        /// Get publish product
        /// </summary>
        /// <param name="parentProductId">publish Product Id</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand collection</param>
        /// <returns>Publish Product Model</returns>
        PublishProductModel GetParentProduct(int parentProductId, FilterCollection filters, NameValueCollection expands);
                
        /// <summary>
        /// This method only returns the details of a parent published product.
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="filters"></param>
        /// <param name="expands"></param>
        /// <returns></returns>
        PublishProductModel GetpublishParentProduct(int publishProductId, FilterCollection filters, NameValueCollection expands);

        /// <summary>
        /// This method only returns the extended details of a published product based on the supplied expands.
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="filters"></param>
        /// <param name="expands">Pass appropriate expands to get the corresponding detail in response.</param>
        /// <returns></returns>
        PublishProductModel GetExtendedPublishProductDetails(int publishProductId, FilterCollection filters, NameValueCollection expands);

		/// <summary>
		/// Get published product
		/// </summary>
		/// <param name="expands">Expand Collection.</param>
		/// <param name="filters">Filter collection.</param>
		/// <param name="sorts">Sort Collection.</param>
		/// <param name="Page">paging parameters.</param>
		/// <returns>Publish Product List Model</returns>
		PublishProductListModel GetPublishProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get published product
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <param name="parameters">Parameter filter to be applied on product List</param>
        /// <returns>Publish Product List Model</returns>
        PublishProductListModel GetPublishProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, ParameterKeyModel parameters);

        /// <summary>
        ///Get product price and inventory by sku. 
        /// </summary>
        /// <param name="parameters">Model with sku and portal id.</param>
        /// <returns>ProductInventoryPriceListModel</returns>
        ProductInventoryPriceListModel GetProductPriceAndInventory(ParameterInventoryPriceModel parameters);

        /// <summary>
        ///Get product price or inventory by skus. 
        /// </summary>
        /// <param name="parameters">Model with sku and portal id.</param>
        /// <returns>ProductInventoryPriceListModel</returns>
        ProductInventoryPriceListModel GetPriceWithInventory(ParameterInventoryPriceListModel parameters);

        /// <summary>
        /// Get product by product sku.
        /// </summary>
        /// <param name="model">Model with parameters.</param>
        /// <param name="expands">Expands to have related data.</param>
        /// <param name="filters">Filter for product.</param> 
        /// <returns>Returns model with product details.</returns>
        PublishProductModel GetPublishProductBySKU(ParameterProductModel model, NameValueCollection expands, FilterCollection filters);

        /// <summary>
        /// Get Configurable product by attributes.
        /// </summary>
        /// <param name="productAttributes">Model with attributes data.</param>
        /// <param name="expands">Expands for related data.</param>
        /// <returns>Model with product data.</returns>
        PublishProductModel GetConfigurableProduct(ParameterProductModel productAttributes, NameValueCollection expands);

        /// <summary>
        /// Get bundle product list.
        /// </summary>
        /// <param name="filters">Filters to have bundle products.</param>
        /// <returns>Model with list of associated products.</returns>
        WebStoreBundleProductListModel GetBundleProducts(FilterCollection filters);

        /// <summary>
        /// Get associated attributes of product.
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <param name="parameterProductModel">Model with attributes data.</param>
        /// <returns>Returns attribute list.</returns>
        ConfigurableAttributeListModel GetProductAttribute(int productId, ParameterProductModel model);

        /// <summary>
        /// GEt Group Product list
        /// </summary>
        /// <param name="filters">Filters to have group products.</param>
        /// <returns>Model with list of associated products.</returns>
        WebStoreGroupProductListModel GetGroupProducts(FilterCollection filters);

        /// <summary>
        /// Get publish products excluding assigned Ids.
        /// </summary>
        /// <param name="assignedIds">assigned Ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>Publish product List Model</returns>
        PublishProductListModel GetUnAssignedPublishProductList(ParameterModel assignedIds, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Bind price related product data.
        /// </summary>
        /// <param name="productModel">ProductModel.</param>
        /// <param name="productId">Id of product.</param>
        /// <param name="publishedCatalogId">Id of published catalog.</param>
        /// <param name="portalId">Id of portal.</param>
        /// <param name="localeId">Id of locale.</param>
        /// <param name="productType">type of product.</param>
        void BindAssociatedProductDetails(ProductInventoryPriceModel productModel, int productId, int publishedCatalogId, int portalId, int localeId, string productType);

        /// <summary>
        /// Get price for products through ajax async call.
        /// </summary>
        /// <param name="parameter">parameters to get product price</param>
        /// <returns>ProductInventoryPriceListModel</returns>
        ProductInventoryPriceListModel GetProductPrice(ParameterInventoryPriceModel parameter);

        /// <summary>
        /// Get publish product
        /// </summary>
        /// <param name="publishProductId">publish Product Id</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand collection</param>
        /// <returns>Publish Product Model</returns>
        PublishProductListModel GetPublishProductForSiteMap(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Total Publish Product Count
        /// </summary>
        /// <param name="filters">Filter to get publish products count</param>
        /// <returns>Total product count.</returns>
        int GetPublishProductCount(FilterCollection filters);

        /// <summary>
        /// Get value from selected value
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeCode"></param>
        /// <returns></returns>
        string ValueFromSelectValue(List<PublishAttributeModel> attributes, string attributeCode);

        /// <summary>
        /// Get Product Filter For Stored Procedure
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        DataTable GetProductFiltersForSP(List<PublishProductModel> products);

        /// <summary>
        /// Get published Product List
        /// </summary>
        /// <param name="expands">Expand For Product List</param>
        /// <param name="filters">Filter to get publish products count</param>
        /// <param name="sorts">Sort for order by</param>
        /// <param name="page"></param>
        /// <returns>Publish Product List</returns>
        PublishProductListModel GetPublishedProductsListData(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Associated Group Product Price.
        /// </summary>
        /// <param name="productModel">productModel</param>
        /// <param name="productId">productId</param>
        /// <param name="portalId">portalId</param>
        /// <param name="localeId">localeId</param>
        void GetAssociatedGroupProductPrice(ProductInventoryPriceModel productModel, int productId, int portalId, int localeId);

        /// <summary>
        /// Get Associated Configurable Product.
        /// </summary>
        /// <param name="productId">productId</param>
        /// <param name="localeId">localeId</param>
        /// <param name="catalogVersionId">catalogVersionId</param>
        /// <param name="portalId">portalId</param>
        /// <returns></returns>
        PublishProductModel GetAssociatedConfigurableProduct(int productId, int localeId, int? catalogVersionId, int portalId);


        /// <summary>
        /// This method return all location inventory if flag is avaliable in expands
        /// </summary>
        /// <param name="ProductInventoryDetailModel"> Product inventory</param>
        /// <param name="filters"></param>
        /// <param name="expands"></param>
        /// <returns></returns>
        ProductInventoryDetailModel GetProductInventory(int publishProductId, FilterCollection filters, NameValueCollection expands);

        /// <summary>
        /// Get Configurable Product Entity
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="catalogVersionId"></param>
        /// <returns></returns>
        PublishedConfigurableProductEntityModel GetConfigurableProductEntity(int productId, int? catalogVersionId);

        /// <summary>
        /// Get Associated Configurable Products
        /// </summary>
        /// <param name="localeId"></param>
        /// <param name="catalogVersionId"></param>
        /// <param name="childSKU"></param>
        /// <returns></returns>
        List<PublishProductModel> GetAssociatedConfigurableProducts(int localeId, int? catalogVersionId, IEnumerable<string> childSKU);

        /// <summary>
        /// Get Clipart Value from Xml.
        /// </summary>
        /// <param name="decorationOption"></param>
        /// <param name="descendants"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        string GetClipartValue(string decorationOption, string descendants, string element);

        /// <summary>
        /// Get Group Product List
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="portalId"></param>
        /// <param name="localeId"></param>
        /// <param name="isInventory"></param>
        /// <returns></returns>
        List<WebStoreGroupProductModel> GetGroupProductList(FilterCollection filters, int portalId, string localeId, bool isInventory = false);

        /// <summary>
        /// Where Clause For PortalId.
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        string WhereClauseForPortalId(int portalId);

        /// <summary>
        /// Get Attribute value already exist.
        /// </summary>
        /// <param name="ConfigurableAttributeList"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool? AlreadyExist(List<ConfigurableAttributeModel> ConfigurableAttributeList, string value);

        /// <summary>
        /// Get Expand
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="localeId"></param>
        /// <param name="expands"></param>
        /// <param name="publishProductListModel"></param>
        /// <param name="catalogVersionId"></param>
        void GetExpands(int portalId, int localeId, NameValueCollection expands, PublishProductListModel publishProductListModel, int catalogVersionId = 0);

        /// <summary>
        /// Get Expand
        /// </summary>
        /// <param name="expands"></param>
        /// <returns></returns>
        List<string> GetExpands(NameValueCollection expands);

        /// <summary>
        /// Get message for group product.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        string GetGroupProductMessage(List<WebStoreGroupProductModel> list, ProductInventoryPriceModel product);

        /// <summary>
        /// Get associated configurable product variants.
        /// </summary>
        /// <param name="publishProductId">Published product Id</param>
        /// <returns>Configurable Product Variants</returns>
        List<WebStoreConfigurableProductModel> GetAssociatedConfigurableVariants(int publishProductId);

        /// <summary>
        /// Submit Stock Request.
        /// </summary>
        /// <param name="stockNotificationModel">stockNotificationModel</param>
        /// <returns>status</returns>
        bool SubmitStockRequest(StockNotificationModel stockNotificationModel);

        /// <summary>
        /// Send stock notification.
        /// </summary>
        /// <returns>status</returns>
        bool SendStockNotification();
    }
}
