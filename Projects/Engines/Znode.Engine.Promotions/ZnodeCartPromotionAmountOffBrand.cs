using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionAmountOffBrand : ZnodeCartPromotionType
    {
        #region Variable
        protected readonly IZnodePromotionHelper promotionHelper = null;
        private List<PromotionCartItemQuantity> promobrandSkus;
        #endregion

        #region Constructor
        public ZnodeCartPromotionAmountOffBrand()
        {
            Name = "Amount Off Brand";
            Description = "Applies an amount off products for a particular brand or manufacturer; affects the shopping cart.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountAmount);
            Controls.Add(ZnodePromotionControl.RequiredBrand);
            Controls.Add(ZnodePromotionControl.RequiredBrandMinimumQuantity);
            Controls.Add(ZnodePromotionControl.MinimumOrderAmount);
            Controls.Add(ZnodePromotionControl.Coupon);

            promotionHelper = GetService<IZnodePromotionHelper>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the amount off products for a particular brand or manufacturer in the shopping cart.
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
                        ShoppingCart.IsAnyPromotionApplied = ApplyLineItemDiscount(isCouponValid, couponIndex, cartItem.Product, promotionsBrand, cartItem.Quantity, configurable, true);
                    }
                }
                //to apply promotion for group product
                else if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                    {
                        if (!ApplyParentItemDiscount(isCouponValid, couponIndex, cartItem.Product, group, promotionsBrand, cartItem.Quantity))
                        {
                            ShoppingCart.IsAnyPromotionApplied = ApplyLineItemDiscount(isCouponValid, couponIndex, group, promotionsBrand, cartItem.Quantity);
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
            decimal qtyOrdered = GetCartQuantity();

            if (Equals(PromotionBag.Coupons, null))
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, qtyOrdered, ShoppingCart.SubTotal, false))
                    return;

                if (PromotionBag.MinimumOrderAmount <= subTotal
                 && cartItem.Product.ZNodeConfigurableProductCollection.Count == 0
                  && cartItem.Product.ZNodeGroupProductCollection.Count == 0
                  && IsRequiredMinimumQuantity(cartItem.Product.SKU))
                {
                    if (IsDiscountApplicable(cartItem.Product.DiscountAmount, cartItem.Product.FinalPrice))
                    {
                        decimal maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(PromotionBag.Discount, cartItem.Quantity, cartItem.Product);
                        cartItem.Product.DiscountAmount += maxApplicableDiscount;
                        cartItem.Product.OrdersDiscount = SetOrderDiscountDetails(PromotionBag.PromoCode, maxApplicableDiscount, OrderDiscountTypeEnum.PROMOCODE, cartItem.Product.OrdersDiscount);
                        SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
                    }
                    ShoppingCart.IsAnyPromotionApplied = true;
                }
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
                        if (PromotionBag.MinimumOrderAmount <= subTotal
                        && cartItem.Product.ZNodeGroupProductCollection.Count == 0
                        && IsDiscountApplicable(cartItem.Product.DiscountAmount, cartItem.Product.FinalPrice)
                        && cartItem.Product.ZNodeConfigurableProductCollection.Count == 0
                        && IsRequiredMinimumQuantity(cartItem.Product.SKU))
                        {
                            decimal maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(PromotionBag.Discount, cartItem.Quantity, cartItem.Product);
                            cartItem.Product.DiscountAmount += maxApplicableDiscount;
                            cartItem.Product.OrdersDiscount = SetOrderDiscountDetails(coupon.Code, maxApplicableDiscount, OrderDiscountTypeEnum.COUPONCODE, cartItem.Product.OrdersDiscount);
                            SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
                            SetCouponApplied(coupon.Code);
                            ShoppingCart.Coupons[couponIndex.Value].CouponApplied = true;
                        }

                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                        {
                            break;
                        }
                    }
                }
            }
        }

        //to apply parent item discount
        private bool ApplyParentItemDiscount(bool isCouponValid, int? couponIndex, ZnodeProductBaseEntity product, ZnodeProductBaseEntity childProduct, List<BrandModel> promotionsBrand, decimal quantity)
        {
            bool discountApplied = false;
            if (Equals(PromotionBag.Coupons, null))
            {
                discountApplied = ApplyBrandDiscountToChildProduct(product, quantity, promotionsBrand, childProduct);
            }
            else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
            {
                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && ShoppingCart.Coupons[couponIndex.Value].Coupon.Equals(coupon.Code, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        discountApplied = ApplyBrandDiscountToChildProduct(product, quantity, promotionsBrand, childProduct, coupon.Code);
                        if (discountApplied)
                        {
                            SetCouponApplied(coupon.Code);
                            ShoppingCart.Coupons[couponIndex.Value].CouponApplied = IsCouponApplicableForLineItem(coupon.Code);
                        }
                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                        {
                            break;
                        }
                    }
                }
            }
            return discountApplied;
        }

        //to apply line item discount
        private bool ApplyLineItemDiscount(bool isCouponValid, int? couponIndex, ZnodeProductBaseEntity product, List<BrandModel> promotionsBrand, decimal quantity, ZnodeProductBaseEntity childProduct = null, bool isConfigurableProduct = false)
        {
            bool discountApplied = false;
            if (Equals(PromotionBag.Coupons, null))
            {
                discountApplied = ApplyBrandDiscountToChildProduct(product, quantity, promotionsBrand, childProduct, string.Empty, isConfigurableProduct);
            }
            else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
            {
                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        discountApplied = ApplyBrandDiscountToChildProduct(product, quantity, promotionsBrand, childProduct, coupon.Code, isConfigurableProduct);
                        if (discountApplied)
                        {
                            SetCouponApplied(coupon.Code);
                            ShoppingCart.Coupons[couponIndex.Value].CouponApplied = IsCouponApplicableForLineItem(coupon.Code);
                        }
                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                        {
                            break;
                        }
                    }
                }
            }
            return discountApplied;
        }

        //to  apply brand discount to child product
        private bool ApplyBrandDiscountToChildProduct(ZnodeProductBaseEntity product, decimal quantity, List<BrandModel> promotionsBrand, ZnodeProductBaseEntity childProduct = null, string couponCode = "", bool isConfigurableProduct = false)
        {
            bool isDiscountApplied = false;
            foreach (BrandModel promotion in promotionsBrand)
            {
                decimal subTotal = GetCartSubTotal(ShoppingCart);
                if (string.Equals(product.BrandCode, promotion.BrandCode, StringComparison.OrdinalIgnoreCase)
                     && (product.FinalPrice > 0.0M
                     && PromotionBag.MinimumOrderAmount <= subTotal)
                     && IsDiscountApplicable(product.DiscountAmount, product.FinalPrice)
                     && IsRequiredMinimumQuantity(product.SKU))
                {
                    decimal maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(PromotionBag.Discount, quantity, product, childProduct);
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
                    isDiscountApplied = true;
                }
            }
            return isDiscountApplied;
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
