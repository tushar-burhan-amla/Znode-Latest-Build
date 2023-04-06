using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionPercentOffOrder : ZnodeCartPromotionType
    {
        #region  Variable
        protected readonly IZnodePromotionHelper promotionHelper = null;
        #endregion

        #region Constructor
        public ZnodeCartPromotionPercentOffOrder()
        {
            Name = "Percent Off Order";
            Description = "Applies a percent off an entire order; affects the shopping cart.";
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
        /// Calculates the percent off an order.
        /// </summary>
        public override void Calculate(int? couponIndex, List<PromotionModel> allPromotions)
        {
            decimal subTotal = GetCartSubTotal(ShoppingCart);
            decimal discountedSubtotal = ShoppingCart.GetMaximumApplicableDiscountOnSubtotal();
            OrderBy = nameof(PromotionModel.OrderMinimum);
            ApplicablePromolist = ZnodePromotionManager.GetPromotionsByType(ZnodeConstant.PromotionClassTypeCart, ClassName, allPromotions, OrderBy);
            if (Equals(PromotionBag.Coupons, null))
            {
                decimal discount = PromotionBag.Discount / 100;
                discount = discountedSubtotal * discount;
                if (PromotionBag.MinimumOrderAmount <= subTotal)
                {
                    List<PromotionModel> Promotionlist = promotionHelper.GetMostApplicablePromoList(ApplicablePromolist, ShoppingCart.SubTotal, false);
                    if (!promotionHelper.IsApplicablePromotion(Promotionlist, PromotionBag.PromoCode, ShoppingCart.SubTotal, true, false))
                        return;

                    ApplyDiscount(discount);
                    ShoppingCart.IsAnyPromotionApplied = true;
                }
                else
                {
                    RemoveDiscount(discount);
                    ShoppingCart.IsAnyPromotionApplied = true;
                }
            }
            else if (!Equals(PromotionBag.Coupons, null))
            {
                bool isCouponValid = ValidateCoupon(couponIndex);

                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        if (PromotionBag.MinimumOrderAmount <= subTotal && isCouponValid)
                        {
                            if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, ShoppingCart.SubTotal, true, true))
                                return;

                            decimal discount = PromotionBag.Discount / 100;
                            discount = discountedSubtotal * discount;
                            bool isDiscountApplied = ApplyDiscount(discount, coupon.Code);
                            SetCouponApplied(coupon.Code);
                            ShoppingCart.Coupons[couponIndex.Value].CouponApplied = IsCouponApplicableForLineItem(coupon.Code);
                        }

                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                            break;
                    }
                }

                AddPromotionMessage(couponIndex);
            }
        }
        #endregion

        #region Private Methods
        private bool ApplyDiscount(decimal discount, string couponCode = "")
        {
            bool isDiscountApplied = false;
            if (discount > 0)
            {
                OrderDiscountModel orderDiscountModel = new OrderDiscountModel();
                orderDiscountModel.DiscountMultiplier = ShoppingCart.GetDiscountMultiplier(discount);
                orderDiscountModel.DiscountCode = GetDiscountCode(PromotionBag.PromoCode, couponCode);
                OrderDiscountTypeEnum discountType = GetDiscountType(couponCode);
                orderDiscountModel.OmsDiscountTypeId = (int)discountType;
                orderDiscountModel.DiscountLevelTypeId = (int)DiscountLevelTypeIdEnum.OrderLevel;
                orderDiscountModel.PromotionName = PromotionBag?.PromotionName;
                orderDiscountModel.PromotionTypeId = PromotionBag?.PromotionTypeId;
                orderDiscountModel.PromotionMessage = PromotionBag?.PromotionMessage;
                decimal appliedDiscount = ShoppingCart.DistributeOrderDiscountAmount(orderDiscountModel);
                ShoppingCart.OrderLevelDiscount += appliedDiscount;
                ShoppingCart.OrderLevelDiscountDetails = SetOrderDiscountDetails(orderDiscountModel.DiscountCode, appliedDiscount, discountType, ShoppingCart.OrderLevelDiscountDetails, DiscountLevelTypeIdEnum.OrderLevel, orderDiscountModel.DiscountMultiplier);
                isDiscountApplied = true;
            }
            return isDiscountApplied;
        }

        // To add order level discount Amount 


        private void RemoveDiscount(decimal discount)
        {
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                decimal finalPrice = cartItem.PromotionalPrice;
                decimal lineItemDiscount = finalPrice * discount;

                if (cartItem.Product.DiscountAmount > lineItemDiscount)
                    cartItem.Product.DiscountAmount -= lineItemDiscount;

                cartItem.Product.DiscountAmount = cartItem.Product.DiscountAmount < 0 ? 0 : cartItem.Product.DiscountAmount;

                foreach (ZnodeProductBaseEntity addon in cartItem.Product.ZNodeAddonsProductCollection)
                {
                    if (addon.FinalPrice > 0.0M)
                    {
                        lineItemDiscount = addon.FinalPrice * discount;
                        if (addon.DiscountAmount > lineItemDiscount)
                        {
                            addon.DiscountAmount -= lineItemDiscount;
                        }
                        addon.DiscountAmount = addon.DiscountAmount < 0 ? 0 : addon.DiscountAmount;
                    }
                }

                foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                {
                    if (group.FinalPrice > 0.0M)
                    {
                        lineItemDiscount = group.FinalPrice * discount;
                        if (group.DiscountAmount > lineItemDiscount)
                        {
                            group.DiscountAmount -= lineItemDiscount;
                        }
                        group.DiscountAmount = group.DiscountAmount < 0 ? 0 : group.DiscountAmount;
                    }
                }

            }
        }
        #endregion
    }
}
