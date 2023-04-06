using System;
using System.Collections.Generic;
using System.Diagnostics;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    /// <summary>
    /// Helps manage product promotions.
    /// </summary>
    public class ZnodeProductPromotionManager : ZnodePromotionManager
    {
        #region Variables
        private ZnodeGenericCollection<IZnodeProductPromotionType> _productPromotions;
        protected readonly IZnodePromotionHelper promotionHelper = null;
        #endregion

        #region Constructor
        public ZnodeProductPromotionManager()
        {
            _productPromotions = new ZnodeGenericCollection<IZnodeProductPromotionType>();
            promotionHelper = GetService<IZnodePromotionHelper>();
            //Get the current portal.
            int currentPortalId = GetHeaderPortalId();
            int? currentProfileId = null;

            //Get the current profile.
            if (!currentProfileId.HasValue)
            {
                ProfileModel profile = promotionHelper.GetProfileCache();

                if (HelperUtility.IsNotNull(profile))
                    currentProfileId = profile.ProfileId;
            }

            List<PromotionModel> productPromotionsFromCache = promotionHelper.GetActivePromotions(currentPortalId, currentProfileId, ProductPromotionCache); 

            // Sort the promotions, build the list, then kill the cache clone
            BuildPromotionsList(productPromotionsFromCache, currentPortalId, currentProfileId);
            Dispose(productPromotionsFromCache);
        }
        #endregion

        #region Public Methods        
        // Changes details for a product.
        public void ChangeDetails(ZnodeProductBaseEntity product)
        {
            _productPromotions?.Sort("Precedence");

            foreach (ZnodeProductPromotionType promo in _productPromotions ?? new ZnodeGenericCollection<IZnodeProductPromotionType>())
                promo.ChangeDetails(product);
        }
        #endregion

        #region Private Methods

        //Bind cached promotion if any.
        private void BuildPromotionsList(List<PromotionModel> promotionsFromCache, int? currentPortalId, int? currentProfileId)
        {
            foreach (var promotion in promotionsFromCache ?? new List<PromotionModel>())
            {
                var promoBag = BuildPromotionBag(promotion, currentPortalId, currentProfileId);
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
            //Kindly take promotion message from promotion entity and not from promotion coupon entity.
            return promotionBag;
        }

        //Add Promotion Type in promotionBag
        private void AddPromotionType(PromotionModel promotion, ZnodePromotionBag promotionBag)
        {
                try
                {
                IZnodeProductPromotionType productPromo = GetPromotionTypeInstance<IZnodeProductPromotionType>(promotion);

                if (HelperUtility.IsNotNull(productPromo))
                    {
                        productPromo.Precedence = promotion.DisplayOrder.GetValueOrDefault();
                        productPromo.Bind(promotionBag);
                        _productPromotions.Add(productPromo);
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

