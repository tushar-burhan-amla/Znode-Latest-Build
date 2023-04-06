using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    /// <summary>
    /// Helps manage price promotions.
    /// </summary>
    public class ZnodePricePromotionManager : ZnodePromotionManager
    {
        #region Variables
        private ZnodeGenericCollection<IZnodePricePromotionType> _pricePromotions;
        protected readonly IZnodePromotionHelper promotionHelper = null;
        #endregion

        #region Constructor
        public ZnodePricePromotionManager()
        {
            _pricePromotions = new ZnodeGenericCollection<IZnodePricePromotionType>();
            promotionHelper = GetService<IZnodePromotionHelper>();
            //Get the current portal.
            int currentPortalId = GetHeaderPortalId();
            int? currentProfileId = null;

            ////Get the current profile.
            if (!currentProfileId.HasValue)
            {
                ProfileModel profile = promotionHelper.GetProfileCache();

                if (HelperUtility.IsNotNull(profile))
                    currentProfileId = profile.ProfileId;
            }

            List<PromotionModel> pricePromotionsFromCache = promotionHelper.GetActivePromotions(currentPortalId, currentProfileId, PricePromotionCache);

            // Sort the promotions, build the list, then kill the cache clone
            BuildPromotionsList(pricePromotionsFromCache, currentPortalId, currentProfileId);
            Dispose(pricePromotionsFromCache);
        }
        #endregion

        #region Public Methods

        // Calculates the final promotional price for the given product.
        public decimal PromotionalPrice(ZnodeProductBaseEntity product, int omsOrderId = 0) => PromotionalPrice(product.ProductID, product.ProductPrice, omsOrderId);


        // Calculates the final promotional price for the given product.
        public decimal PromotionalPrice(ZnodeProductBaseEntity product, decimal tieredPrice, int omsOrderId = 0) => PromotionalPrice(product.ProductID, tieredPrice, omsOrderId);

        // Calculates the final promotional price for the given product.
        public decimal PromotionalPrice(int productId, decimal currentPrice, int omsOrderId = 0)
        {
            decimal promoPrice = currentPrice;
            if (omsOrderId > 0)
                return promoPrice;
            _pricePromotions?.Sort("Precedence");

            foreach (ZnodePricePromotionType promo in _pricePromotions ?? new ZnodeGenericCollection<IZnodePricePromotionType>())
                promoPrice = promo.PromotionalPrice(productId, promoPrice);
                        return promoPrice;
        }

        #endregion

        #region Private Methods

        //Bind cached promotion if any.
        private void BuildPromotionsList(List<PromotionModel> promotionsFromCache, int? currentPortalId, int? currentProfileId)
        {
            foreach (PromotionModel promotion in promotionsFromCache ?? new List<PromotionModel>())
            {
                ZnodePromotionBag promoBag = BuildPromotionBag(promotion, currentPortalId, currentProfileId);
                AddPromotionType(promotion, promoBag);
            }
        }

        //Bind ZnodePromotion data to ZnodePromotionBag.
        private ZnodePromotionBag BuildPromotionBag(PromotionModel promotion, int? currentPortalId, int? currentProfileId)
        {
            ZnodePromotionBag promotionBag = new ZnodePromotionBag();
            promotionBag.PromotionId = promotion.PromotionId;
            promotionBag.PortalId = currentPortalId;
            promotionBag.ProfileId = currentProfileId;
            promotionBag.Discount = promotion.Discount.GetValueOrDefault();
            promotionBag.MinimumOrderAmount = promotion.OrderMinimum.GetValueOrDefault(0);
            promotionBag.RequiredProductMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.DiscountedProductId = promotion.ReferralPublishProductId.GetValueOrDefault(0);
            promotionBag.DiscountedProductQuantity = promotion.PromotionProductQuantity.GetValueOrDefault(1);
            promotionBag.RequiredBrandMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.RequiredCatalogMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.RequiredCategoryMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.IsCouponAllowedWithOtherCoupons = promotion.IsAllowedWithOtherCoupons;
            promotionBag.PromoCode = promotion.PromoCode;
            promotionBag.IsUnique = promotion.IsUnique;
            promotionBag.PromotionName = promotion.Name;
            promotionBag.PromotionTypeId = promotion.PromotionTypeId;
            promotionBag.PromotionMessage = promotion.PromotionMessage;
            if (promotion.IsCouponRequired.GetValueOrDefault())
            {
                promotionBag.Coupons = promotionHelper.GetPromotionCoupons(promotion.PromotionId);
            }
            //to set associated products to this promotion.
            promotionBag.AssociatedProducts = promotionHelper.GetPromotionProducts(promotion.PromotionId);

            return promotionBag;
        }

        //Add Promotion Type in promotionBag
        private void AddPromotionType(PromotionModel promotion, ZnodePromotionBag promotionBag)
        {
                try
                {

                IZnodePricePromotionType pricePromo = GetPromotionTypeInstance<IZnodePricePromotionType>(promotion);

                if (HelperUtility.IsNotNull(pricePromo))
                    {
                        pricePromo.Precedence = promotion.DisplayOrder.GetValueOrDefault();
                        pricePromo.Bind(promotionBag);
                        _pricePromotions.Add(pricePromo);
                    }
                }
                catch (Exception ex)
                {
                ZnodeLogging.LogMessage("Error while instantiating promotion type: " + promotion?.PromotionType + " " + ex.ToString(), ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Verbose);
                }
        }
        #endregion
    }
}
