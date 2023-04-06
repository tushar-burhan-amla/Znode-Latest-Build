using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IPromotionClient : IBaseClient
    {
        #region Promotion

        /// <summary>
        /// Get promotion on the basis of promotionId.
        /// </summary>
        /// <param name="promotionId">Promotion Id to get promotion details.</param>
        /// <param name="expands">Expands to be retrieved along with promotion</param>
        /// <returns>Returns PromotionModel.</returns>
        PromotionModel GetPromotion(int promotionId, ExpandCollection expands);

        /// <summary>
        /// Gets the list of Promotion.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with promotion list.</param>
        /// <param name="filters">Filters to be applied on promotion list.</param>
        /// <param name="sorts">Sorting to be applied on promotion list.</param>
        /// <param name="pageIndex">Start page index of promotion list.</param>
        /// <param name="pageSize">Page size of promotion list.</param>
        /// <returns>Returns promotion list.</returns>
        PromotionListModel GetPromotionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create promotion.
        /// </summary>
        /// <param name="model">Promotion model to create.</param>
        /// <returns>Returns created promotion model.</returns>
        PromotionModel CreatePromotion(PromotionModel model);

        /// <summary>
        /// Update promotion model.
        /// </summary>
        /// <param name="model">Promotion model to update.</param>
        /// <returns>Returns updated promotion model.</returns>
        PromotionModel UpdatePromotion(PromotionModel model);

        /// <summary>
        /// Delete promotion by promotion Ids.
        /// </summary>
        /// <param name="promotionId">Promotion Ids to be deleted.</param>
        /// <returns>True if promotion deleted successfully; false if promotion fails to delete.</returns>
        bool DeletePromotion(ParameterModel promotionId);

        /// <summary>
        /// Gets list of published categories.
        /// </summary>
        /// <param name="filterIds">Filter to be applied on CategoryList.</param>
        /// <returns>List of published categories.</returns>
        CategoryListModel GetPublishedCategories(ParameterModel filterIds);

        /// <summary>
        /// Gets list of published products.
        /// </summary>
        /// <param name="filterIds">Filter to be applied on ProductList.</param>
        /// <returns>List of published products.</returns>
        ProductDetailsListModel GetPublishedProducts(ParameterModel filterIds);

        #endregion

        #region Coupon

        /// <summary>
        /// Get coupon on the basis of filter passed.
        /// </summary>
        /// <param name="filters">Filter to get coupon details.</param>
        /// <returns>Returns CouponModel.</returns>
        CouponModel GetCoupon(FilterCollection filters);

        /// <summary>
        /// Gets the list of Coupon.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with coupon list.</param>
        /// <param name="filters">Filters to be applied on coupon list.</param>
        /// <param name="sorts">Sorting to be applied on coupon list.</param>
        /// <param name="pageIndex">Start page index of coupon list.</param>
        /// <param name="pageSize">Page size of coupon list.</param>
        /// <returns>Returns coupon list.</returns>
        CouponListModel GetCouponList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get promotion attribute on changing discount type
        /// </summary>
        /// <param name="discountName">Discount type name</param>
        /// <returns></returns>
        PIMFamilyDetailsModel GetPromotionAttribute(string discountName);

        // <summary>
        /// Associate catalog to already created promotion. 
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="associateCatelogIds">catelog ids to be associate</param>
        /// <param name="promotionId">Promotion id</param>
        /// <returns>true/false</returns>
        bool AssociateCatalogToPromotion(int portalId, string associateCatelogIds, int promotionId);

        // <summary>
        /// Associate category to already created promotion. 
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="associateCategoryIds">category ids to be associate</param>
        /// <param name="promotionId">Promotion id</param>
        /// <returns>true/false</returns>
        bool AssociateCategoryToPromotion(int portalId, string associateCategoryIds, int promotionId);

        // <summary>
        /// Associate product to already created promotion. 
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="associateProductIds">Product ids to be associate</param>
        /// <param name="promotionId">Promotion id</param>
        /// <returns>true/false</returns>
        bool AssociateProductToPromotion(int portalId, string associateProductIds, int promotionId, string discountTypeName);

        /// <summary>
        /// Get associated or un-associated product by product ids.
        /// </summary>
        /// <param name="promotionModel">promotion Model.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Publish product List Model</returns>
        PublishProductListModel GetAssociatedUnAssociatedProductList(PromotionModel promotionModel, bool isAssociatedProduct, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        ///Get associated or un-associated category List
        /// </summary>
        /// <param name="promotionModel">promotion Model.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Publish Category List Model</returns>
        PublishCategoryListModel GetAssociatedUnAssociatedCategoryList(PromotionModel promotionModel, bool isAssociatedCategory, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get associated or un-associated catalogs by catalog ids.
        /// </summary>
        /// <param name="promotionModel">promotion Model.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Publish Catalog List Model</returns>
        PublishCatalogListModel GetAssociatedUnAssociatedCatalogList(PromotionModel promotionModel, bool isAssociatedCatalog, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

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

        // <summary>
        /// Associate brand to already created promotion. 
        /// </summary>
        /// <param name="associateBrandIds">Brand ids to be associate</param>
        /// <param name="promotionId">Promotion id</param>
        /// <returns>true/false</returns>
        bool AssociateBrandToPromotion(string associateBrandIds, int promotionId);

        /// <summary>
        /// Get associated or un-associated Brand by brand ids.
        /// </summary>
        /// <param name="promotionModel">promotion Model.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Brand List Model</returns>
        BrandListModel GetAssociatedUnAssociatedBrandList(PromotionModel promotionModel, bool isAssociatedBrand, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Removes a Brand type association entry from promotion.
        /// </summary>
        /// <param name="brandIds">IDs of Brand type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of Brand Associate to promotion.</param>
        /// <returns>True if Brand type association is removed;False if removal of Brand type association failed.</returns>
        bool UnAssociateBrand(ParameterModel brandIds, int promotionId);

        // <summary>
        /// Associate Shipping to already created promotion. 
        /// </summary>
        /// <param name="associateShippingIds">Shipping ids to be associate</param>
        /// <param name="promotionId">Promotion id</param>
        /// <returns>true/false</returns>
        bool AssociateShippingToPromotion(string associateShippingIds, int promotionId);

        /// <summary>
        /// Get associated or un-associated Shipping by Shipping ids.
        /// </summary>
        /// <param name="promotionModel">promotion Model.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Shipping List Model</returns>
        ShippingListModel GetAssociatedUnAssociatedShippingList(PromotionModel promotionModel, bool isAssociatedShipping, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Removes a Shipping type association entry from promotion.
        /// </summary>
        /// <param name="shippingIds">IDs of Shipping type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of Shipping Associate to promotion.</param>
        /// <returns>True if Shipping type association is removed;False if removal of Shipping type association failed.</returns>
        bool UnAssociateShipping(ParameterModel shippingIds, int promotionId);
        #endregion
    }
}
