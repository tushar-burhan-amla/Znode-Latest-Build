using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionPercentOffBrand : ZnodeCartPromotionType
    {
        #region Variable
        protected readonly IZnodePromotionHelper promotionHelper =null;
        private List<PromotionCartItemQuantity> promobrandSkus;
        #endregion

        #region Constructor
        public ZnodeCartPromotionPercentOffBrand()
        {
            Name = "Percent Off Brand";
            Description = "Applies a percent off products for a particular brand or manufacturer; affects the shopping cart.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountPercent);
            Controls.Add(ZnodePromotionControl.RequiredBrand);
            Controls.Add(ZnodePromotionControl.RequiredBrandMinimumQuantity);
            Controls.Add(ZnodePromotionControl.MinimumOrderAmount);
            Controls.Add(ZnodePromotionControl.Coupon);

            promotionHelper = GetService<IZnodePromotionHelper>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the percent off products for a particular brand or manufacturer in the shopping cart.
        /// </summary>
        public override void Calculate(int? couponIndex, List<PromotionModel> allPromotions)
        {
            ApplicablePromolist = ZnodePromotionManager.GetPromotionsByType(ZnodeConstant.PromotionClassTypeCart, ClassName, allPromotions, OrderBy);

            bool isCouponValid = false;
            if (!Equals(couponIndex, null))
            {
                isCouponValid = ValidateCoupon(couponIndex);
            }

            //to get all product of promotion by PromotionId
            List<BrandModel> promotionsBrand = promotionHelper.GetPromotionBrands(PromotionBag.PromotionId);

            //to set all promotions brand wise sku to calculate each sku quantity
            promobrandSkus = promotionHelper.SetPromotionBrandSKUQuantity(promotionsBrand, ShoppingCart);

            // Loop through each cart Item
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                //to apply promotion for configurable product
                if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity configurable in cartItem.Product.ZNodeConfigurableProductCollection)
                    {
                        ApplyLineItemDiscount(couponIndex, cartItem.Quantity, cartItem.Product, promotionsBrand, configurable, true);
                    }
                }
                //to apply promotion for group product
                else if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                    {
                        if (!ApplyLineItemDiscount(couponIndex, cartItem.Quantity, cartItem.Product, promotionsBrand, group))
                        {
                            ApplyLineItemDiscount(couponIndex, cartItem.Quantity, group, promotionsBrand);
                        }
                    }
                }
                else
                {
                    foreach (BrandModel brand in promotionsBrand)
                    {
                        if (string.Equals(brand.BrandCode, cartItem.Product.BrandCode, StringComparison.OrdinalIgnoreCase))
                        {
                            ApplyDiscount(isCouponValid, couponIndex, cartItem);
                            break;
                        }
                    }
                }
            }
            AddPromotionMessage(couponIndex);
        }
        #endregion

        #region Private Method
        //to apply discount
        private void ApplyDiscount(bool isCouponValid, int? couponIndex, ZnodeShoppingCartItem cartItem)
        {
            decimal subTotal = GetCartSubTotal(ShoppingCart);

            if (Equals(PromotionBag.Coupons, null))
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, cartItem.Quantity, ShoppingCart.SubTotal, false))
                    return;

                ApplyProductDiscount(couponIndex, subTotal, cartItem);
            }
            else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
            {
                decimal cartQty = promobrandSkus.Sum(x => x.Quantity);
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, cartQty, ShoppingCart.SubTotal, true))
                    return;

                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        ApplyProductDiscount(couponIndex, subTotal, cartItem, coupon.Code);
                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                        {
                            break;
                        }
                    }
                }
            }
        }

        //to apply product discount
        private void ApplyProductDiscount(int? couponIndex, decimal subTotal, ZnodeShoppingCartItem cartItem, string couponCode = "")
        {
            bool discountApplied = false;
            if (PromotionBag.MinimumOrderAmount <= subTotal
                 && cartItem.Product.ZNodeConfigurableProductCollection.Count == 0
                     && cartItem.Product.ZNodeGroupProductCollection.Count == 0
                     && IsRequiredMinimumQuantity(cartItem.Product.SKU))
            {
                decimal discountedPrice = ShoppingCart.GetDiscountedPrice(cartItem.Product, cartItem.Quantity);
                decimal discount = discountedPrice * (PromotionBag.Discount / 100);
                decimal maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(discount, cartItem.Quantity, cartItem.Product);
                cartItem.Product.DiscountAmount += maxApplicableDiscount;
                cartItem.Product.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, GetDiscountType(couponCode), cartItem.Product.OrdersDiscount);
                SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
                discountApplied = true;
            }

            if (!string.IsNullOrEmpty(couponCode) && discountApplied)
            {
                SetCouponApplied(couponCode);
                ShoppingCart.Coupons[couponIndex.Value].CouponApplied = IsCouponApplicableForLineItem(couponCode);
            }
            else
            {
                ShoppingCart.IsAnyPromotionApplied = discountApplied;
            }
        }
        //to apply line item discount
        private bool ApplyLineItemDiscount(int? couponIndex, decimal quantity, ZnodeProductBaseEntity product, List<BrandModel> promotionsBrand, ZnodeProductBaseEntity childProduct = null, bool isConfigurableProduct = false)
        {
            bool discountApplied = false;
            decimal subTotal = GetCartSubTotal(ShoppingCart);
            foreach (BrandModel promotion in promotionsBrand)
            {
                if (string.Equals(product.BrandCode, promotion.BrandCode, StringComparison.OrdinalIgnoreCase))
                {
                    if (Equals(PromotionBag.Coupons, null))
                    {
                        discountApplied = ApplyChildItemDiscount(couponIndex, subTotal, quantity, product, childProduct, string.Empty, isConfigurableProduct);
                    }
                    else
                    {
                        foreach (CouponModel coupon in PromotionBag.Coupons)
                        {
                            if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                            {
                                discountApplied = ApplyChildItemDiscount(couponIndex, subTotal, quantity, product, childProduct, coupon.Code, isConfigurableProduct);
                                if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return discountApplied;
        }

        //to apply child item discount
        private bool ApplyChildItemDiscount(int? couponIndex, decimal subTotal, decimal quantity, ZnodeProductBaseEntity product, ZnodeProductBaseEntity childProduct = null, string couponCode = "", bool isConfigurableProduct = false)
        {
            bool discountApplied = false;
            decimal maxApplicableDiscount = 0;
            decimal discountedPrice = ShoppingCart.GetDiscountedPrice(product, quantity, childProduct);

            if (IsRequiredMinimumQuantity(product.SKU) && PromotionBag.MinimumOrderAmount <= subTotal)
            {
                decimal discount = discountedPrice * (PromotionBag.Discount / 100);
                maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(discount, quantity, product, childProduct);
                if (childProduct == null)
                {
                    product.DiscountAmount += maxApplicableDiscount;
                    product.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, GetDiscountType(couponCode), product.OrdersDiscount);
                }
                else
                {
                    if (isConfigurableProduct)
                    {
                        product.DiscountAmount += maxApplicableDiscount;
                        product.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, GetDiscountType(couponCode), product.OrdersDiscount);
                    }
                    else
                    {
                        childProduct.DiscountAmount += maxApplicableDiscount;
                        childProduct.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, GetDiscountType(couponCode), product.OrdersDiscount);
                    }

                }

                discountApplied = true;
                ShoppingCart.IsAnyPromotionApplied = true;
            }

            if (!string.IsNullOrEmpty(couponCode))
            {
                SetCouponApplied(couponCode);
                ShoppingCart.Coupons[couponIndex.Value].CouponApplied = IsCouponApplicableForLineItem(couponCode);
                discountApplied = true;
            }


            return discountApplied;
        }

        //to check minimum quantity of promotion in the shopping cart item
        private bool IsRequiredMinimumQuantity(string sku)
        {
            bool result = false;
            if (promobrandSkus?.Count > 0)
            {
                decimal cartQty = promobrandSkus.Sum(x => x.Quantity);
                if (cartQty >= PromotionBag.RequiredBrandMinimumQuantity)
                {
                    result = promobrandSkus.Any(x => x.SKU.Contains(sku));
                }
            }
            return result;
        }
        #endregion
    }
}
