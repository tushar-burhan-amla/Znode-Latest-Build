using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IPromotionAgent
    {
        #region Promotion

        /// <summary>
        /// Get promotion on the basis of promotionId.
        /// </summary>
        /// <param name="promotionId">Promotion Id to get promotion details.</param>
        /// <returns>Returns PromotionViewModel.</returns>
        PromotionViewModel GetPromotionById(int promotionId);

        /// <summary>
        /// Get promotion list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with promotion list.</param>
        /// <param name="filters">Filters to be applied on promotion list.</param>
        /// <param name="sorts">Sorting to be applied on promotion list.</param>
        /// <returns>Returns promotion list view model.</returns>
        PromotionListViewModel GetPromotionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get promotion list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with promotion list.</param>
        /// <param name="filters">Filters to be applied on promotion list.</param>
        /// <param name="sorts">Sorting to be applied on promotion list.</param>
        /// <param name="pageIndex">Start page index of promotion list.</param>
        /// <param name="pageSize">Records per page in promotion list.</param>
        /// <returns>Returns promotion list view model.</returns>
        PromotionListViewModel GetPromotionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isForExport);

        /// <summary>
        /// Create promotion.
        /// </summary>
        /// <param name="model">Promotion view model to create.</param>
        /// <returns>Returns created promotion view model.</returns>
        PromotionViewModel CreatePromotion(PromotionViewModel model, BindDataModel bindDataModel);

        /// <summary>
        /// Update promotion model.
        /// </summary>
        /// <param name="model">Promotion view model to update.</param>
        /// <returns>Returns updated promotion view model.</returns>
        PromotionViewModel UpdatePromotion(PromotionViewModel model, BindDataModel bindDataModel);

        /// <summary>
        /// Delete promotion by promotion Ids.
        /// </summary>
        /// <param name="promotionId">Promotion Ids to be deleted.</param>
        /// <returns>True if promotion deleted successfully; false if promotion fails to delete.</returns>
        bool DeletePromotion(string promotionId, out string errorMessage);

        /// <summary>
        /// Bind Dropdown values
        /// </summary>
        /// <param name="promotionViewModel">model of type PromotionViewModel</param>
        void BindDropdownValues(PromotionViewModel promotionViewModel);

        /// <summary>
        /// Get Published Catalogs as List of SelectListItem.
        /// </summary>
        /// <param name="storeIds">StoreId to create filter</param>
        /// <returns>Returns list of SelectListItem</returns>
        List<SelectListItem> GetPublishedCatalogList(int portalId);

        /// <summary>
        /// Get Published Categories as List of SelectListItem.
        /// </summary>
        /// <param name="publishedCatalogIds">Published Catalog Ids to create filter</param>
        /// <returns>Returns list of SelectListItem</returns>
        List<SelectListItem> GetPublishedCategoryList(int portalId);

        /// <summary>
        /// Get Published Products List.
        /// </summary>
        /// <returns>Returns list of Published Products.</returns>
        PublishProductsListViewModel GetPublishedProductList(int portalId, int promotionId, string assignedIds, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage);

        /// <summary>
        /// Get Published Categories List.
        /// </summary>
        /// <returns>Returns list of Published Products.</returns>
        CategoryListViewModel GetPublishedCategoryList(int portalId, int promotionId, string assignedIds, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage);

        /// <summary>
        /// Get Published Catelog List.
        /// </summary>
        /// <returns>Returns list of Published Products.</returns>
        PortalCatalogListViewModel GetPublishedCatelogList(int portalId, int promotionId, string assignedIds, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage);

        /// <summary>
        /// Get profile list by storeId.
        /// </summary>
        /// <param name="storeId"> storeId</param>
        /// <returns>Returns profile list.</returns>
        List<SelectListItem> ProfileListByStorId(int storeId);

        /// <summary>
        /// Get catalog list by storeId.
        /// </summary>
        /// <param name="storeId"> stroreId</param>
        /// <returns>Returns list of catalog.</returns>
        List<SelectListItem> CatalogListByStorId(int storeId);

        /// <summary>
        /// Get promotion attribute on changing discount type
        /// </summary>
        /// <param name="discountId">Discount type Id</param>
        /// <returns></returns>
        PIMFamilyDetailsViewModel GetPromotionAttribute(string discountName);


        /// <summary>
        /// Get associated or un-associated Catelog List.
        /// </summary>
        /// <returns>Returns list of associated or un-associated catelog</returns>
        PortalCatalogListViewModel GetAssociatedUnAssociatedCatelogList(int portalId, string catalogIds, int promotionId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage);

        /// <summary>
        /// Get associated or un-associated category List.
        /// </summary>
        /// <returns>Returns list of associated or un-associated categories</returns>
        CategoryListViewModel GetAssociatedUnAssociatedCategoryList(int portalId, string categoryIds, int promotionId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage);


        /// <summary>
        /// Get associated or un-associated Product List.
        /// </summary>
        /// <returns>Returns list of associated or un-associated Product</returns>
        PublishProductsListViewModel GetAssociatedUnAssociatedProductList(int portalId, string productIds, int promotionId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage);

        /// <summary>
        /// Associate catelog to already created promotion. 
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="associateCatelogIds">catelog ids to be associate</param>
        /// <param name="promotionId">Promotion id</param>
        /// <returns>true/false</returns>
        bool AssociateCatalogToPromotion(int portalId, string associateCatelogIds, int promotionId);

        /// <summary>
        /// Associate catelog to already created promotion. 
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="associateCategoryIds">category ids to be associate</param>
        /// <param name="promotionId">Promotion id</param>
        /// <returns>true/false</returns>
        bool AssociateCategoryToPromotion(int portalId, string associateCategoryIds, int promotionId);

        /// <summary>
        /// Associate product to already created promotion. 
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="associateProductIds">product ids to be associate</param>
        /// <param name="promotionId">Promotion id</param>
        /// <returns>true/false</returns>
        bool AssociateProductToPromotion(int portalId, string associateProductIds, int promotionId, string discountTypeName);

        /// <summary>
        /// Get Store list.
        /// </summary>
        /// <param name="filters">Filters to be applied on store list.</param>
        /// <param name="sorts">Sorting to be applied on store list.</param>
        /// <param name="pageIndex">Start page index of store list.</param>
        /// <param name="pageSize">Records per page in store list.</param>
        /// <returns>Store list.</returns>
        StoreListViewModel GetStoreList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = default(int?), int? pageSize = default(int?));

        /// <summary>
        /// Show all store checkbox
        /// </summary>
        /// <returns>bool</returns>
        bool ShowAllStoreCheckbox();

        /// <summary>
        /// Get promotion view model.
        /// </summary>
        /// <param name="viewModel">Promotion view model</param>
        /// <param name="bindDataModel">bind data model.</param>
        /// <returns>Return model with data bind from bind data model.</returns>
        PromotionViewModel GetPromotionViewModel(PromotionViewModel viewModel, BindDataModel bindDataModel);
        #endregion

        #region Coupon

        /// <summary>
        /// Get coupon on the basis of promotionId.
        /// </summary>
        /// <param name="promotionId">Promotion Id to get coupon details.</param>
        /// <returns>Returns CouponViewModel.</returns>
        CouponViewModel GetCoupon(int promotionId);

        /// <summary>
        /// Get coupon list.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with coupon list.</param>
        /// <param name="filters">Filters to be applied on coupon list.</param>
        /// <param name="sorts">Sorting to be applied on coupon list.</param>
        /// <param name="pageIndex">Start page index of coupon list.</param>
        /// <param name="pageSize">Records per page in coupon list.</param>
        /// <returns>Returns coupon list view model.</returns>
        CouponListViewModel GetCouponList(int promotionId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isForExport);
        /// <summary>
        /// Get coupon on the basis of promotionId.
        /// </summary>
        /// <param name="promotionId">Promotion Id to get already exists details.</param>
        /// /// <param name="PromoCode">code to get already exists details.</param>
        /// <returns>Returns CouponViewModel.</returns>
        bool CheckPromotionCodeExist(string PromoCode, int PromotionId);

        /// <summary>
        /// Get coupon on the basis of promotionId.
        /// </summary>
        /// <param name="PromotionCouponId">Promotioncoupon Id to get already exists details.</param>
        /// <param name="Code">code to get already exists details.</param>
        /// <returns>Returns CouponViewModel.</returns>
        bool CheckCouponCodeExist(string Code, int PromotionCouponId);

        /// <summary>
        /// Removes a product type association entry from promotion.
        /// </summary>
        /// <param name="publishProductIds">IDs of product type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of product Associate to promotion.</param>
        /// <returns>True if product type association is removed;False if removal of product type association failed.</returns>
        bool UnAssociateProduct(string publishProductIds, int promotionId);

        /// <summary>
        /// Removes a Category type association entry from promotion.
        /// </summary>
        /// <param name="publishCategoryIds">IDs of Category type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of Category Associate to promotion.</param>
        /// <returns>True if Category type association is removed;False if removal of Category type association failed.</returns>
        bool UnAssociateCategory(string publishCategoryIds, int promotionId);

        /// <summary>
        /// Removes a Catalog type association entry from promotion.
        /// </summary>
        /// <param name="publishCatalogIds">IDs of Catalog type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of Catalog Associate to promotion.</param>
        /// <returns>True if Catalog type association is removed;False if removal of Catalog type association failed.</returns>
        bool UnAssociateCatalog(string publishCatalogIds, int promotionId);

        #endregion

        #region Brand

        /// <summary>
        /// Associate brand to already created promotion. 
        /// </summary>
        /// <param name="associateBrandIds">brand ids to be associate</param>
        /// <param name="promotionId">Promotion id</param>
        /// <returns>true/false</returns>
        bool AssociateBrandToPromotion(string associateBrandIds, int promotionId);

        /// <summary>
        /// Get associated or un-associated brand List.
        /// </summary>
        /// <returns>Returns list of associated or un-associated brand</returns>
        BrandListViewModel GetAssociatedUnAssociatedBrandList(string brandIds, int promotionId, bool isAssociatedBrand, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage);

        /// <summary>
        /// Removes a brand type association entry from promotion.
        /// </summary>
        /// <param name="brandIds">IDs of brand type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of product Associate to promotion.</param>
        /// <returns>True if brand type association is removed;False if removal of brand type association failed.</returns>
        bool UnAssociateBrand(string brandIds, int promotionId);

        #endregion

        #region Shipping
        /// <summary>
        /// Associate Shipping to already created promotion. 
        /// </summary>
        /// <param name="associateShippingIds">Shipping ids to be associate</param>
        /// <param name="promotionId">Promotion id</param>
        /// <returns>true/false</returns>
        bool AssociateShippingToPromotion(string associateShippingIds, int promotionId);

        /// <summary>
        /// Get associated unassociated shipping list for portal.
        /// </summary>
        /// <param name="filters">Filters for portal shipping.</param>
        /// <param name="sorts">Sorts for for portal shipping.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Portal Shipping.</returns>
        ShippingListViewModel GetAssociatedUnAssociatedShippingList(int portalId, string shippingIds, int promotionId, bool isAssociatedShipping, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Removes a Shipping type association entry from promotion.
        /// </summary>
        /// <param name="ShippingIds">IDs of Shipping type association to be deleted.</param>
        /// <param name="promotionId">promotionId for delete record of product Associate to promotion.</param>
        /// <returns>True if Shipping type association is removed;False if removal of Shipping type association failed.</returns>
        bool UnAssociateShipping(string shippingIds, int promotionId);

        #endregion
    }
}