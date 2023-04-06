using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPromotionService
    {
        #region Promotion 

        /// <summary>
        /// Get promotion on the basis of promotionId.
        /// </summary>
        /// <param name="promotionId">Promotion Id.</param>
        /// <param name="expands">Expands to be retrieved along with promotion</param>
        /// <returns>Returns Promotion Model.</returns>
        PromotionModel GetPromotion(int promotionId, NameValueCollection expands);

        /// <summary>
        /// Get the list of promotions.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with promotion list.</param>
        /// <param name="filters">Filters to be applied on promotion list.</param>
        /// <param name="sorts">Sorting to be applied on promotion list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of promotion model.</returns>
        PromotionListModel GetPromotionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create promotion.
        /// </summary>
        /// <param name="model">Promotion model to create.</param>
        /// <returns>Returns created Promotion Model.</returns>
        PromotionModel CreatePromotion(PromotionModel model);

        /// <summary>
        /// Update promotion data.
        /// </summary>
        /// <param name="model">Promotion model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdatePromotion(PromotionModel model);

        /// <summary>
        /// Delete promotion as per promotion Ids.
        /// </summary>
        /// <param name="promotionId">Promotion Ids to be deleted.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeletePromotion(ParameterModel promotionId);

        /// <summary>
        /// Gets list of published categories.
        /// </summary>
        /// <param name="filterIds">Filter to be applied on category list</param>
        /// <returns>List of published categories.</returns>
        CategoryListModel GetPublishedCategoryList(ParameterModel filterIds);

        /// <summary>
        /// Get list of published products.
        /// </summary>
        /// <param name="filterIds">Filter to be applied on product list</param>
        /// <returns>List of published products.</returns>
        ProductDetailsListModel GetPublishedProductList(ParameterModel filterIds);

        /// <summary>
        /// Get promotion attribute on changing discount type
        /// </summary>
        /// <param name="discountName">Discount type name</param>
        /// <returns></returns>
        PIMFamilyDetailsModel GetPromotionAttribute(string discountName);

        /// <summary>
        /// Associate catalog to already created promotion. 
        /// </summary>
        /// <param name="model">AssociatedParameterModel</param>
        /// <returns>True/False</returns>
        bool AssociateCatalogToPromotion(AssociatedParameterModel model);

        /// <summary>
        /// Associate category to already created promotion. 
        /// </summary>
        /// <param name="model">AssociatedParameterModel</param>
        /// <returns>True/False</returns>
        bool AssociateCategoryToPromotion(AssociatedParameterModel model);

        /// <summary>
        /// Associate product to already created promotion. 
        /// </summary>
        /// <param name="model">AssociatedParameterModel</param>
        /// <returns>True/False</returns>
        bool AssociateProductToPromotion(AssociatedParameterModel model);

        /// <summary>
        /// set association of promotion.
        /// </summary>
        /// <param name="promotionModel">promotion model</param>
        /// <param name="promotionId">promotion model.</param>
        void SetAssociationToPromotion(PromotionModel promotionModel, int promotionId);

        #endregion

        #region Coupon

        /// <summary>
        /// Get coupon on the basis of filter collection.
        /// </summary>
        /// <param name="filters">Filter Collection.</param>
        /// <returns>Returns CouponModel.</returns>
        CouponModel GetCoupon(FilterCollection filters);

        /// <summary>
        /// Get the list of coupons.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with coupons list.</param>
        /// <param name="filters">Filters to be applied on coupons list.</param>
        /// <param name="sorts">Sorting to be applied on coupons list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of coupons model.</returns>
        CouponListModel GetCouponList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get associated or UnAssociated products on the basis of isAssociatedProduct
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="productIds">product Ids.</param>
        /// <param name="promotionId">Promotion Ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters</param>
        /// <returns>associated product List Model</returns>
        PublishProductListModel GetAssociatedUnAssociatedProductList(int portalId, string productIds, int promotionId, bool isAssociatedProduct, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get associated products 
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="productIds">product Ids.</param>
        /// <param name="promotionId">Promotion Ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters</param>
        /// <returns>associated product List Model</returns>
        PublishProductListModel GetAssociatedProductList(int portalId, string productIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get un-associated products 
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="productIds">product Ids.</param>
        /// <param name="promotionId">Promotion Ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters</param>
        /// <returns>associated product List Model</returns>
        PublishProductListModel GetUnAssociatedProductList(int portalId, string productIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get associated or un-associated category list  on the basis of isAssociatedCategory
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="categoryIds"></param>
        /// <param name="promotionId"></param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>un-associated Category List Model</returns>
        PublishCategoryListModel GetAssociatedUnAssociatedCategoryList(int portalId, string categoryIds, int promotionId, bool isAssociatedCategory, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get un-associated category list 
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="categoryIds"></param>
        /// <param name="promotionId"></param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>un-associated Category List Model</returns>
        PublishCategoryListModel GetUnAssociatedCategoryList(int portalId, string categoryIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get associated category list
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="categoryIds"></param>
        /// <param name="promotionId"></param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>associated Category List Model</returns>
        PublishCategoryListModel GetAssociatedCategoryList(int portalId, string categoryIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get un-associated catalogs 
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="catelogIds">Catelog Ids.</param>
        /// <param name="promotionId">Promotion Ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters</param>
        /// <returns>Un-associated Catalog List Model</returns>
        PublishCatalogListModel GetUnAssociatedCatalogList(int portalId, string catalogIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get associated catalogs  
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="catelogIds">Catelog Ids.</param>
        /// <param name="promotionId">Promotion Ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters</param>
        /// <returns>Associated Catalog List Model</returns>
        PublishCatalogListModel GetAssociatedCatalogList(int portalId, string catalogIds, int promotionId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get associated UnAssociated catalogs on the basis of isAssociatedCatalog
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="catelogIds">Catelog Ids.</param>
        /// <param name="promotionId">Promotion Ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters</param>
        /// <returns>Associated Catalog List Model</returns>
        PublishCatalogListModel GetAssociatedUnAssociatedCatalogList(int portalId, string catalogIds, int promotionId, bool isAssociatedCatalog, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Removes a product type association entry from promotion.
        /// </summary>
        /// <param name="publishProductIds">IDs of product type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of product Associate to promotion.</param>
        /// <returns>True if product type association is removed;False if removal of product type association failed.</returns>
        bool UnAssociateProduct(ParameterModel publishProductIds, int promotionId);

        /// <summary>
        /// Removes a Category type association entry from promotion.
        /// </summary>
        /// <param name="publishCategoryIds">IDs of Category type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of Category Associate to promotion.</param>
        /// <returns>True if Category type association is removed;False if removal of Category type association failed.</returns>
        bool UnAssociateCategory(ParameterModel publishCategoryIds, int promotionId);

        /// <summary>
        /// Removes a Catalog type association entry from promotion.
        /// </summary>
        /// <param name="publishCatalogIds">IDs of Catalog type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of Catalog Associate to promotion.</param>
        /// <returns>True if Catalog type association is removed;False if removal of Catalog type association failed.</returns>
        bool UnAssociateCatalog(ParameterModel publishCatalogIds, int promotionId);

        #region Brand 
        /// <summary>
        /// Get associated or UnAssociated Brand on the basis of isAssociatedBrand
        /// </summary>
        /// <param name="brandIds">Brand Ids.</param>
        /// <param name="promotionId">Promotion Ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters</param>
        /// <returns>associated Brand List Model</returns>
        BrandListModel GetAssociatedUnAssociatedBrandList(string brandIds, int promotionId, bool isAssociatedBrand, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate Brand to already created promotion. 
        /// </summary>
        /// <param name="model">AssociatedParameterModel</param>
        /// <returns>True/False</returns>
        bool AssociateBrandToPromotion(AssociatedParameterModel model);

        /// <summary>
        /// Removes a Brand type association entry from promotion.
        /// </summary>
        /// <param name="brandIds">IDs of Brand type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of Brand Associate to promotion.</param>
        /// <returns>True if Brand type association is removed;False if removal of Brand type association failed.</returns>
        bool UnAssociateBrand(ParameterModel brandIds, int promotionId);
        #endregion

        #region Shipping 
        /// <summary>
        /// Get associated or UnAssociated Shipping on the basis of isAssociatedShipping
        /// </summary>
        /// <param name="ShippingIds">Shipping Ids.</param>
        /// <param name="promotionId">Promotion Ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters</param>
        /// <returns>associated Shipping List Model</returns>
        ShippingListModel GetAssociatedUnAssociatedShippingList(int portalId, string ShippingIds, int promotionId, bool isAssociatedShipping, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate Shipping to already created promotion. 
        /// </summary>
        /// <param name="model">AssociatedParameterModel</param>
        /// <returns>True/False</returns>
        bool AssociateShippingToPromotion(AssociatedParameterModel model);

        /// <summary>
        /// Removes a Shipping type association entry from promotion.
        /// </summary>
        /// <param name="ShippingIds">IDs of Shipping type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of Shipping Associate to promotion.</param>
        /// <returns>True if Shipping type association is removed;False if removal of Shipping type association failed.</returns>
        bool UnAssociateShipping(ParameterModel ShippingIds, int promotionId);
        #endregion
        #endregion
    }
}
