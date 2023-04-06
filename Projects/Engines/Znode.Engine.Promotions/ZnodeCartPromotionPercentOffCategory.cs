using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionPercentOffCategory : ZnodeCartPromotionType
    {
        #region Private Variables
        protected readonly IZnodePromotionHelper promotionHelper = null;
        private List<PromotionCartItemQuantity> promocategorySkus;
        decimal quantity;
        #endregion

        #region Constructor
        public ZnodeCartPromotionPercentOffCategory()
        {
            Name = "Percent Off Category";
            Description = "Applies a percent off products in a particular category; affects the shopping cart.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountPercent);
            Controls.Add(ZnodePromotionControl.RequiredCategory);
            Controls.Add(ZnodePromotionControl.RequiredCategoryMinimumQuantity);
            Controls.Add(ZnodePromotionControl.MinimumOrderAmount);
            Controls.Add(ZnodePromotionControl.Coupon);
            promotionHelper = GetService<IZnodePromotionHelper>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the percent off products in a particular category in the shopping cart.
        /// </summary>
        public override void Calculate(int? couponIndex, List<PromotionModel> allPromotions)
        {
            ApplicablePromolist = ZnodePromotionManager.GetPromotionsByType(ZnodeConstant.PromotionClassTypeCart, ClassName, allPromotions, OrderBy);
            bool isCouponValid = false;
            if (!Equals(couponIndex, null))
            {
                isCouponValid = ValidateCoupon(couponIndex);
            }

            //to get all category of promotion by PromotionId
            List<CategoryModel> promotionCategories = promotionHelper.GetPromotionCategory(PromotionBag.PromotionId);

            //to set all promotions brand wise sku to calculate each sku quantity
            promocategorySkus = promotionHelper.SetPromotionCategorySKUQuantity(promotionCategories, ShoppingCart,out quantity);

            // Loop through each cart Item
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                bool isPromotionApplied = false;
                int productId = 0;
                // Get the category by product
                if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0 || cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                    productId = cartItem.ParentProductId;
                else
                    productId = cartItem.Product.ProductID;

                List<CategoryModel> productCategories = promotionHelper.GetCategoryByProduct(productId);

                foreach (CategoryModel promotion in promotionCategories)
                {
                    foreach (CategoryModel product in productCategories)
                    {
                        if (promotion.PimCategoryId == product.PimCategoryId)
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
            decimal qtyOrdered = GetQuantityOrdered(cartItem.Product.ProductID);
            bool discountApplied = false;

            // Introduce new variable to identify discount is of promotion type or coupon type
            bool isCoupon = true;

            List<PromotionModel> promolistModel = new List<PromotionModel>();

            // This is used to check wheather this is coupon or not, below condition satisfied incase of coupon
            if (ApplicablePromolist.Any(x => x.PromoCode == PromotionBag.PromoCode && x.QuantityMinimum <= quantity && x.IsCouponRequired == true))
            {
                promolistModel = new List<PromotionModel> { ApplicablePromolist.FirstOrDefault(x => x.PromoCode == PromotionBag.PromoCode && x.QuantityMinimum <= quantity && x.IsCouponRequired == true) };
            }

            // In case of coupon PromolistModel is not null
            // isCoupon set true or false in case of coupon or promotion
            if (promolistModel.Count() == 0)
            {
                isCoupon = false;
               
                if (ApplicablePromolist.Any(x => x.OrderMinimum <= subTotal && x.QuantityMinimum <= quantity))
                {
                    int? promotionId = ApplicablePromolist.Where(x => x.OrderMinimum <= subTotal && x.QuantityMinimum <= quantity).OrderByDescending(y => y.OrderMinimum)?.FirstOrDefault().PromotionId;
                    promolistModel = new List<PromotionModel> { ApplicablePromolist.FirstOrDefault(x => x.PromotionId == promotionId) };
                }
            }

            // isCoupon is supplied 
            if (!promotionHelper.IsApplicablePromotion(promolistModel, PromotionBag.PromoCode, subTotal, quantity, PromoApplicabilityCriteria.Both, isCoupon))
            {
                isPromotionApplied = false;
                return;
            }           

            if (Equals(PromotionBag.Coupons, null))
            {
                discountApplied = ApplyProductDiscount(couponIndex, subTotal, cartItem);
            }
            else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
            {
                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        discountApplied = ApplyProductDiscount(couponIndex, subTotal, cartItem, coupon.Code);
                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                        {
                            break;
                        }
                    }
                }
            }

            isPromotionApplied = discountApplied;
        }
        private bool ApplyProductDiscount(int? couponIndex, decimal subTotal, ZnodeShoppingCartItem cartItem, string couponCode = "")
        {
            bool discountApplied = false;
            decimal maxApplicableDiscount = 0;
            if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
            {
                foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                {
                    if (IsRequiredMinimumQuantity(cartItem.Product.SKU) && PromotionBag.MinimumOrderAmount <= subTotal)
                    {
                        decimal discount = ShoppingCart.GetDiscountedPrice(group, cartItem.Quantity) * (PromotionBag.Discount / 100);
                        maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(discount, cartItem.Quantity, group);
                        group.DiscountAmount += maxApplicableDiscount;
                        group.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, GetDiscountType(couponCode), group.OrdersDiscount);
                        cartItem.Quantity = GetGroupProductCartQuantity(cartItem.Product.ProductID, group.ProductID);
                        SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
                    }
                    discountApplied = true;
                }
            }
            else
            {
                if (IsRequiredMinimumQuantity(cartItem.Product.SKU) && PromotionBag.MinimumOrderAmount <= subTotal)
                {
                    // Product discount percent
                    decimal discount = ShoppingCart.GetDiscountedPrice(cartItem.Product, cartItem.Quantity) * (PromotionBag.Discount / 100);
                    maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(discount, cartItem.Quantity, cartItem.Product);
                    cartItem.Product.DiscountAmount += maxApplicableDiscount;
                    cartItem.Product.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, GetDiscountType(couponCode), cartItem.Product.OrdersDiscount);
                    discountApplied = true;
                    SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
                }
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
       
        //to check minimum quantity of promotion in the shopping cart item
        private bool IsRequiredMinimumQuantity(string sku)
        {
            bool result = false;
            if (promocategorySkus?.Count > 0)
            {
                decimal cartQty = promocategorySkus.Sum(x => x.Quantity);
                if (cartQty >= PromotionBag.RequiredCategoryMinimumQuantity)
                {
                    result = promocategorySkus.Any(x => x.SKU.Contains(sku));
                }
            }
            return result;
        }        
        #endregion
    }
}
