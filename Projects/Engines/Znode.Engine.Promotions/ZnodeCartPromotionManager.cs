using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    /// <summary>
    /// Helps manage cart promotions.
    /// </summary>
    public class ZnodeCartPromotionManager : ZnodePromotionManager, IZnodeCartPromotionManager
    {
        #region Private Variables      
        private ZnodeShoppingCart _shoppingCart;
        private ZnodeGenericCollection<IZnodeCartPromotionType> _cartPromotions;
        protected readonly IZnodePromotionHelper promotionHelper = null;
        List<PromotionModel> newPromotionsFromCache;
        private readonly IZnodeOrderDiscountHelper znodeOrderDiscountHelper = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Throws a NotImplementedException because this class requires a shopping cart to work.
        /// </summary>
        public ZnodeCartPromotionManager()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Instantiates all the promotions that are required for the current shopping cart.
        /// </summary>
        /// <param name="shoppingCart">The current shopping cart.</param>
        /// <param name="profileId">The current profile ID, if any.</param>
        public ZnodeCartPromotionManager(ZnodeShoppingCart shoppingCart, int? profileId)
        {
            _shoppingCart = shoppingCart;
            _cartPromotions = new ZnodeGenericCollection<IZnodeCartPromotionType>();
            znodeOrderDiscountHelper = GetService<IZnodeOrderDiscountHelper>(new ZnodeNamedParameter("shoppingCart", shoppingCart));
            promotionHelper = GetService<IZnodePromotionHelper>();
            //Get the current portal.
            int currentPortalId = GetHeaderPortalId();

            //check if shoppingcart is null.
            if (HelperUtility.IsNotNull(_shoppingCart) && HelperUtility.IsNotNull(_shoppingCart.PortalId))
                currentPortalId = _shoppingCart.PortalId.GetValueOrDefault();

            //Get the current profile.
            if (!profileId.HasValue)
            {
                ProfileModel profile = promotionHelper.GetProfileCache();

                if (HelperUtility.IsNotNull(profile))
                    profileId = profile.ProfileId;
            }
            if (CartPromotionCache?.Count <= 0)
                CacheActiveCartPromotions();

            // Check for default portal

            List<PromotionModel> cartPromotionsFromCache = CartPromotionCache;
            cartPromotionsFromCache = ApplyPromotionsFilter(cartPromotionsFromCache, currentPortalId, profileId);
            //Check if cached promotion are null and bind data to ZNodePromotionBag. 
            if (HelperUtility.IsNotNull(newPromotionsFromCache))
            {
                BuildPromotionsList(newPromotionsFromCache, currentPortalId, profileId);
                Dispose(newPromotionsFromCache);
            }
            else
            {
                BuildPromotionsList(cartPromotionsFromCache, currentPortalId, profileId);
                Dispose(cartPromotionsFromCache);
            }
        }

        /// <summary>
        /// Calculates the promotion and updates the shopping cart.
        /// </summary>
        public virtual void Calculate()
        {
            // No sense in continuing without a shopping cart
            if (HelperUtility.IsNull(_shoppingCart))
                return;

            _shoppingCart.ClearPreviousErrorMessages();

            ClearAllPromotionsAndCoupons();

            SetPromotionalPrice();


            if (_shoppingCart.OrderId > 0 && !_shoppingCart.IsOldOrder)
            {
                znodeOrderDiscountHelper.OrderCalculate();
            }
            else
            {
                CalculateEachPromotion();
              
                if (HelperUtility.IsNotNull(_shoppingCart.Coupons))
                {
                    for (int couponIndex = 0; couponIndex < _shoppingCart.Coupons.Count; couponIndex++)
                        CheckForInvalidCoupon(couponIndex);
                }
            }
            _shoppingCart.CalculateLineItemCSRDiscount();
            if (_shoppingCart.Shipping?.ShippingDiscount > 0)
                _shoppingCart.ShippingDiscount = _shoppingCart.Shipping.ShippingDiscount;
            else
                _shoppingCart.ShippingDiscount = 0;
        }
        /// <summary>
        /// Process anything that must be done before the order is submitted.
        /// </summary>
        /// <returns>True if everything is good for submitting the order; otherwise, false.</returns>
        public virtual bool PreSubmitOrderProcess()
        {
            bool allPreConditionsOk = true;

            foreach (ZnodeCartPromotionType promo in _cartPromotions)
                // Make sure all pre-conditions are good before letting the customer check out
                allPreConditionsOk &= promo.PreSubmitOrderProcess();
            return allPreConditionsOk;
        }

        /// <summary>
        /// Process anything that must be done after the order is submitted.
        /// </summary>
        public virtual void PostSubmitOrderProcess()
        {
            foreach (ZnodeCartPromotionType promo in _cartPromotions)
                promo.PostSubmitOrderProcess();
        }
        #endregion

        #region Private Methods

        //Set Promotional Price for Products
        public virtual void SetPromotionalPrice()
        {
            ZnodePricePromotionManager pricePromoManager = new ZnodePricePromotionManager();
            foreach (ZnodeShoppingCartItem cartItem in _shoppingCart.ShoppingCartItems)
            {
                cartItem.PromotionalPrice = pricePromoManager.PromotionalPrice(cartItem.Product, cartItem.TieredPricing, _shoppingCart.OrderId.GetValueOrDefault());

                cartItem.Product.PromotionalPrice = cartItem.PromotionalPrice;

                if (cartItem.Product.ZNodeAddonsProductCollection.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity addon in cartItem.Product.ZNodeAddonsProductCollection)
                    {
                        addon.PromotionalPrice = addon.FinalPrice;
                    }
                }

                if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity config in cartItem.Product.ZNodeConfigurableProductCollection)
                    {
                        config.PromotionalPrice = config.FinalPrice;
                    }
                }

                if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                    {
                        group.PromotionalPrice = group.FinalPrice;
                    }
                }
            }
        }

        //to clear all promotions and coupons applied to shopping cart
        public virtual void ClearAllPromotionsAndCoupons()
        {
            _shoppingCart.Shipping.ShippingDiscount = 0;

            ClearDiscount();

            //to clear all promotions and coupons applied 
            if (_shoppingCart?.Coupons?.Count > 0)
            {
                foreach (Libraries.ECommerce.Entities.ZnodeCoupon coupon in _shoppingCart.Coupons)
                {
                    coupon.CouponApplied = false;
                    coupon.CouponValid = false;
                    coupon.CouponMessage = string.Empty;
                }
            }
        }

        protected virtual void ClearDiscount()
        {
            _shoppingCart.OrderLevelDiscount = 0;
            _shoppingCart?.OrderLevelDiscountDetails?.Clear();
            foreach (ZnodeShoppingCartItem cartItem in _shoppingCart.ShoppingCartItems)
            {
                cartItem.ExtendedPriceDiscount = 0;
                cartItem.Product.DiscountAmount = 0;
                cartItem.Product.OrdersDiscount = null;
                cartItem.Product.OrderDiscountAmount = 0;
                cartItem.Product.CSRDiscountAmount = 0;

                //Clear discount for Addons Product Collection
                ClearDiscountFromProductCollection(cartItem.Product?.ZNodeAddonsProductCollection);

                //Clear discount for Bundle Product Collection
                ClearDiscountFromProductCollection(cartItem.Product?.ZNodeBundleProductCollection);

                //Clear discount for Configurable Product Collection
                ClearDiscountFromProductCollection(cartItem.Product?.ZNodeConfigurableProductCollection);

                //Clear discount for Group Product Collection
                ClearDiscountFromProductCollection(cartItem.Product?.ZNodeGroupProductCollection);
            }

        }

        protected virtual void ClearDiscountFromProductCollection(ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection)
        {
            foreach (ZnodeProductBaseEntity product in productCollection)
            {
                product.DiscountAmount = 0;
                product.OrdersDiscount = null;
                product.OrderDiscountAmount = 0;
                product.CSRDiscountAmount = 0;
            }
        }

        //Calculate each and every promotion applied on cart.
        protected virtual void CalculateEachPromotion()
        {
            List<PromotionModel> activePromotions = null;
            if (_shoppingCart.OrderId > 0 && _shoppingCart.IsOldOrder)
            {
                _cartPromotions?.Clear();
                string[] newAppliedCoupons = _shoppingCart.Coupons.Where(x => x.IsExistInOrder == false).Select(x => x.Coupon).ToArray();
                activePromotions = promotionHelper.GetOrderPromotions(_shoppingCart.OrderId, _shoppingCart.PortalId, _shoppingCart.ProfileId, newAppliedCoupons);

                BuildPromotionsList(activePromotions, _shoppingCart.PortalId, _shoppingCart.ProfileId);

            }
                
            else
                activePromotions = promotionHelper.GetActivePromotions(_shoppingCart.PortalId, _shoppingCart.ProfileId, promotionHelper.GetAllPromotions());
                                


            foreach (PromotionModel item in activePromotions?.Where(x => x.IsCouponRequired == false))
            {
                item.IsAllowWithOtherPromotionsAndCoupons = _shoppingCart.IsAllowWithOtherPromotionsAndCoupons;
            }

            activePromotions = (!_shoppingCart.IsAllowWithOtherPromotionsAndCoupons) ? promotionHelper.IsAllowWithPromotionsAndCoupons(activePromotions) : activePromotions;

            //IsAllowWithOtherPromotionsAndCoupons=false then remove previously applied discount.
            if (!_shoppingCart.IsPendingOrderRequest && !_shoppingCart.IsAllowWithOtherPromotionsAndCoupons && activePromotions.Where(a => !a.IsAllowWithOtherPromotionsAndCoupons).ToList()?.Count > 0)
                RemoveDiscount();

            for (int cartPromoIndex = 0, cartPromoCouponIndex = 0; cartPromoIndex < _cartPromotions.Count; cartPromoIndex++)
            {
                if (_cartPromotions[cartPromoIndex].IsPromoCouponAvailable)
                {
                    if (_shoppingCart.IsPendingOrderRequest)
                    {
                        if (_shoppingCart?.Coupons?.Count > 0)
                        {
                            _shoppingCart.Coupons[cartPromoCouponIndex].CouponValid = false;
                            _shoppingCart.Coupons[cartPromoCouponIndex].CouponApplied = false;
                            _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage = string.IsNullOrEmpty(_shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage) ? "Invalid Coupon Code." : _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage;
                        }
                    }
                    else
                    {
                        CalculateCouponDiscount(activePromotions, cartPromoIndex, cartPromoCouponIndex);
                    }
                    cartPromoCouponIndex++;
                }
                else
                {
                    string promotionClassName = _cartPromotions[cartPromoIndex].ClassName;

                    int appliedCouponCount = _shoppingCart.Coupons.Select(x => x.CouponApplied).Count();
                    if ((znodeOrderDiscountHelper.IsDiscountApplicable() || IsShippingPromotionDiscount(promotionClassName)) && promotionHelper.IsAllowWithOtherPromotionsAndCoupons(promotionClassName, _shoppingCart.IsAllowWithOtherPromotionsAndCoupons, appliedCouponCount, _shoppingCart.IsPendingOrderRequest))
                        CalculatePromotionDiscount(activePromotions, cartPromoIndex);
                }
            }
        }

        protected virtual void CalculatePromotionDiscount(List<PromotionModel> activePromotions, int cartPromoIndex)
        {
            _cartPromotions[cartPromoIndex].Calculate(null, activePromotions);
        }

        protected virtual void CalculateCouponDiscount(List<PromotionModel> activePromotions, int cartPromoIndex, int cartPromoCouponIndex)
        {
            string promotionClassName = _cartPromotions[cartPromoIndex].ClassName;
            if (Equals(cartPromoCouponIndex, 0))
            {
                
                  
                if(!znodeOrderDiscountHelper.IsDiscountApplicable())
                {
                    CouponDiscountErrorMessage(cartPromoCouponIndex);
                }
                if (!string.IsNullOrEmpty(promotionClassName)
                        && (string.Equals(promotionClassName, ZnodeConstant.AmountOffShipping, StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(promotionClassName, ZnodeConstant.AmountOffShippingWithCarrier, StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(promotionClassName, ZnodeConstant.PercentOffShipping, StringComparison.InvariantCultureIgnoreCase)
                        || string.Equals(promotionClassName, ZnodeConstant.PercentOffShippingWithCarrier, StringComparison.InvariantCultureIgnoreCase))
                        && (!znodeOrderDiscountHelper.IsShippingDiscountApplicable()))
                {
                    ShippingCouponDiscountErrorMessage(cartPromoCouponIndex);
                }
                else
                {
                    CalculateCouponPromotion(cartPromoIndex, cartPromoCouponIndex, activePromotions);
                }
            }
            else
            {
                //Check if multiple coupons are applied or not and set the value of coupons of cart accordingly.
                if (_shoppingCart.CartAllowsMultiCoupon && _shoppingCart.Coupons[cartPromoCouponIndex].AllowsMultipleCoupon)
                {
                    if (!znodeOrderDiscountHelper.IsDiscountApplicable())
                    {
                        CouponDiscountErrorMessage(cartPromoCouponIndex);
                    }
                    if (!string.IsNullOrEmpty(promotionClassName)
                             && (string.Equals(promotionClassName, ZnodeConstant.AmountOffShipping, StringComparison.InvariantCultureIgnoreCase)
                             || string.Equals(promotionClassName, ZnodeConstant.AmountOffShippingWithCarrier, StringComparison.InvariantCultureIgnoreCase)
                             || string.Equals(promotionClassName, ZnodeConstant.PercentOffShipping, StringComparison.InvariantCultureIgnoreCase)
                             || string.Equals(promotionClassName, ZnodeConstant.PercentOffShippingWithCarrier, StringComparison.InvariantCultureIgnoreCase))
                             && (!znodeOrderDiscountHelper.IsShippingDiscountApplicable()))
                    {
                        ShippingCouponDiscountErrorMessage(cartPromoCouponIndex);
                    }
                    else
                    {
                        CalculateCouponPromotion(cartPromoIndex, cartPromoCouponIndex, activePromotions);
                    }
                }
                else if (_shoppingCart?.Coupons?.Count > 0)
                {
                    _shoppingCart.Coupons[cartPromoCouponIndex].CouponApplied = false;
                    _shoppingCart.Coupons[cartPromoCouponIndex].CouponValid = false;
                    _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage = string.IsNullOrEmpty(_shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage) ? Api_Resources.CouponMessage : _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage;
                }
            }
        }

        protected virtual void CouponDiscountErrorMessage(int cartPromoCouponIndex)
        {
            _shoppingCart.Coupons[cartPromoCouponIndex].CouponValid = false;
            _shoppingCart.Coupons[cartPromoCouponIndex].CouponApplied = false;
            _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage = Api_Resources.NoFurtherCouponMessage;
        }

        protected virtual void ShippingCouponDiscountErrorMessage(int cartPromoCouponIndex)
        {
            _shoppingCart.Coupons[cartPromoCouponIndex].CouponValid = false;
            _shoppingCart.Coupons[cartPromoCouponIndex].CouponApplied = false;
            _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage = Api_Resources.NoFurtherShippingCouponMessage;
        }

        protected virtual void RemoveDiscount()
        {
            _shoppingCart.OrderLevelDiscount = 0;
            ZnodePricePromotionManager pricePromoManager = new ZnodePricePromotionManager();
            _shoppingCart.OrderLevelDiscountDetails?.Clear();
            _shoppingCart.DiscountAppliedSequence?.Clear();
            _shoppingCart.DiscountedProductQuantity?.Clear();
            foreach (ZnodeShoppingCartItem cartItem in _shoppingCart.ShoppingCartItems)
            {
                cartItem.Product.DiscountAmount = 0;
                cartItem.Product.OrderDiscountAmount = 0;
                cartItem.PromotionalPrice = pricePromoManager.PromotionalPrice(cartItem.Product, cartItem.TieredPricing, _shoppingCart.OrderId.GetValueOrDefault());
                cartItem.Product.OrdersDiscount?.Clear();
                foreach (ZnodeProductBaseEntity addon in cartItem.Product.ZNodeAddonsProductCollection)
                {
                    addon.DiscountAmount = 0;
                    addon.OrderDiscountAmount = 0;
                    addon.OrdersDiscount?.Clear();
                }

                foreach (ZnodeProductBaseEntity config in cartItem.Product.ZNodeConfigurableProductCollection)
                {
                    config.DiscountAmount = 0;
                    config.OrderDiscountAmount = 0;
                    config.OrdersDiscount?.Clear();
                }

                foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                {
                    group.DiscountAmount = 0;
                    group.OrderDiscountAmount = 0;
                    group.OrdersDiscount?.Clear();
                }
                cartItem.ExtendedPriceDiscount = 0;
            }
        }

        //Calculate each and every Coupon promotion applied on cart.
        protected virtual void CalculateCouponPromotion(int cartPromoIndex, int cartPromoCouponIndex, List<PromotionModel> AllPromotions)
        {
            _cartPromotions[cartPromoIndex].Calculate(cartPromoCouponIndex, AllPromotions);

            if (!Equals(_shoppingCart.Coupons, null) && _shoppingCart.Coupons.Count > 0 && !Equals(_shoppingCart.Coupons[cartPromoCouponIndex], null) && _shoppingCart.Coupons[cartPromoCouponIndex].CouponApplied)
            {
                if (cartPromoCouponIndex == 0)
                    _shoppingCart.ClearPreviousErrorMessages();
                
                    _shoppingCart.AddPromoDescription += _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage;
                    _shoppingCart.Coupons[cartPromoCouponIndex].CouponPromotionType = _cartPromotions[cartPromoIndex]?.ClassName;
            }
            else if (_shoppingCart?.Coupons?.Count > 0)
            {
                _shoppingCart.Coupons[cartPromoCouponIndex].CouponValid = false;
                _shoppingCart.Coupons[cartPromoCouponIndex].CouponApplied = false;
                _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage = string.IsNullOrEmpty(_shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage) ? "Invalid Coupon Code." : _shoppingCart.Coupons[cartPromoCouponIndex].CouponMessage;
            }
        }

        //check for invalid coupon is applied on cart.
        protected virtual void CheckForInvalidCoupon(int couponIndex)
        {
            if (!_shoppingCart.Coupons[couponIndex].CouponApplied && !_shoppingCart.Coupons[couponIndex].CouponValid && !string.IsNullOrEmpty(_shoppingCart.Coupons[couponIndex].Coupon))
            {
                _shoppingCart.AddPromoDescription = GlobalSetting_Resources.MessageInvalidCoupon;
                _shoppingCart.Coupons[couponIndex].CouponMessage = !string.IsNullOrEmpty(_shoppingCart.Coupons[couponIndex].CouponMessage) ? _shoppingCart.Coupons[couponIndex].CouponMessage : GlobalSetting_Resources.MessageInvalidCoupon;
            }
        }

        //to Apply Promotions Filter
        protected virtual List<PromotionModel> ApplyPromotionsFilter(List<PromotionModel> promotionsFromCache, int? currentPortalId, int? currentProfileId)
        {
            //Check if no coupons applied for current shopping cart.
            if (HelperUtility.IsNull(_shoppingCart.Coupons) || _shoppingCart.Coupons.Count <= 0)
            {
                promotionsFromCache = promotionsFromCache.Where(promotion => (DateTime.Today.Date >= promotion.StartDate && DateTime.Today.Date <= promotion.EndDate) &&
                            (promotion.ProfileId == currentProfileId || promotion.ProfileId == null || currentProfileId == null) &&
                            (promotion.PortalId == currentPortalId || promotion.PortalId == null) &&
                            (promotion.IsCouponRequired == false) &&
                            promotion.PromotionType.ClassType.Equals("CART", StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.DisplayOrder).ToList();
            }
            else
            {
                List<PromotionModel> cartPromotionsFromCache = promotionsFromCache;

                promotionsFromCache = promotionsFromCache?.Where(promotion => (DateTime.Today.Date >= promotion.StartDate && DateTime.Today.Date <= promotion.EndDate) &&
                                (promotion.ProfileId == currentProfileId || promotion.ProfileId == null || currentProfileId == null) &&
                                (promotion.PortalId == currentPortalId || promotion.PortalId == null) &&
                                (promotion.IsCouponRequired == false) &&
                                promotion.PromotionType.ClassType.Equals("CART", StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.DisplayOrder).ToList() ?? new List<PromotionModel>();

                newPromotionsFromCache = new List<PromotionModel>();

                //to add all promotions without coupons
                foreach (PromotionModel promo in promotionsFromCache ?? new List<PromotionModel>())
                {
                    if (!promo.IsCouponRequired.GetValueOrDefault())
                        newPromotionsFromCache.Add(promo);
                }

                foreach (Libraries.ECommerce.Entities.ZnodeCoupon coupon in _shoppingCart.Coupons)
                {
                    if (!string.IsNullOrEmpty(coupon.Coupon))
                    {
                        PromotionModel promotionsWithCoupon = promotionHelper.GetCouponsPromotion(cartPromotionsFromCache, coupon.Coupon, currentPortalId, currentProfileId, _shoppingCart.OrderId);
                        if (HelperUtility.IsNotNull(promotionsWithCoupon))
                        {
                            newPromotionsFromCache.Add(promotionsWithCoupon);
                        }
                    }
                }

                foreach (Libraries.ECommerce.Entities.ZnodeCoupon coupon in _shoppingCart.Coupons)
                {
                    coupon.AllowsMultipleCoupon = promotionHelper.AllowsMultipleCoupon(coupon.Coupon, currentPortalId, currentProfileId);
                }
                _shoppingCart.CartAllowsMultiCoupon = Convert.ToBoolean(_shoppingCart?.Coupons?.FirstOrDefault()?.AllowsMultipleCoupon);
                promotionsFromCache = newPromotionsFromCache;
            }
            return promotionsFromCache;
        }

        //Bind cached promotion if any.
        protected virtual void BuildPromotionsList(List<PromotionModel> promotionsFromCache, int? currentPortalId, int? currentProfileId)
        {
            foreach (PromotionModel promotion in promotionsFromCache ?? new List<PromotionModel>())
            {
                ZnodePromotionBag promoBag = BuildPromotionBag(promotion, currentPortalId, currentProfileId);
                AddPromotionType(promotion, promoBag);
            }
        }

        //Bind ZnodePromotion data to ZnodePromotionBag.
        protected virtual ZnodePromotionBag BuildPromotionBag(PromotionModel promotion, int? currentPortalId, int? currentProfileId)
        {
            ZnodePromotionBag promotionBag = new ZnodePromotionBag();
            promotionBag.PromotionId = promotion.PromotionId;
            promotionBag.PortalId = currentPortalId;
            promotionBag.ProfileId = currentProfileId;
            promotionBag.Discount = promotion.Discount.GetValueOrDefault();
            promotionBag.MinimumOrderAmount = promotion.OrderMinimum.GetValueOrDefault(0);
            promotionBag.RequiredProductMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.RequiredProductId = promotion.ReferralPublishProductId.GetValueOrDefault(0);
            promotionBag.DiscountedProductQuantity = promotion.PromotionProductQuantity.GetValueOrDefault(1);
            promotionBag.RequiredBrandMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.RequiredCatalogMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.RequiredCategoryMinimumQuantity = promotion.QuantityMinimum.GetValueOrDefault(1);
            promotionBag.IsCouponAllowedWithOtherCoupons = promotion.IsAllowedWithOtherCoupons;
            promotionBag.PromoCode = promotion.PromoCode;
            promotionBag.IsUnique = promotion.IsUnique;
            promotionBag.PromotionName = promotion.Name;
            promotionBag.PromotionTypeId = promotion.PromotionTypeId;
            if (promotion.IsCouponRequired.GetValueOrDefault())
            {
                promotionBag.Coupons = promotionHelper.GetPromotionCoupons(promotion.PromotionId, GetCartCoupons());
                promotionBag.PromotionMessage = promotion.PromotionMessage;
            }
            return promotionBag;
        }

        //Add Promotion Type in promotionBag
        protected virtual void AddPromotionType(PromotionModel promotion, ZnodePromotionBag promotionBag)
        {
            try
            {
                IZnodeCartPromotionType cartPromo = GetPromotionTypeInstance<IZnodeCartPromotionType>(promotion);

                if (HelperUtility.IsNotNull(cartPromo))
                {
                    cartPromo.Precedence = promotion.DisplayOrder.GetValueOrDefault();

                    if (HelperUtility.IsNotNull(promotionBag.Coupons) && promotionBag.Coupons.Count > 0)
                        cartPromo.IsPromoCouponAvailable = true;
                    
                    cartPromo.Bind(_shoppingCart, promotionBag);
                    _cartPromotions.Add(cartPromo);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error while instantiating promotion type: " + promotion?.PromotionType + " " + ex.ToString(), ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Verbose);
            }
        }

        //to get cart coupons
        protected virtual string GetCartCoupons()
        {
            return string.Join(",", _shoppingCart.Coupons.Select(coupon => coupon.Coupon));
        }


        //method to check Shipping promotion. 
        protected virtual bool IsShippingPromotionDiscount(string promotionClassName)
        {
            return (!string.IsNullOrEmpty(promotionClassName)
                                    && (string.Equals(promotionClassName, ZnodeConstant.AmountOffShipping, StringComparison.InvariantCultureIgnoreCase)
                                    || string.Equals(promotionClassName, ZnodeConstant.AmountOffShippingWithCarrier, StringComparison.InvariantCultureIgnoreCase)
                                    || string.Equals(promotionClassName, ZnodeConstant.PercentOffShipping, StringComparison.InvariantCultureIgnoreCase)
                                    || string.Equals(promotionClassName, ZnodeConstant.PercentOffShippingWithCarrier, StringComparison.InvariantCultureIgnoreCase)));
        }       
        #endregion
    }
}