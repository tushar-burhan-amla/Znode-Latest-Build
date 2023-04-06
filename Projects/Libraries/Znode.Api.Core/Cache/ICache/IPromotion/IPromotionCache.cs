namespace Znode.Engine.Api.Cache
{
    public interface IPromotionCache
    {
        /// <summary>
        /// Get promotion details.
        /// </summary>
        /// <param name="promotionId">promotionId to get promotion details</param>
        /// <param name="routeUri">route Uri</param>
        /// <param name="routeTemplate">route Template</param>
        /// <returns>Returns promotion data</returns>
        string GetPromotion(int promotionId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Promotion list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Promotion list.</returns>
        string GetPromotionList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get coupon details.
        /// </summary>
        /// <param name="routeUri">route Uri</param>
        /// <param name="routeTemplate">route Template</param>
        /// <returns>Returns coupon data</returns>
        string GetCoupon(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Coupon list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Coupon list.</returns>
        string GetCouponList(string routeUri, string routeTemplate);

        #region Product
        /// <summary>
        /// Get Publish associated or unAssociated products from productIds.
        /// </summary>
        /// <param name="portalId">portal Id.</param>
        /// <param name="productIds">product Ids.</param>
        /// <param name="promotionId">promotion Id.</param>
        /// <param name="isAssociatedProduct">isAssociatedProduct</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetAssociatedUnAssociatedProductList(int portalId, string productIds, int promotionId, bool isAssociatedProduct, string routeUri, string routeTemplate);

        #endregion

        #region Category
        /// <summary>
        /// Get associated or unAssociated category list
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="categoryIds"></param>
        /// <param name="promotionId"></param>
        /// <param name="routeUri"></param>
        ///  <param name="isAssociatedCategory"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetAssociatedUnAssociatedCategoryList(int portalId, string categoryIds, int promotionId,bool isAssociatedCategory, string routeUri, string routeTemplate);
        #endregion

        #region Catalog
        /// <summary>
        /// Get Publish Associated or unAssociated Catalogs from catalogIds.
        /// </summary>
        /// <param name="portalId">portal Id.</param>
        /// <param name="catalogIds">catalogIds Ids.</param>
        /// <param name="promotionId">promotion Id.</param>
        /// <param name="isAssociatedCatalog">isAssociatedCatalog.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetAssociatedUnAssociatedCatalogList(int portalId, string catalogIds, int promotionId, bool isAssociatedCatalog, string routeUri, string routeTemplate);
        #endregion

        #region Brand
        /// <summary>
        /// Get associated or unAssociated Brand from BrandIds.
        /// </summary>
        /// <param name="brandIds">Brand Ids.</param>
        /// <param name="promotionId">promotion Id.</param>
        /// <param name="isAssociatedBrand">isAssociatedBrand</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetAssociatedUnAssociatedBrandList(string brandIds, int promotionId, bool isAssociatedBrand, string routeUri, string routeTemplate);

        #endregion

        #region Shipping
        /// <summary>
        /// Get associated or unAssociated Shipping from ShippingIds.
        /// </summary>
        /// <param name="portalId">PortalId.</param>
        /// <param name="ShippingIds">Shipping Ids.</param>
        /// <param name="promotionId">promotion Id.</param>
        /// <param name="isAssociatedShipping">isAssociatedShipping</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetAssociatedUnAssociatedShippingList(int portalId, string ShippingIds, int promotionId, bool isAssociatedShipping, string routeUri, string routeTemplate);

        #endregion
    }
}