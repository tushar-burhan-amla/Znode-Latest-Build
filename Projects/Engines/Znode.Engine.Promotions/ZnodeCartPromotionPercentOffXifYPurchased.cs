using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionPercentOffXifYPurchased : ZnodeCartPromotionType
    {
        #region Variable
        protected readonly IZnodePromotionHelper promotionHelper = null;
        #endregion

        #region Constructor
        public ZnodeCartPromotionPercentOffXifYPurchased()
        {
            Name = "Percent Off X If Y Purchased";
            Description = "Applies a percent off product X if product Y is purchased; affects the shopping cart.";
            AvailableForFranchise = true;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountPercent);
            Controls.Add(ZnodePromotionControl.RequiredProduct);
            Controls.Add(ZnodePromotionControl.RequiredProductMinimumQuantity);
            Controls.Add(ZnodePromotionControl.DiscountedProduct);
            Controls.Add(ZnodePromotionControl.DiscountedProductQuantity);
            Controls.Add(ZnodePromotionControl.Coupon);
            promotionHelper = GetService<IZnodePromotionHelper>();
        }
        #endregion

        #region Public Methods
        public override void Calculate(int? couponIndex, List<PromotionModel> allPromotions)
        {
            ApplicablePromolist = ZnodePromotionManager.GetPromotionsByType(ZnodeConstant.PromotionClassTypeCart, ClassName, allPromotions, OrderBy);

            if (HelperUtility.IsNull(ShoppingCart.DiscountedProductQuantity) || ShoppingCart.DiscountedProductQuantity?.Count == 0)
                ShoppingCart.DiscountedProductQuantity = new Dictionary<string, decimal>();

            bool isCouponValid = false;
            if (!Equals(couponIndex, null))
            {
                isCouponValid = ValidateCoupon(couponIndex);
            }

            SetDiscountedProduct();

            // Loop through each cart Item
            if (ShoppingCart.ShoppingCartItems.Exists(s => s.Product.ProductID == PromotionBag.RequiredProductId) &&
                ShoppingCart.ShoppingCartItems.Exists(d => d.Product.ProductID == PromotionBag.DiscountedProductId))
            {
                var cartItem = ShoppingCart.ShoppingCartItems.Find(p => p.Product.ProductID == PromotionBag.DiscountedProductId);
                ApplyDiscount(isCouponValid, couponIndex, cartItem);
            }
            else
            {
                bool isRequiredItemPresent = false;
                //Looping to check if required product for promotion is available in the cart.
                foreach (ZnodeShoppingCartItem cartItems in ShoppingCart.ShoppingCartItems)
                {
                    if (cartItems.Product.ZNodeGroupProductCollection.Count > 0)
                    {
                        foreach (ZnodeProductBaseEntity groupProduct in cartItems.Product.ZNodeGroupProductCollection)
                        {
                            if (groupProduct.ProductID == PromotionBag.RequiredProductId && groupProduct.SelectedQuantity >= PromotionBag.RequiredProductMinimumQuantity)
                            {
                                isRequiredItemPresent = true;
                                //Sending out of loop, cannot use break because there are two loops are running so goto is the best available choice.
                                goto OutOfRequiredProductCheckLoop;
                            }
                        }
                    }
                }

                //Label fpr goto statement in previous loop
                OutOfRequiredProductCheckLoop:
                foreach (ZnodeShoppingCartItem cartItems in ShoppingCart.ShoppingCartItems)
                {
                    if (cartItems.Product.ZNodeGroupProductCollection.Count > 0)
                    {
                        foreach (ZnodeProductBaseEntity groupProduct in cartItems.Product.ZNodeGroupProductCollection)
                        {
                            if (groupProduct.ProductID == PromotionBag.DiscountedProductId && isRequiredItemPresent)
                            {
                                ShoppingCart.IsAnyPromotionApplied = ApplyLineItemDiscount(isCouponValid, couponIndex, cartItems, groupProduct.SelectedQuantity);
                                goto OutOfDiscountProductCheckLoop;
                            }
                        }
                    }

                }
            }

            //Label for goto statement in previous discount check loop
            OutOfDiscountProductCheckLoop:

            // Finally add promo message
            AddPromotionMessage(couponIndex);
        }

        #endregion

        #region Private
        //to apply discount to cart item
        private void ApplyDiscount(bool isCouponValid, int? couponIndex, ZnodeShoppingCartItem cartItem)
        {
            // Tiered pricing calculation
            decimal unitPrice = cartItem.TieredPricing;
            decimal requiredProductQtyOrdered = GetQuantityOrdered(PromotionBag.RequiredProductId);
            decimal requiredProductMinQty = PromotionBag.RequiredProductMinimumQuantity;
            decimal discountedProductQty = cartItem.Quantity;
            decimal extendedPrice = unitPrice * PromotionBag.DiscountedProductQuantity;

            //if product is of group type then set required product quantity to 1           
            requiredProductQtyOrdered = requiredProductQtyOrdered == 0 ? GetGroupProductQuantity(PromotionBag.RequiredProductId) : requiredProductQtyOrdered;

            if (Equals(PromotionBag.Coupons, null))
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, ShoppingCart.SubTotal, true, false))
                    return;

                if (PromotionBag.DiscountedProductId == PromotionBag.RequiredProductId)
                {
                    if (discountedProductQty == requiredProductQtyOrdered)
                    {
                        discountedProductQty = cartItem.Quantity - requiredProductMinQty;
                    }

                    requiredProductMinQty += PromotionBag.DiscountedProductQuantity;
                }

                if (requiredProductQtyOrdered >= requiredProductMinQty && PromotionBag.DiscountedProductQuantity <= discountedProductQty)
                {
                    CalculateAndApplyDiscount(couponIndex, cartItem);
                }
            }
            else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, ShoppingCart.SubTotal, true, true))
                    return;

                requiredProductMinQty = PromotionBag.RequiredProductMinimumQuantity;
                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        if (PromotionBag.DiscountedProductId == PromotionBag.RequiredProductId)
                        {
                            if (discountedProductQty == requiredProductQtyOrdered)
                            {
                                discountedProductQty = cartItem.Quantity - requiredProductMinQty;
                            }
                            requiredProductMinQty += PromotionBag.DiscountedProductQuantity;
                        }
                        if (requiredProductQtyOrdered >= requiredProductMinQty && PromotionBag.DiscountedProductQuantity <= discountedProductQty)
                        {
                             CalculateAndApplyDiscount(couponIndex, cartItem, coupon.Code);
                        }

                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void CalculateAndApplyDiscount(int? couponIndex, ZnodeShoppingCartItem cartItem, string couponCode= "")
        {
            decimal extendedPrice;
            string discountCode = GetDiscountCode(PromotionBag.PromoCode, couponCode);
            if (!ShoppingCart.DiscountedProductQuantity.ContainsKey(discountCode))
                ShoppingCart.DiscountedProductQuantity.Add(discountCode, PromotionBag.DiscountedProductQuantity);
            decimal discountedPrice = ShoppingCart.GetDiscountedPrice( cartItem.Product, cartItem.Quantity, discountCode: discountCode);
            decimal discount = discountedPrice * (PromotionBag.Discount / 100);
            decimal maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(discount, cartItem.Quantity, cartItem.Product, discountCode: discountCode);
            extendedPrice = maxApplicableDiscount * PromotionBag.DiscountedProductQuantity;
            maxApplicableDiscount = extendedPrice > 0 ? extendedPrice / cartItem.Quantity : 0;
            cartItem.ExtendedPriceDiscount += extendedPrice;
            SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
            cartItem.Product.OrdersDiscount = SetOrderDiscountDetails(discountCode, maxApplicableDiscount, GetDiscountType(couponCode), cartItem.Product.OrdersDiscount);

            if (string.IsNullOrEmpty(couponCode))
                ShoppingCart.IsAnyPromotionApplied = true;
            else
            {
                SetCouponApplied(couponCode);
                ShoppingCart.Coupons[couponIndex.Value].CouponApplied = IsCouponApplicableForLineItem(couponCode);
            }
        }

        private bool ApplyLineItemDiscount(bool isCouponValid, int? couponIndex, ZnodeShoppingCartItem cartItems, decimal cartQuantity)
        {
            bool isPromotionApplied = false;
            //Checking if coupon is required.
            if (Equals(PromotionBag.Coupons, null))
            {
                isPromotionApplied = ApplyDiscountToChildProduct(isCouponValid, couponIndex, string.Empty, cartItems, cartQuantity);
            }
            else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, ShoppingCart.SubTotal, false, true))
                    return false;

                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    //Checking coupon related validations.
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        isPromotionApplied = ApplyDiscountToChildProduct(isCouponValid, couponIndex, coupon.Code, cartItems, cartQuantity);
                    }

                    if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                    {
                        break;
                    }
                }
            }

            return isPromotionApplied;
        }

        private bool ApplyDiscountToChildProduct(bool isCouponValid, int? couponIndex, string couponCode, ZnodeShoppingCartItem cartItem, decimal cartQuantity)
        {
            bool isDiscountApplied = false;
            CalculateAndApplyDiscount(couponIndex, cartItem , couponCode);
            isDiscountApplied = true;
            return isDiscountApplied;
        }

        //to set discounted product id
        private void SetDiscountedProduct()
        {
            List<ProductModel> promotionsProduct = promotionHelper.GetPromotionProducts(PromotionBag.PromotionId);
            if (promotionsProduct?.Count > 0)
            {
                ProductModel promotion = promotionsProduct.FirstOrDefault();
                PromotionBag.DiscountedProductId = promotion.ProductId;
            }
        }
        #endregion
    }
}
