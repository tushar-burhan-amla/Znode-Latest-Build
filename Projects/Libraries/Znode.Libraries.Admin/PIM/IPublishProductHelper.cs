using System.Collections.Generic;
using System.Data;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.Admin
{
    public interface IPublishProductHelper
    {
        /// <summary>
        /// GetAddOnsData
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="configurableProductId"></param>
        /// <param name="portalId"></param>
        /// <param name="localeId"></param>
        /// <param name="catalogVersionId"></param>
        /// <param name="userId"></param> 
        /// <param name="publishCatalogId"></param>
        /// <param name="profileId"></param>
        /// <param name="omsOrderId"></param>
        /// <returns>WebStoreAddOnModel</returns>
        List<WebStoreAddOnModel> GetAddOnsData(int publishProductId, int configurableProductId, int portalId, int localeId, int? catalogVersionId, int userId, int publishCatalogId, int profileId = 0, int omsOrderId = 0);

        /// <summary>
        /// Get Associated product for configurable type.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="localeId"></param>
        /// <param name="catalogVersionId"></param>
        /// <param name="configEntity"></param>
        /// <returns>List of PublishProductModel</returns>
        List<PublishProductModel> GetAssociatedProducts(int productId, int localeId, int? catalogVersionId, List<PublishedConfigurableProductEntityModel> configEntity);

        /// <summary>
        /// Get brand data for products.
        /// </summary>
        /// <param name="publishProduct"></param>
        /// <param name="catalogVersionId"></param>
        void GetBrandDataForProduct(PublishProductModel publishProduct, int? catalogVersionId = null);

        /// <summary>
        /// Get Portal brand data for products.
        /// </summary>
        /// <param name="publishProduct">Publish Product Model</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="webstoreVersionId">Webstore Version Id</param>
        void GetBrandDataForProduct(PublishProductModel publishProduct, int portalId, int? webstoreVersionId);

        /// <summary>
        /// Get current catalog version id by catalog id.
        /// </summary>
        /// <param name="publishCatalogId"></param>
        /// <param name="localeId"></param>
        /// <returns>Version Id</returns>
        int GetCatalogVersionId(int publishCatalogId, int localeId = 0);

        /// <summary>
        /// Get configurable product variants.
        /// </summary>
        /// <param name="productList"></param>
        /// <param name="ConfigurableAttributeCodes"></param>
        /// <returns>List of List<PublishAttributeModel></returns>
        List<List<PublishAttributeModel>> GetConfigurableAttributes(List<PublishProductModel> productList, List<string> ConfigurableAttributeCodes = null);

        /// <summary>
        /// Get promotional price of product if any promotion associated to it.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="catalogVersionId"></param>
        /// <returns>List of ConfigurableProductEntity</returns>
        List<PublishedConfigurableProductEntityModel> GetConfigurableProductEntity(int productId, int? catalogVersionId);

        /// <summary>
        /// Get promotional price of product if any promotion associated to it.
        /// </summary>
        /// <param name="productIds"></param>
        /// <param name="catalogVersionId"></param>
        /// <returns>List of PublishedConfigurableProductEntityModel</returns>
        List<PublishedConfigurableProductEntityModel> GetConfigurableProductEntity(List<int> productIds, int? catalogVersionId);

        /// <summary>
        /// get products associated to categories from expands
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="navigationProperties"></param>
        /// <param name="searchProducts"></param>
        /// <param name="localeId"></param>
        /// <param name="userId"></param>
        /// <param name="catalogVersionId"></param>
        /// <param name="profileId"></param>
        void GetDataFromExpands(int portalId, List<string> navigationProperties, List<SearchProductModel> searchProducts, int localeId, int userId = 0, int catalogVersionId = 0, int profileId = 0);

        /// <summary>
        /// get products associated to categories from expands
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="navigationProperties"></param>
        /// <param name="publishProductListModel"></param>
        /// <param name="localeId"></param>
        /// <param name="userId"></param>
        /// <param name="catalogVersionId"></param>
        void GetDataFromExpands(int portalId, List<string> navigationProperties, PublishProductListModel publishProductListModel, int localeId, int userId = 0, int catalogVersionId = 0, int profileId = 0);

        /// <summary>
        /// get expands associated to Product
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="navigationProperties"></param>
        /// <param name="publishProduct"></param>
        /// <param name="localeId"></param>
        /// <param name="whereClause"></param>
        /// <param name="userId"></param>
        /// <param name="catalogVersionId"></param>
        /// <param name="webstoreVersionId"></param>
        /// <param name="profileId"></param>
        /// <param name="omsOrderId"></param>
        void GetDataFromExpands(int portalId, List<string> navigationProperties, PublishProductModel publishProduct, int localeId, string whereClause = "", int userId = 0, int? catalogVersionId = null, int? webstoreVersionId = null, int profileId = 0, int omsOrderId = 0);

        /// <summary>
        /// Get Inventory Associated to SKU.
        /// </summary>
        /// <param name="skus"></param>
        /// <param name="portalId"></param>
        /// <returns>List of InventorySKUModel</returns>
        List<InventorySKUModel> GetInventoryBySKUs(IEnumerable<string> skus, int portalId);

        /// <summary>
        /// Get Pricing Associated to SKU.
        /// </summary>
        /// <param name="skus"></param>
        /// <param name="portalId"></param>
        /// <param name="userId"></param>
        /// <param name="profileId"></param>
        /// <param name="omsOrderId"></param>
        /// <returns>List of PriceSKUModel</returns>
        List<PriceSKUModel> GetPricingBySKUs(IEnumerable<string> skus, int portalId, int userId = 0, int profileId = 0, int omsOrderId = 0, bool isBundleProduct = false, string bundleProductParentSKU = "");
        /// <summary>
        /// Get product customer reviews.
        /// </summary>
        /// <param name="publishProduct"></param>
        /// <param name="portalId"></param>
        void GetProductCustomerReviews(PublishProductModel publishProduct, int portalId);

        /// <summary>
        /// Get Product Inventory details.
        /// </summary>
        /// <param name="publishProduct"></param>
        /// <param name="portalId"></param>
        void GetProductInventory(PublishProductModel publishProduct, int portalId);

        /// <summary>
        /// Get Template associate to product detail page.
        /// </summary>
        /// <param name="publishProduct"></param>
        /// <param name="portalId"></param>
        /// <param name="webstoreVersionId"></param>
        void GetProductPageTemplate(PublishProductModel publishProduct, int portalId, int? webstoreVersionId = null);

        /// <summary>
        /// Get Product price data by skus.
        /// </summary>
        /// <param name="publishProduct"></param>
        /// <param name="portalId"></param>
        /// <param name="userId"></param>
        /// <param name="profileId"></param>
        /// <param name="orderId"></param>
        void GetProductPriceData(PublishProductModel publishProduct, int portalId, int userId, int profileId, int omsOrderId = 0);

        /// <summary>
        /// Get Promotions for product.
        /// </summary>
        /// <param name="publishProduct"></param>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        void GetProductPromotions(PublishProductModel publishProduct, int userId = 0, int portalId = 0);

        /// <summary>
        /// This is use for Get Product Promotions For Products
        /// </summary>
        /// <param name="productList"></param>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        void GetProductPromotionsForProductList(List<SearchProductModel> productList, int userId = 0, int portalId = 0);

        /// <summary>
        /// This is use for Get Product Promotions For Products
        /// </summary>
        /// <param name="publishProductListModel"></param>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        void GetProductPromotionsForProductList(PublishProductListModel publishProductListModel, int userId = 0, int portalId = 0);

        /// <summary>
        /// This is use for get product reviews 
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <param name="portalId"></param>
        /// <returns>List of CustomerReviewModel</returns>
        List<CustomerReviewModel> GetProductReviews(int publishProductId, int portalId);

        /// <summary>
        /// This method is use to Get ProductsSeo and Reviews.
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="productList"></param>
        /// <param name="localeId"></param>
        void GetProductsSEOAndReviews(int portalId, List<SearchProductModel> productList, int localeId, int catalogVersionId);

        /// <summary>
        /// This method is use to Get ProductsSeo and Reviews.
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="publishProductListModel"></param>
        /// <param name="localeId"></param>
        /// <param name="catalogVersionId"></param>
        void GetProductsSEOAndReviews(int portalId, PublishProductListModel publishProductListModel, int localeId, int? catalogVersionId = null);

        /// <summary>
        /// This method is use to get Products SEO Details.
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="publishProductListModel"></param>
        /// <param name="localeId"></param>
        /// <param name="catalogVersionId"></param>
        void GetProductsSEODetails(int portalId, PublishProductListModel publishProductListModel, int localeId, int? catalogVersionId = null);

        /// <summary>
        /// This method is use to Get Promotion By Publish ProductIds
        /// </summary>
        /// <param name="publishProductIds"></param>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        /// <returns>LIst of ProductPromotionModel</returns>
        List<ProductPromotionModel> GetPromotionByPublishProductIds(IEnumerable<int> publishProductIds, int userId = 0, int portalId = 0);


        /// <summary>
        /// This method is use to Get SEO Details For List
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="seoTypeName"></param>
        /// <returns>List of SEODetailsModel</returns>
        List<SEODetailsModel> GetSEODetailsForList(int portalId, string seoTypeName);

        /// <summary>
        /// This method is use to Get Tax Class Id
        /// </summary>
        /// <param name="sKU"></param>
        /// <param name="countryCode"></param>
        /// <returns>TaxClassId</returns>
        int GetTaxClassId(string sKU, string countryCode);

        /// <summary>
        /// This method is use to Get Tax Rules.
        /// </summary>
        /// <param name="sKU"></param>
        /// <returns>TaxClassId</returns>
        List<TaxClassRuleModel> GetTaxRules(List<string> sKUs);

        /// <summary>
        /// This method is use to Map Configurable Attribute Data
        /// </summary>
        /// <param name="attributeList"></param>
        /// <param name="products"></param>
        /// <returns>List of PublishAttributeModel</returns>
        List<PublishAttributeModel> MapConfigurableAttributeData(List<List<PublishAttributeModel>> attributeList, List<PublishedProductEntityModel> products);

        /// <summary>
        /// This is method is use to Map Inventory.
        /// </summary>
        /// <param name="productList"></param>
        /// <param name="inventory"></param>
        void MapInventory(List<SearchProductModel> productList, List<InventorySKUModel> inventory);

        /// <summary>
        /// This is method is use to Map Inventory.
        /// </summary>
        /// <param name="publishProductListModel"></param>
        /// <param name="inventory"></param>
        void MapInventory(PublishProductListModel publishProductListModel, List<InventorySKUModel> inventory);

        /// <summary>
        /// This method is use to Map Price.
        /// </summary>
        /// <param name="productList"></param>
        /// <param name="priceSKU"></param>
        void MapPrice(List<SearchProductModel> productList, List<PriceSKUModel> priceSKU);

        /// <summary>
        /// This method is use to Map Price.
        /// </summary>
        /// <param name="publishProductListModel"></param>
        /// <param name="priceSKU"></param>
        void MapPrice(PublishProductListModel publishProductListModel, List<PriceSKUModel> priceSKU);

        /// <summary>
        /// Get Cart Line Item Data
        /// </summary>
        /// <param name="sku">SKU list</param>
        /// <param name="catalogId">Catalog id</param>
        /// <param name="localeId">locale id</param>
        /// <param name="navigationProperties">list of expands</param>
        /// <param name="userId">user id</param>
        /// <param name="portalId">portal id</param>
        /// <param name="versionId">catalog version id</param>
        /// <param name="includeInactiveProduct">This flag is used to consider inactive products</param>
        /// <param name="omsOrderId">order id</param>
        /// <returns>List of publish product.</returns>
        List<PublishProductModel> GetDataForCartLineItems(List<string> sku, int catalogId, int localeId, List<string> navigationProperties, int userId, int portalId, int versionId, out List<PublishedConfigurableProductEntityModel> configurableProductEntities, bool includeInactiveProduct = false, int omsOrderId = 0);


        //Get Content State for this portal.
        ZnodePublishStatesEnum GetPortalPublishState();

        /// <summary>
        /// Get products addon.
        /// </summary>
        /// <param name="publishProducts"></param>
        /// <param name="portalId"></param>
        /// <param name="userId"></param>
        /// <param name="localeId"></param>
        /// <param name="catalogVersionId"></param>
        /// <returns></returns>
        List<PublishProductModel> GetProductsAddOn(List<PublishProductModel> publishProducts, int portalId, int userId, int localeId, int? catalogVersionId = null);

        /// <summary>
        /// This is use for get product reviews using sku
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="portalId"></param>
        /// <returns>List of CustomerReviewModel</returns>
        List<CustomerReviewModel> GetProductReviews(int portalId, string sku);

        /// <summary>
        /// Define filter for sp call.
        /// </summary>
        /// <param name="products">product list</param>
        /// <returns>fProduct filters</returns>
        DataTable GetProductFiltersForSP(List<PublishProductModel> products);

        /// <summary>
        /// Bind product deatails
        /// </summary>
        /// <param name="publishProductListModel">Product list</param>
        /// <param name="portalId">portal id</param>
        /// <param name="productDetails">SP data.</param>
        void BindProductDetails(PublishProductListModel publishProductListModel, int portalId, IList<PublishCategoryProductDetailModel> productDetails);

        /// <summary>
        /// To get additional product details.
        /// </summary>
        /// <param name="publishProduct">PublishProductModel</param>
        /// <param name="expands">Expands</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="userId">User Id</param>
        void GetAdditionalProductData(PublishProductModel publishProduct, List<string> expands, int localeId, int portalId, int userId);

        /// <summary>
        /// Published product price details shown on respective modules of Admin and Webstore application can be modified by this method.
        /// </summary>
        /// <param name="priceSKUList">List of PriceSKUModel</param>
        void ModifySKUPriceListDetails(List<PriceSKUModel> priceSKUList);

        /// <summary>
        /// Published product tier price details shown on respective modules of Admin and Webstore application can be modified by this method.
        /// </summary>
        /// <param name="tierPriceList">List of PriceTierModel</param>
        void ModifyTierPriceListDetails(List<PriceTierModel> tierPriceList);
    }
}