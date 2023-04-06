using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionAmountOffShippingWithCarrier : ZnodeCartPromotionType
    {
        #region Private Variable
        protected readonly IZnodePromotionHelper promotionHelper = null;
        #endregion

        #region Constructor
        public ZnodeCartPromotionAmountOffShippingWithCarrier()
        {
            Name = "Amount Off Shipping With Carrier";
            Description = "Applies an amount off shipping with carrier for an order; affects the shopping cart.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountAmount);
            Controls.Add(ZnodePromotionControl.MinimumOrderAmount);
            Controls.Add(ZnodePromotionControl.Coupon);
            promotionHelper = GetService<IZnodePromotionHelper>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the amount off shipping for the order.
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
            decimal discountAmount = 0;
            if (PromotionBag.MinimumOrderAmount <= subTotal && (ShoppingCart.ShippingCost - ShoppingCart.Shipping.ShippingDiscount) > 0)
            {
                //If the shipping amount less than shipping promotion then only apply shipping amount equals discount
                ShoppingCart.Shipping.ShippingDiscount += (ShoppingCart.ShippingCost <= PromotionBag.Discount) ? ShoppingCart.ShippingCost : PromotionBag.Discount;
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
                discountAmount = GetAmountOffShippingDiscount(ShoppingCart.ShippingCost, PromotionBag.Discount);
                SetShippingLevelDiscount(discountAmount, couponCode);

            }
            else if (ShoppingCart.IsAnyPromotionApplied)
            {
                ShoppingCart.IsAnyPromotionApplied = true;
                ShoppingCart.Shipping.ShippingDiscountDescription = PromotionBag.PromoCode;
                ShoppingCart.Shipping.ShippingDiscountType = Convert.ToInt32(OrderDiscountTypeEnum.PROMOCODE);
                discountAmount = GetAmountOffShippingDiscount(ShoppingCart.ShippingCost, PromotionBag.Discount);
                SetShippingLevelDiscount(discountAmount);
            }
        }

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
