using System.Collections.Generic;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPublishedProductDataService
    {
        /// <summary>
        /// Get Published Products based on pageListModel
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <returns>List of ZnodePublishProductEntity</returns>
        List<ZnodePublishProductEntity> GetPublishProducts(PageListModel pageListModel);

        /// <summary>
        /// Get Published products by filters
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>ZnodePublishProductEntity</returns>
        ZnodePublishProductEntity GetPublishProductByFilters(FilterCollection filters);

        /// <summary>
        /// Get publish product List by ids
        /// </summary>
        /// <param name="productIds"></param>
        /// <returns>List of ZnodePublishProductEntity</returns>
        List<ZnodePublishProductEntity> GetPublishProductListByIds(List<int> productIds);

        /// <summary>
        /// Get Publish Products PageList
        /// </summary>
        /// <param name="pageListModel"></param>
        /// <returns></returns>
        List<ZnodePublishProductEntity> GetPublishProductsPageList(PageListModel pageListModel, out int rowCount);

        /// <summary>
        /// Get Published Product Based on Parameters
        /// </summary>
        /// <param name="publishProductId">Product Id</param>
        /// <param name="portalId">portal Id</param>
        /// <param name="localeId">Locale ID</param>
        /// <param name="catalogVersionId">Version Id</param>
        /// <returns>ZnodePublishProductEntity</returns>
        ZnodePublishProductEntity GetPublishProduct(int publishProductId, int portalId, int localeId, int? catalogVersionId);

        /// <summary>
        /// Get Products Bt Category Id
        /// </summary>
        /// <param name="categoryId">Category Id</param>
        /// <param name="localeId">Locale ID</param>
        /// <returns>List of ZnodePublishProductEntity<ZnodePublishProductEntity></returns>
        List<ZnodePublishProductEntity> GetProductsByCategoryId(int categoryId, int localeId);

        /// <summary>
        /// Get Product Name by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <param name="localeId">Locale ID</param>
        /// <param name="catalogVersionId">Catalog Version Id</param>
        /// <returns>Product name</returns>
        string GetProductNameBySKU(string sku, int localeId, int catalogVersionId);


        /// <summary>
        /// Gets publish product by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <param name="publishedCatalogId">Catalog Id</param>
        /// <param name="localeId">locale Id</param>
        /// <param name="catalogVersionId">Version Id</param>
        /// <param name="omsOrderId">Order Id</param>
        /// <returns>ZnodePublishProductEntity</returns>
        ZnodePublishProductEntity GetPublishProductBySKU(string sku, int publishedCatalogId, int localeId, int? catalogVersionId = 0, int omsOrderId = 0);

        /// <summary>
        /// Gets publish product by list of SKUs
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="publishedCatalogId"></param>
        /// <param name="localeId"></param>
        /// <param name="catalogVersionId"></param>
        /// <returns>List of</returns>
        List<ZnodePublishProductEntity> GetPublishProductBySKUs(List<string> sku, int publishedCatalogId, int localeId, int? catalogVersionId = 0);

        /// <summary>
        /// Get Published Add on Products
        /// </summary>
        /// <param name="pageListModel"></param>
        /// <returns>List of ZnodePublishAddonEntity</returns>
        List<ZnodePublishAddonEntity> GetPublishAddonProducts(PageListModel pageListModel);

        /// <summary>
        /// Get Published Bundle Product
        /// </summary>
        /// <param name="pageListModel"></param>
        /// <returns>List of ZnodePublishBundleProductEntity</returns>
        List<ZnodePublishBundleProductEntity> GetPublishedBundleProduct(PageListModel pageListModel);

        /// <summary>
        ///  Get Published Configurable Product
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <returns>List of ZnodePublishConfigurableProductEntity</returns>
        List<ZnodePublishConfigurableProductEntity> GetPublishedConfigurableProducts(PageListModel pageListModel);

        /// <summary>
        /// Get Published Group Product
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <returns>List of ZnodePublishGroupProductEntity</returns>
        List<ZnodePublishGroupProductEntity> GetPublishedGroupProducts(PageListModel pageListModel);

        /// <summary>
        /// Get Associated Products
        /// </summary>
        /// <param name="catalogVersionId">catalog version id</param>
        /// <param name="localeId">locale id</param>
        /// <param name="publishProductId">publish product id</param>
        /// <returns></returns>
        List<ZnodePublishProductEntity> GetAssociatedConfigurableProducts(int publishProductId, int localeId, int? catalogVersionId);

        /// <summary>
        /// Get Publish Product count
        /// </summary>
        /// <param name="localeId">localeId</param>
        /// <param name="versionId">versionId</param>
        /// <param name="catalogId">catalogId</param>
        /// <returns>Product Count</returns>
        int GetPublishProductCount(int localeId, int versionId, int catalogId);

        /// <summary>
        /// Get Associated Bundle Products
        /// </summary>
        /// <param name="publishProductId">publish product id</param>
        /// <param name="localeId">locale id</param>
        /// <param name="catalogVersionId">catalog version id</param>
        /// <returns></returns>
        List<ZnodePublishProductEntity> GetAssociatedBundleProducts(int publishProductId, int localeId, int? catalogVersionId);

        /// <summary>
        /// Call update store procedure to update associated & linked products data
        /// </summary>
        void UpdatePublishedProductAssociatedData();

    }
}
