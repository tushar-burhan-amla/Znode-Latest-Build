using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionPercentOffCatalog : ZnodeCartPromotionType
    {
        #region Private Variables
        protected readonly IZnodePromotionHelper promotionHelper = null;
        #endregion

        #region Constructor
        public ZnodeCartPromotionPercentOffCatalog()
        {
            Name = "Percent Off Catalog";
            Description = "Applies a percent off products for a particular catalog; affects the shopping cart.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountPercent);
            Controls.Add(ZnodePromotionControl.RequiredCatalog);
            Controls.Add(ZnodePromotionControl.RequiredCatalogMinimumQuantity);
            Controls.Add(ZnodePromotionControl.MinimumOrderAmount);
            Controls.Add(ZnodePromotionControl.Coupon);

            promotionHelper = GetService<IZnodePromotionHelper>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the percent off products for a particular catalog in the shopping cart.
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

            // Loop through each cart Item
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                bool isPromotionApplied = false;
                int productId = cartItem.Product.ProductID;
                if (cartItem.Product.ZNodeConfigurableProductCollection.Count> 0)
                {
                    productId = cartItem.ParentProductId;
                }
                // Get the catalogs by product
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
        private void ApplyDiscount(out bool isPromotionApplied, bool isCouponValid, int? couponIndex, ZnodeShoppingCartItem cartItem)
        {
            decimal subTotal = GetCartSubTotal(ShoppingCart);
            decimal qtyOrdered = GetCartQuantity();
            bool discountApplied = false;


            if (Equals(PromotionBag.Coupons, null))
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, qtyOrdered, ShoppingCart.SubTotal, false))
                {
                    isPromotionApplied = false;
                    return;

                }
                discountApplied = ApplyProductDiscount(couponIndex, subTotal, qtyOrdered, cartItem);
            }
            else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, qtyOrdered, ShoppingCart.SubTotal, true))
                {
                    isPromotionApplied = false;
                    return;
                }

                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        discountApplied = ApplyProductDiscount(couponIndex, subTotal, qtyOrdered, cartItem, coupon.Code);
                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                        {
                            break;
                        }
                    }
                }
            }
            isPromotionApplied = discountApplied;
        }

        //to apply product discount
        private bool ApplyProductDiscount(int? couponIndex, decimal subTotal, decimal qtyOrdered, ZnodeShoppingCartItem cartItem, string couponCode = "")
        {
            bool discountApplied = false;

            if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
            {
                ApplyGroupProductDiscount(cartItem, qtyOrdered, subTotal, out discountApplied);
            }
            else if (PromotionBag.RequiredCatalogMinimumQuantity <= qtyOrdered && PromotionBag.MinimumOrderAmount <= subTotal)
            {
                decimal discountedPrice = ShoppingCart.GetDiscountedPrice(cartItem.Product, cartItem.Quantity);
                decimal discount = discountedPrice * (PromotionBag.Discount / 100);
                decimal maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(discount, cartItem.Quantity, cartItem.Product);
                cartItem.Product.DiscountAmount += maxApplicableDiscount;
                cartItem.Product.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, GetDiscountType(couponCode), cartItem.Product.OrdersDiscount);
                discountApplied = true;
                SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
            }

            if (!string.IsNullOrEmpty(couponCode) && discountApplied)
            {
                SetCouponApplied(couponCode);
                ShoppingCart.Coupons[couponIndex.Value].CouponApplied = IsCouponApplicableForLineItem(couponCode);
            }
            else
            {
                ShoppingCart.IsAnyPromotionApplied = true;
            }

            return discountApplied;
        }

        // This is code is not needed as in ZnodeCartPromotionAmountOffCatalog we are not calculating it for Configurable Product so removing the reference of this function. If found any scenario will check this again else remove this function.
        private void ApplyConfigurableProductDiscount(ZnodeShoppingCartItem cartItem, decimal qtyOrdered, decimal subTotal, out bool discountApplied, string couponCode = "")
        {
            bool isDiscountApplied = false;
            decimal basePrice = cartItem.PromotionalPrice;
            foreach (ZnodeProductBaseEntity config in cartItem.Product.ZNodeConfigurableProductCollection)
            {
                if (PromotionBag.RequiredCatalogMinimumQuantity <= qtyOrdered && PromotionBag.MinimumOrderAmount <= subTotal)
                {
                    decimal discountedPrice = ShoppingCart.GetDiscountedPrice(cartItem.Product, cartItem.Quantity);
                    decimal discount = discountedPrice * (PromotionBag.Discount / 100);
                    decimal maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(discount, cartItem.Quantity, cartItem.Product);
                    cartItem.Product.DiscountAmount += maxApplicableDiscount;
                    config.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, GetDiscountType(couponCode), config.OrdersDiscount);
                    isDiscountApplied = true;
                    SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
                }
                else
                {
                    if (config.DiscountAmount > basePrice * (PromotionBag.Discount / 100))
                    {
                        config.DiscountAmount -= basePrice * (PromotionBag.Discount / 100);
                    }
                    config.DiscountAmount = cartItem.Product.DiscountAmount < 0 ? 0 : cartItem.Product.DiscountAmount;
                    config.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), cartItem.Product.DiscountAmount, GetDiscountType(couponCode), config.OrdersDiscount);
                    isDiscountApplied = true;
                }
            }
            discountApplied = isDiscountApplied;
        }

        // to apply group product Discount
        private void ApplyGroupProductDiscount(ZnodeShoppingCartItem cartItem, decimal qtyOrdered, decimal subTotal, out bool discountApplied, string couponCode = "")
        {
            bool isDiscountApplied = false;
            foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
            {
                decimal basePrice = group.FinalPrice;
                if (PromotionBag.RequiredCatalogMinimumQuantity <= qtyOrdered && PromotionBag.MinimumOrderAmount <= subTotal)
                {
                    decimal discountedPrice = ShoppingCart.GetDiscountedPrice(group, cartItem.Quantity);
                    decimal discount = discountedPrice * (PromotionBag.Discount / 100);
                    decimal maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(discount, cartItem.Quantity, group);
                    group.DiscountAmount += maxApplicableDiscount;
                    group.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, GetDiscountType(couponCode), group.OrdersDiscount);
                    isDiscountApplied = true;
                }
                else
                {
                    if (group.DiscountAmount > basePrice * (PromotionBag.Discount / 100))
                    {
                        group.DiscountAmount -= basePrice * (PromotionBag.Discount / 100);
                    }
                    group.DiscountAmount = cartItem.Product.DiscountAmount < 0 ? 0 : cartItem.Product.DiscountAmount;
                    group.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), cartItem.Product.DiscountAmount, GetDiscountType(couponCode), group.OrdersDiscount);
                    isDiscountApplied = true;
                }
            }
            discountApplied = isDiscountApplied;
        }

        #endregion
    }
}
