using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using System;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionPercentOffShippingWithCarrier : ZnodeCartPromotionType
    {
        #region Variable
        protected readonly IZnodePromotionHelper promotionHelper = null;
        #endregion

        #region Constructor
        public ZnodeCartPromotionPercentOffShippingWithCarrier()
        {
            Name = "Percent Off Shipping With Carrier";
            Description = "Applies a percent off shipping with carrier for an order; affects the shopping cart.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountPercent);
            Controls.Add(ZnodePromotionControl.MinimumOrderAmount);
            Controls.Add(ZnodePromotionControl.Coupon);
            promotionHelper = GetService<IZnodePromotionHelper>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the percent off shipping for the order.
        /// </summary>       
        public override void Calculate(int? couponIndex, List<PromotionModel> allPromotions)
        {
            //to check promotion is applicable for this shipping method
            if (!IsPromotionApplicable())
                return;

            OrderBy = nameof(PromotionModel.OrderMinimum);
            ApplicablePromolist = ZnodePromotionManager.GetPromotionsByType(ZnodeConstant.PromotionClassTypeCart, ClassName, allPromotions, OrderBy);


            bool isCouponValid = false;
            if (!Equals(couponIndex, null))
            {
                isCouponValid = ValidateCoupon(couponIndex);
            }

            if (Equals(PromotionBag.Coupons, null))
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, ShoppingCart.SubTotal, true, false))
                    return;

                ApplyDiscount(couponIndex);
            }
            else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, ShoppingCart.SubTotal, true, true))
                    return;

                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        ApplyDiscount(couponIndex, coupon.Code);
                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                        {
                            break;
                        }
                    }
                }
                AddPromotionMessage(couponIndex);
            }
        }
        #endregion

        #region Private Method
        //to Apply Discount
        private void ApplyDiscount(int? couponIndex, string couponCode = "")
        {
            decimal subTotal = GetCartSubTotal(ShoppingCart);
            decimal shippingCost = ShoppingCart.ShippingCost;
            decimal discountAmount = GetPercentOffShippingDiscount(shippingCost, ShoppingCart.Shipping.ShippingDiscount, PromotionBag.Discount);

            if (PromotionBag.MinimumOrderAmount <= subTotal && (ShoppingCart.ShippingCost - ShoppingCart.Shipping.ShippingDiscount) > 0)
            {
                ShoppingCart.Shipping.ShippingDiscount += (shippingCost - ShoppingCart.Shipping.ShippingDiscount) * (PromotionBag.Discount / 100);
                ShoppingCart.IsAnyPromotionApplied = true;
            }
            else
            {
                ShoppingCart.IsAnyPromotionApplied = false;
            }

            if (!string.IsNullOrEmpty(couponCode) && ShoppingCart.IsAnyPromotionApplied)
            {
                SetCouponApplied(couponCode);
                ShoppingCart.Coupons[couponIndex.Value].CouponApplied = true;
                ShoppingCart.Shipping.ShippingDiscountDescription = couponCode;
                ShoppingCart.Shipping.ShippingDiscountType = Convert.ToInt32(OrderDiscountTypeEnum.COUPONCODE);
                SetShippingLevelDiscount(discountAmount, couponCode);
            }
            else if (ShoppingCart.IsAnyPromotionApplied)
            {
                ShoppingCart.IsAnyPromotionApplied = true;
                ShoppingCart.Shipping.ShippingDiscountDescription = PromotionBag.PromoCode;
                ShoppingCart.Shipping.ShippingDiscountType = Convert.ToInt32(OrderDiscountTypeEnum.PROMOCODE);
                SetShippingLevelDiscount(discountAmount);
            }
            //to check free shipping is applied 
            ShoppingCart.Shipping.ShippingDiscountApplied = FreeShippingApplied(ShoppingCart.IsAnyPromotionApplied);
        }

        //to check percent of shipping promotion is applied and discount amount is greater than or equals to 100
        private bool FreeShippingApplied(bool promoapplied)
          => promoapplied && PromotionBag.Discount >= 100 && ShoppingCart.Shipping.ShippingID > 0;

        //to get shopping cart shipping id
        private int GetCartShippingId()
            => ShoppingCart?.Shipping?.ShippingID ?? 0;

        //to check this promotion is applicable for shopping cart shipping option if yes the return true 
        private bool IsPromotionApplicable()
        {
            int cartShippingId = GetCartShippingId();
            if (cartShippingId > 0)
            {
                List<ShippingModel> promotionsShipping = promotionHelper.GetPromotionShipping(PromotionBag.PromotionId);
                if (HelperUtility.IsNotNull(promotionsShipping))
                {
                    foreach (ShippingModel shipping in promotionsShipping)
                    {
                        if (shipping.ShippingId == cartShippingId)
                            return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}

