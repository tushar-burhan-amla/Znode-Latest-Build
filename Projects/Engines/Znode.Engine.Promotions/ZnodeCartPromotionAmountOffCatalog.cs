using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionAmountOffCatalog : ZnodeCartPromotionType
    {
        #region Variable
        protected readonly IZnodePromotionHelper promotionHelper = null;
        #endregion

        #region Constructor
        public ZnodeCartPromotionAmountOffCatalog()
        {
            Name = "Amount Off Catalog";
            Description = "Applies an amount off products for a particular catalog; affects the shopping cart.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountAmount);
            Controls.Add(ZnodePromotionControl.RequiredCatalog);
            Controls.Add(ZnodePromotionControl.RequiredCatalogMinimumQuantity);
            Controls.Add(ZnodePromotionControl.MinimumOrderAmount);
            Controls.Add(ZnodePromotionControl.Coupon);
            promotionHelper = GetService<IZnodePromotionHelper>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the amount off products for a particular catalog in the shopping cart.
        /// </summary>
        public override void Calculate(int? couponIndex, List<PromotionModel> allPromotions)
        {
            ApplicablePromolist = ZnodePromotionManager.GetPromotionsByType(ZnodeConstant.PromotionClassTypeCart, ClassName, allPromotions, OrderBy);
            bool isCouponValid = false;
            if (!Equals(couponIndex, null))
            {
                isCouponValid = ValidateCoupon(couponIndex);
            }

            //to get all catalog of promotion by PromotionId
            List<CatalogModel> promotionsCatalog = promotionHelper.GetPromotionCatalogs(PromotionBag.PromotionId);

            bool isPromotionApplied = false;
            // Loop through each cart Item
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                 isPromotionApplied = false;

                int productId = cartItem.Product.ProductID;
                if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0)
                {
                    productId = cartItem.ParentProductId;
                }
                // Get the catalogs for the category
                List<CatalogModel> productCatalogs = promotionHelper.GetCatalogByProduct(productId);

                foreach (CatalogModel promotion in promotionsCatalog)
                {
                   
                    foreach (CatalogModel product in productCatalogs)
                    {
                        if (promotion.PimCatalogId == product.PimCatalogId)
                        {
                            ApplyDiscount(out isPromotionApplied, isCouponValid, couponIndex, cartItem);
                            // Break out of the catalogs loop
                            break;
                        }
                    }
                    if (isPromotionApplied)
                        // Break out of the category loop
                        break;
                }
            }
            AddPromotionMessage(couponIndex);
        }
        #endregion

        #region Private Method

        //to apply discount
        private void ApplyDiscount(out bool isPromotionApplied, bool isCouponValid, int? couponIndex, ZnodeShoppingCartItem cartItem)
        {
            decimal subTotal = GetCartSubTotal(ShoppingCart);
            decimal cartCount = GetCartQuantity();
            bool discountApplied = false;
            if (Equals(PromotionBag.Coupons, null))
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, cartCount, ShoppingCart.SubTotal, false))
                {
                    isPromotionApplied = false;
                    return;
                }
                
                if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                {
                    discountApplied = ApplyDiscountForGroupProducts(cartItem, subTotal, cartCount, OrderDiscountTypeEnum.PROMOCODE);
                }
                else if (PromotionBag.RequiredCatalogMinimumQuantity <= cartCount && PromotionBag.MinimumOrderAmount <= subTotal)
                {
                    // Product discount amount
                    if (IsDiscountApplicable(cartItem.Product.DiscountAmount, cartItem.Product.FinalPrice))
                    {
                        decimal maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(PromotionBag.Discount, cartItem.Quantity, cartItem.Product);
                        SetProductDiscount(cartItem.Product, maxApplicableDiscount, PromotionBag.PromoCode, OrderDiscountTypeEnum.PROMOCODE, cartItem.Product.OrdersDiscount);
                        SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
                    }
                    discountApplied = true;
                    ShoppingCart.IsAnyPromotionApplied = discountApplied;
                }
            }
            else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, cartCount, ShoppingCart.SubTotal, true))
                {
                    isPromotionApplied = false;
                    return;
                }

                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        // Added condition for Group Product to calculate AmountOffCatalog discount for Coupons.
                        if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                        {
                            discountApplied = ApplyDiscountForGroupProducts(cartItem, subTotal, cartCount, OrderDiscountTypeEnum.COUPONCODE, coupon.Code);
                        }
                        else if (PromotionBag.RequiredCatalogMinimumQuantity <= cartCount && PromotionBag.MinimumOrderAmount <= subTotal)
                        {
                            decimal maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(PromotionBag.Discount, cartItem.Quantity, cartItem.Product);
                            SetProductDiscount(cartItem.Product, maxApplicableDiscount, coupon.Code, OrderDiscountTypeEnum.COUPONCODE, cartItem.Product.OrdersDiscount);
                            SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
                            SetCouponApplied(coupon.Code);
                            discountApplied = IsCouponApplicableForLineItem(coupon.Code);
                            ShoppingCart.Coupons[couponIndex.Value].CouponApplied = discountApplied; 
                        }

                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                        {
                            break;
                        }
                    }
                }
            }
            isPromotionApplied = discountApplied;
        }

        private bool ApplyDiscountForGroupProducts(ZnodeShoppingCartItem cartItem, decimal subTotal, decimal cartCount, OrderDiscountTypeEnum discountType, string couponCode="")
        {
            bool discountApplied = false;

            foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
            {
                if (group.FinalPrice > 0.0M && PromotionBag.RequiredCatalogMinimumQuantity <= cartCount && PromotionBag.MinimumOrderAmount <= subTotal)
                {
                    if (IsDiscountApplicable(group.DiscountAmount, group.FinalPrice))
                    {
                        decimal maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(PromotionBag.Discount, cartItem.Quantity, group);
                        SetProductDiscount(group, maxApplicableDiscount, GetDiscountCode(PromotionBag.PromoCode, couponCode), discountType, group.OrdersDiscount);
                        discountApplied = true;
                    }
                    ShoppingCart.IsAnyPromotionApplied = discountApplied;
                }
            }

            return discountApplied;
        }

        //Set Product Discount
        private void SetProductDiscount(ZnodeProductBaseEntity product, decimal maxApplicableLineItemDiscount, string discountCode, OrderDiscountTypeEnum discountType, List<OrderDiscountModel> ordersDiscount)
        {
            product.DiscountAmount += maxApplicableLineItemDiscount;
            product.OrdersDiscount = SetOrderDiscountDetails(discountCode, maxApplicableLineItemDiscount, discountType, ordersDiscount);
        }
    }
    #endregion
}


