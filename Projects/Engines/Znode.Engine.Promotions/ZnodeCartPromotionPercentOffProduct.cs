using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Promotions
{
    public class ZnodeCartPromotionPercentOffProduct : ZnodeCartPromotionType
    {
        #region Private Variable
        protected readonly IZnodePromotionHelper promotionHelper = null;
        #endregion

        #region Constructor
        public ZnodeCartPromotionPercentOffProduct()
        {
            Name = "Percent Off Product";
            Description = "Applies a percent off a product for an order; affects the shopping cart.";
            AvailableForFranchise = true;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountPercent);
            Controls.Add(ZnodePromotionControl.RequiredProduct);
            Controls.Add(ZnodePromotionControl.RequiredProductMinimumQuantity);
            Controls.Add(ZnodePromotionControl.MinimumOrderAmount);
            Controls.Add(ZnodePromotionControl.Coupon);
            promotionHelper = GetService<IZnodePromotionHelper>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the percent off a product in the shopping cart.
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
            List<ProductModel> promotionsProduct = promotionHelper.GetPromotionProducts(PromotionBag.PromotionId);

            // Loop through each cart Item
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                if (CheckProductPromotion(promotionsProduct, cartItem.Product.ProductID)
                    && cartItem.Product.ZNodeConfigurableProductCollection.Count == 0
                    && cartItem.Product.ZNodeGroupProductCollection.Count == 0)
                {
                    ApplyDiscount(isCouponValid, couponIndex, cartItem);
                }

                //to apply promotion for addon product
                if (cartItem.Product.ZNodeAddonsProductCollection.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity addon in cartItem.Product.ZNodeAddonsProductCollection)
                    {
                        ApplyLineItemDiscount(cartItem.Quantity, isCouponValid, couponIndex, addon, promotionsProduct);
                    }
                }

                //to apply promotion for configurable product
                if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0)
                {
                    if (CheckProductPromotion(promotionsProduct, cartItem.ParentProductId))
                        ApplyLineItemDiscount(cartItem.Quantity, isCouponValid, couponIndex, cartItem.Product, promotionsProduct, cartItem.Product.ZNodeConfigurableProductCollection[0], true);
                    else
                    {
                        foreach (ZnodeProductBaseEntity configurable in cartItem.Product.ZNodeConfigurableProductCollection)
                        {
                            ApplyLineItemDiscount(cartItem.Quantity, isCouponValid, couponIndex, configurable, promotionsProduct, null, false, cartItem);
                        }
                    }
                }

                //to apply promotion for group product
                if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                {
                    if (CheckProductPromotion(promotionsProduct, cartItem.Product.ProductID))
                        ApplyLineItemDiscount(cartItem.Quantity, isCouponValid, couponIndex, cartItem.Product, promotionsProduct, cartItem.Product.ZNodeGroupProductCollection[0]);
                    else
                    {
                        foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                        {
                            ApplyLineItemDiscount(group.SelectedQuantity, isCouponValid, couponIndex, group, promotionsProduct);
                        }
                    }
                }
            }
            AddPromotionMessage(couponIndex);
        }
        #endregion

        #region Private Method
        //to apply discount amount
        private void ApplyDiscount(bool isCouponValid, int? couponIndex, ZnodeShoppingCartItem cartItem)
        {
            decimal subTotal = GetCartSubTotal(ShoppingCart);
            decimal qtyOrdered = GetQuantityOrdered(cartItem.Product.ProductID);
            decimal maxApplicableDiscount = 0;
            if (Equals(PromotionBag.Coupons, null))
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, cartItem.Quantity, ShoppingCart.SubTotal, false))
                    return;

                if (PromotionBag.RequiredProductMinimumQuantity <= qtyOrdered && PromotionBag.MinimumOrderAmount <= subTotal)
                {
                    maxApplicableDiscount = SetPromotionCouponDiscount(couponIndex, cartItem.Quantity, cartItem.Product);
                    SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
                }
            }
            else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
            {
                if (!promotionHelper.IsApplicablePromotion(ApplicablePromolist, PromotionBag.PromoCode, cartItem.Quantity, ShoppingCart.SubTotal, true))
                    return;

                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if ((coupon.AvailableQuantity > 0 || IsExistingOrderCoupon(coupon.Code)) && CheckCouponCodeValid(ShoppingCart.Coupons[couponIndex.Value].Coupon, coupon.Code))
                    {
                        if (PromotionBag.RequiredProductMinimumQuantity <= qtyOrdered && PromotionBag.MinimumOrderAmount <= subTotal)
                        {
                            maxApplicableDiscount = SetPromotionCouponDiscount(couponIndex, cartItem.Quantity, cartItem.Product, coupon.Code);
                            SetPromotionalPriceAndDiscount(cartItem, maxApplicableDiscount);
                        }

                        if (IsUniqueCouponApplied(PromotionBag, ShoppingCart.Coupons[couponIndex.Value].CouponApplied))
                            break;
                    }
                }
            }
        }

        //to apply line item discount
        private void ApplyLineItemDiscount(decimal qtyOrdered, bool isCouponValid, int? couponIndex, ZnodeProductBaseEntity product, List<ProductModel> promotionsProduct, ZnodeProductBaseEntity childProduct = null, bool isConfig = false, ZnodeShoppingCartItem cartItem = null)
        {
            if (CheckProductPromotion(promotionsProduct, product.ProductID)
                || childProduct != null)
            {
                if (Equals(PromotionBag.Coupons, null))
                {
                    ApplyChildItemDiscount(qtyOrdered, couponIndex, product, childProduct, string.Empty, isConfig, cartItem);
                }
                else if (!Equals(PromotionBag.Coupons, null) && isCouponValid)
                {
                    foreach (CouponModel coupon in PromotionBag.Coupons)
                    {
                        ApplyChildItemDiscount(qtyOrdered, couponIndex, product, childProduct, coupon.Code, isConfig, cartItem);
                    }
                }
            }
        }

        //to apply child item discount
        private void ApplyChildItemDiscount(decimal quantity, int? couponIndex, ZnodeProductBaseEntity product, ZnodeProductBaseEntity childProduct = null, string couponCode = "", bool isConfigurableProduct = false, ZnodeShoppingCartItem cartItem = null)
        {
            decimal subTotal = GetCartSubTotal(ShoppingCart);
            if (PromotionBag.RequiredProductMinimumQuantity <= quantity && PromotionBag.MinimumOrderAmount <= subTotal)
            {
                SetPromotionCouponDiscount(couponIndex, quantity, product, couponCode, childProduct, isConfigurableProduct, cartItem);
            }
        }

        //to check product have associated promotion to it
        private bool CheckProductPromotion(List<ProductModel> promotionsProduct, int productId)
        => promotionsProduct.Where(x => x.ProductId == productId)?.FirstOrDefault() == null ? false : true;


        // In case of addon, Config and Group child entity passed as Product entity
        private decimal SetPromotionCouponDiscount(int? couponIndex, decimal quantity, ZnodeProductBaseEntity product, string couponCode = "", ZnodeProductBaseEntity childProduct = null, bool isConfigurableProduct = false, ZnodeShoppingCartItem cartItem = null)
        {
            decimal maxApplicableDiscount = 0;
            decimal discount = 0;
            if (HelperUtility.IsNotNull(cartItem))
            {
                // Here for Configurable Product We are passing Parent and childProduct(Collection entity) as child to get Maximum applicable discount
                // child entity send as Product entity form Calculate method ZNodeConfigurableProductCollection
                discount = ShoppingCart.GetDiscountedPrice(cartItem.Product, quantity, product) * (PromotionBag.Discount / 100);
                maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(discount, quantity, cartItem.Product, product);
            }
            else
            {
                discount = ShoppingCart.GetDiscountedPrice(product, quantity, childProduct) * (PromotionBag.Discount / 100);
                maxApplicableDiscount = ShoppingCart.GetMaximumApplicableDiscountOnLineItem(discount, quantity, product, childProduct);
            }

            OrderDiscountTypeEnum discountType = GetDiscountType(couponCode);
            if (HelperUtility.IsNull(childProduct) && HelperUtility.IsNull(cartItem))
            {
                product.DiscountAmount += maxApplicableDiscount;
                product.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, discountType, product.OrdersDiscount);
            }
            //For configurable Product setting discount on Parent Product to calculate correct maximum applicable discount for other Promotion type (Category, Catalog etc.)
            else if (HelperUtility.IsNotNull(cartItem))
            {
                cartItem.Product.DiscountAmount += maxApplicableDiscount;
                cartItem.Product.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, discountType, cartItem.Product.OrdersDiscount);
            }
            else
            {
                if (isConfigurableProduct)
                {
                    product.DiscountAmount += maxApplicableDiscount;
                    product.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, discountType, product.OrdersDiscount);
                }
                else
                {
                    childProduct.DiscountAmount += maxApplicableDiscount;
                    childProduct.OrdersDiscount = SetOrderDiscountDetails(GetDiscountCode(PromotionBag.PromoCode, couponCode), maxApplicableDiscount, discountType, childProduct.OrdersDiscount);
                }

            }
            if (discountType == OrderDiscountTypeEnum.COUPONCODE)
            {
                SetCouponApplied(couponCode);
                ShoppingCart.Coupons[couponIndex.Value].CouponApplied = IsCouponApplicableForLineItem(couponCode);
            }
            else
                ShoppingCart.IsAnyPromotionApplied = true;

            return maxApplicableDiscount;
        }

        #endregion
    }
}
