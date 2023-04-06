using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionAmountOffCategory : ZnodeCartPromotionType
    {
        #region Variables
        protected readonly IZnodePromotionHelper promotionHelper = null;
        private List<PromotionCartItemQuantity> promocategorySkus;
        #endregion

        #region Constructor
        public ZnodeCartPromotionAmountOffCategory()
        {
            Name = "Amount Off Category";
            Description = "Applies an amount off products in a particular category; affects the shopping cart.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountAmount);
            Controls.Add(ZnodePromotionControl.RequiredCategory);
            Controls.Add(ZnodePromotionControl.RequiredCategoryMinimumQuantity);
            Controls.Add(ZnodePromotionControl.MinimumOrderAmount);
            Controls.Add(ZnodePromotionControl.Coupon);
            promotionHelper = GetService<IZnodePromotionHelper>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the amount off products in a particular category in the shopping cart.
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
            decimal quantity;
            //to set all promotions brand wise sku to calculate each sku quantity
            promocategorySkus = promotionHelper.SetPromotionCategorySKUQuantity(promotionCategories, ShoppingCart, out quantity);

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
                            break;
                        }
                    }
                    if (isPromotionApplied)
                        break;
                }
            }
            AddPromotionMessage(couponIndex);
        }
        #endregion

        //to apply discount for each cart item
        private void ApplyDiscount(out bool isPromotionApplied, bool isCouponValid, int? couponIndex, ZnodeShoppingCartItem cartItem)
        {
            decimal subTotal = GetCartSubTotal(ShoppingCart);
            bool discountApplied = false;
            if (Equals(PromotionBag.Coupons, null))
            {
                
                List<PromotionModel> PromolistModel = ApplicablePromolist.Where(x => x.OrderMinimum <= subTotal).ToList();
                decimal maxValue = PromolistModel.Max(y => y.OrderMinimum).GetValueOrDefault();

                if (HelperUtility.IsNotNull(PromolistModel.FirstOrDefault(x => x.OrderMinimum == maxValue)))
                    PromolistModel = new List<PromotionModel> { PromolistModel.FirstOrDefault(x => x.OrderMinimum == maxValue) };

                if (!promotionHelper.IsApplicablePromotion(PromolistModel, PromotionBag.PromoCode, ShoppingCart.SubTotal))
                {
                    isPromotionApplied = false;
                    return;
                }

                discountApplied = ApplyProductDiscount(subTotal, couponIndex, cartItem);
            }
            else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, ShoppingCart.SubTotal, false, true))
                {
                    isPromotionApplied = false;
                    return;
                }

                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        ApplyProductDiscount(subTotal, couponIndex, cartItem, coupon.Code);

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
        private bool ApplyProductDiscount(decimal subTotal, int? couponIndex, ZnodeShoppingCartItem cartItem, string couponCode = "")
        {
            bool discountApplied = false;

            if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
            {
                foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                {
                    if (IsRequiredMinimumQuantity(cartItem.Product.SKU) && PromotionBag.MinimumOrderAmount <= subTotal)
                    {
                        var quantity = GetGroupProductCartQuantity(cartItem.Product.ProductID, group.ProductID);
                        cartItem.Quantity = quantity;
                        SetDiscountAmount(cartItem, couponCode);
                    }
                    discountApplied = true;
                }
            }
            else if (IsRequiredMinimumQuantity(cartItem.Product.SKU) && PromotionBag.MinimumOrderAmount <= subTotal)
            {
                if (IsDiscountApplicable(cartItem.Product.DiscountAmount, cartItem.Product.FinalPrice))
                {
                    SetDiscountAmount(cartItem, couponCode);
                }
                discountApplied = true;
            }

            if (discountApplied && !string.IsNullOrEmpty(couponCode))
            {
                SetCouponApplied(couponCode);
                ShoppingCart.Coupons[couponIndex.Value].CouponApplied = IsCouponApplicableForLineItem(couponCode); 
            }
            else
            {
                ShoppingCart.IsAnyPromotionApplied = discountApplied;
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

        //Set Promotion/Coupon discount amount.
        protected virtual void SetDiscountAmount(ZnodeShoppingCartItem cartItem, string couponCode)
        {
            //If the discount amount is greater than product price then apply discount equals to product price.
            decimal discountPrice = (PromotionBag.Discount > cartItem.PromotionalPrice) ? cartItem.PromotionalPrice : PromotionBag.Discount;

            cartItem.Product.DiscountAmount += discountPrice;
            cartItem.Product.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), discountPrice, GetDiscountType(couponCode), cartItem.Product.OrdersDiscount);
            SetPromotionalPriceAndDiscount(cartItem, discountPrice);
        }
    }
}
