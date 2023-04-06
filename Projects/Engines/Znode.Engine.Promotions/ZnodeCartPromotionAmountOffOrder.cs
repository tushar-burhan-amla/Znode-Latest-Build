using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionAmountOffOrder : ZnodeCartPromotionType
    {
        #region Variable
        protected readonly IZnodePromotionHelper promotionHelper = null;
        #endregion

        #region Constructor
        public ZnodeCartPromotionAmountOffOrder()
        {
            Name = "Amount Off Order";
            Description = "Applies an amount off an entire order; affects the shopping cart.";
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
        /// Calculates the amount off an order.
        /// </summary>
        public override void Calculate(int? couponIndex, List<PromotionModel> allPromotions)
        {
            decimal itemCount = GetCartItemsCount();
            if (itemCount <= 0)
                return;

            decimal subTotal = GetCartSubTotal(ShoppingCart);

            OrderBy = nameof(PromotionModel.OrderMinimum);
            ApplicablePromolist = ZnodePromotionManager.GetPromotionsByType(ZnodeConstant.PromotionClassTypeCart, ClassName, allPromotions, OrderBy);
            
            if (Equals(PromotionBag.Coupons, null))
            {
                if (PromotionBag.MinimumOrderAmount <= subTotal && itemCount > 0)
                {
                    List<PromotionModel> Promotionlist = promotionHelper.GetMostApplicablePromoList(ApplicablePromolist, subTotal, false);
                    if (!promotionHelper.IsApplicablePromotion(Promotionlist, PromotionBag.PromoCode, ShoppingCart.SubTotal, true, false))
                        return;

                    ApplyDiscount(PromotionBag.Discount);
                    ShoppingCart.IsAnyPromotionApplied = true;
                }
            }
            else if (PromotionBag?.Coupons?.Count > 0)
            {
                bool isCouponValid = ValidateCoupon(couponIndex);
                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        // Apply the discount
                        if (PromotionBag.MinimumOrderAmount <= subTotal && isCouponValid && itemCount > 0)
                        {
                            if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, ShoppingCart.SubTotal, true, true))
                                return;

                            bool isDiscountApplied = ApplyDiscount(PromotionBag.Discount, coupon.Code);
                            SetCouponApplied(coupon.Code);
                            ShoppingCart.Coupons[couponIndex.Value].CouponApplied = IsCouponApplicableForLineItem(coupon.Code);
                        }
                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                        {
                            break;
                        }
                    }
                }
                AddPromotionMessage(couponIndex.Value);
            }
        }
        #endregion

        #region Private Methods
        //to get all items count in cart
        private decimal GetCartItemsCount()
        {
            decimal count = 0;

            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                decimal lineItemCount = 0;

                if (cartItem.Product.FinalPrice > 0 && (cartItem?.Product?.ZNodeGroupProductCollection?.Count == 0))
                {
                    lineItemCount += 1;
                }

                foreach (ZnodeProductBaseEntity addon in cartItem.Product.ZNodeAddonsProductCollection)
                {
                    if (addon.FinalPrice > 0.0M)
                        lineItemCount++;
                }

                if (cartItem?.Product?.ZNodeGroupProductCollection?.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                    {
                        if (group.FinalPrice > 0.0M)
                            lineItemCount += group.SelectedQuantity;
                    }
                }
                if (cartItem?.Product?.ZNodeConfigurableProductCollection?.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity config in cartItem.Product.ZNodeConfigurableProductCollection)
                    {
                        if (config.FinalPrice > 0.0M)
                            lineItemCount += config.SelectedQuantity;
                    }
                }
                else
                {
                    lineItemCount = (lineItemCount * cartItem.Quantity);
                }

                count += (lineItemCount);
            }
            return count;
        }

        //apply  calculated discount to shopping cart item
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
        #endregion
    }
}
