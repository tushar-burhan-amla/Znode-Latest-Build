using System.Collections.Generic;

using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
    public interface IZnodePromotionHelper
    {
        // Get profile model on basis of user id from cache.
        ProfileModel GetProfileCache();

        //to get all promotion type
        List<PromotionTypeModel> GetPromotionType();

        //to get all promotions 
        List<PromotionModel> GetAllPromotions();

        //Get promotions list which are active in current date.
        List<PromotionModel> GetActivePromotions(int? portalId, int? profileId, List<PromotionModel> promotionList);

        List<PromotionModel> GetOrderPromotions(int? omsOrderId, int? portalId, int? profileId, string[] newAppliedCoupons);

        bool IsValidPromotion(PromotionModel model, int? portalId, int? profileId);

        // Get Catalog Id By Product Ids.
        List<CatalogModel> GetCatalogByProduct(int productId);

        // Get Category By Product.
        List<CategoryModel> GetCategoryByProduct(int productId);

        // Get  Promotion Catalogs promotionId.
        List<CatalogModel> GetPromotionCatalogs(int promotionId);

        // Get Promotion Category by promotionId.
        List<CategoryModel> GetPromotionCategory(int promotionId);

        // Get Promotion products by promotionId.
        List<ProductModel> GetPromotionProducts(int promotionId);

        // Get Promotion products by promotionId.
        List<BrandModel> GetPromotionBrands(int promotionId);

        // Get Promotion products by promotionId.
        List<ShippingModel> GetPromotionShipping(int promotionId);

        // Get promotion coupons by promotionId.
        List<CouponModel> GetPromotionCoupons(int promotionId, string couponCodes = "");

        //Get all promotions by coupon code
        PromotionModel GetCouponsPromotion(List<PromotionModel> promotionsFromCache, string couponCode, int? currentPortalId, int? currentProfileId, int? orderId = null);

        //to check is multiple coupons allows or not
        bool AllowsMultipleCoupon(string couponCode, int? currentPortalId, int? currentProfileId);

        //to set promotion brand wise sku quantity in list model to calculate each item promotional quantity 
        List<PromotionCartItemQuantity> SetPromotionBrandSKUQuantity(List<BrandModel> promotionsBrand, ZnodeShoppingCart shoppingCart);

        //to set promotion brand wise sku quantity in list model to calculate each item promotional quantity 
        List<PromotionCartItemQuantity> SetPromotionCategorySKUQuantity(List<CategoryModel> promotionCategories, ZnodeShoppingCart shoppingCart, out decimal quantity);

        //to check applied coupon is used in exiting order by OrderId
        bool IsExistingOrderCoupon(int orderId, string couponCode);

        /// <summary>
        /// Check if the promotion is applicable
        /// </summary>
        /// <param name="promotionList">List of all the applicable promotions.</param>
        /// <param name="promoCode">Promotion to be applied</param>
        /// <param name="minValue">Minimum order amount/quantity</param>
        /// <param name="isOrderAmountBasedPromotion">Is order promotion [Default: false]</param>
        /// <returns>If the promotion is applicable or not</returns>

        bool IsApplicablePromotion(List<PromotionModel> promotionList, string promoCode, decimal minValue, bool isOrderAmountBasedPromotion = false, bool isCoupon = false);

        /// <summary>
        /// Check if the promotion is applicable
        /// </summary>
        /// <param name="promotionList">List of all the applicable promotions.</param>
        /// <param name="promoCode">Promotion to be applied</param>
        /// <param name="minQuantity">Minimum quantity</param>
        /// <param name="orderAmount">Order amount</param>
        /// <returns>Returns, if the promotion is applicable or not</returns>
        bool IsApplicablePromotion(List<PromotionModel> promotionList, string promoCode, decimal minOrderValue, decimal minQuantityValue, PromoApplicabilityCriteria applicabilityCriteria, bool isCoupon = false);

        /// <summary>
        /// Allow Promotion with coupons
        /// </summary>
        /// <param name="promotionList">list of promotions</param>
        List<PromotionModel> IsAllowWithPromotionsAndCoupons(List<PromotionModel> promotionList);

        /// <summary>
        /// Check if the promotion is applicable
        /// </summary>
        /// <param name="promotionList">List of all the applicable promotions.</param>
        /// <param name="promoCode">Promotion to be applied</param>
        /// <param name="minQuantity">Minimum quantity</param>
        /// <param name="orderAmount">Order amount</param>
        /// <returns>Returns, if the promotion is applicable or not</returns>
         bool IsApplicablePromotion(List<PromotionModel> promotionList, string promoCode, decimal minQuantity, decimal orderAmount, bool isCoupon = false);


        /// <summary>
        /// This method will find and return most suitable promotion from the applicable promotion list
        /// </summary>
        /// <param name="ApplicablePromolist"></param>
        /// <param name="minValue"></param>
        /// <param name="isCoupon"></param>
        /// <returns>Promotion list</returns>
        List<PromotionModel> GetMostApplicablePromoList(List<PromotionModel> applicablePromoList, decimal minValue, bool isCoupon);

        /// <summary>
        /// This method will find and return active shipping promotion from the applicable promotion list.
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        List<PromotionModel> GetActiveShippingPromotions(int? portalId, int? profileId);

        // This method check for Promotions With Exceptions in case of IsAllowWithOtherPromotionsAndCoupons is disable.
        bool IsAllowWithOtherPromotionsAndCoupons(string promotionClassName, bool globalIsAllowWithOtherPromotionsAndCoupons, int appliedCouponCount, bool isPendingOrder = false);
    }
}
