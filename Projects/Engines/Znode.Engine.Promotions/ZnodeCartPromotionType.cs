using System;
using System.Collections.Generic;
using System.Diagnostics;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Promotions
{
    /// <summary>
    /// Provides the base implementation for all shopping cart promotion types.
    /// </summary>
    public abstract class ZnodeCartPromotionType : ZnodePromotionsType, IZnodeCartPromotionType
    {
        #region Private Variables
        private string _OrderBy = nameof(PromotionModel.QuantityMinimum);
        #endregion

        #region Public Properties
        public ZnodeShoppingCart ShoppingCart { get; set; }
        public ZnodePromotionBag PromotionBag { get; set; }
        public bool IsPromoCouponAvailable { get; set; }
        public List<PromotionModel> ApplicablePromolist = new List<PromotionModel>();
        public virtual string OrderBy
        {
            get { return _OrderBy; }
            set { _OrderBy = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Binds the shopping cart and promotion data to the promotion.
        /// </summary>
        /// <param name="shoppingCart">The current shopping cart.</param>
        /// <param name="promotionBag">The promotion properties.</param>
        /// <param name="couponIndex">Index of the coupon.</param>
        public virtual void Bind(ZnodeShoppingCart shoppingCart, ZnodePromotionBag promotionBag)
        {
            ShoppingCart = shoppingCart;
            PromotionBag = promotionBag;
        }

        /// <summary>
        /// Calculates the promotion and updates the shopping cart.
        /// </summary>
        public virtual void Calculate(int? couponIndex, List<PromotionModel> allPromotions)
        {
        }

        /// <summary>
        /// Process anything that must be done before the order is submitted.
        /// </summary>
        /// <returns>True if everything is good for submitting the order; otherwise, false.</returns>
        public virtual bool PreSubmitOrderProcess() => true;

        /// <summary>
        /// Process anything that must be done after the order is submitted.
        /// </summary>
        public virtual void PostSubmitOrderProcess()
        {
            // Most promotions don't need any further processing after the order is submitted
        }

        /// <summary>
        /// Validates the coupon code on the shopping cart and on the promotion property are the same.
        /// </summary>
        /// <returns>True if the coupon is valid; otherwise, false.</returns>
        public virtual bool ValidateCoupon(int? couponIndex)
        {
            bool isValid = false;

            if ((PromotionBag?.Coupons?.Count > 0 && ShoppingCart?.Coupons?.Count > 0))
            {
                isValid = IsValidCouponCode(ShoppingCart.Coupons[couponIndex.Value]?.Coupon);
                if (isValid)
                {
                    SetValidCoupon(ShoppingCart.Coupons[couponIndex.Value].Coupon);
                    ShoppingCart.Coupons[couponIndex.Value].CouponValid = true;
                }
            }
            return isValid;
        }

        /// <summary>
        /// Add promotion message to the shopping cart.
        /// </summary>
        public virtual void AddPromotionMessage(int? couponIndex)
        {
            if (ValidateCoupon(couponIndex))
            {
                foreach (CouponModel coupon in PromotionBag.Coupons)
                {
                    if (coupon.CouponValid && coupon.CouponApplied)
                    {
                        AddMessageToCart(couponIndex, coupon, ShoppingCart.Coupons[couponIndex.Value].CouponApplied);
                    }
                    else if (ShoppingCart.Coupons[couponIndex.Value].Coupon == coupon.Code && coupon.CouponValid && coupon.CouponApplied)
                    {
                        AddMessageToCart(couponIndex, coupon, ShoppingCart.Coupons[couponIndex.Value].CouponApplied);
                    }
                    else if (coupon.CouponValid)
                    {
                        ShoppingCart.ClearPreviousErrorMessages();
                        ShoppingCart.AddPromoDescription = $"Coupon criteria not met {coupon.Code}";
                    }
                }
            }
        }

        /// <summary>
        /// Check if currentItem is the last item in the shopping cart.
        /// </summary>
        /// <param name="currentItem">The current item in the shopping cart.</param>
        /// <returns>True if currentItem is the last item in the shopping cart; otherwise, false.</returns>
        public virtual bool IsLastItem(ZnodeShoppingCartItem currentItem)
        {
            List<int> indexes = new List<int>();
            bool isLastLineItem = false;
            int index = 0;
            int currentItemIndex = ShoppingCart.ShoppingCartItems.IndexOf(currentItem);

            if (!Equals(currentItem, null))
            {
                foreach (ZnodeShoppingCartItem item in ShoppingCart.ShoppingCartItems)
                {
                    if (Equals(currentItem.Product.ProductID, item.Product.ProductID))
                    {
                        index = ShoppingCart.ShoppingCartItems.IndexOf(item);
                        indexes.Add(index);
                    }
                }

                if (Equals(indexes.Count, 1))
                    isLastLineItem = true;

                if (Equals(indexes.IndexOf(currentItemIndex), indexes.LastIndexOf(index)))
                    isLastLineItem = true;
            }
            return isLastLineItem;
        }

        /// <summary>
        /// Gets total quantity ordered for the product.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>                
        /// <returns>The total quantity ordered for the product.</returns>
        public virtual decimal GetQuantityOrdered(int? productId)
        {
            decimal quantity = 0;

            if (productId.HasValue)
            {
                foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
                {
                    if (Equals(cartItem.Product.ProductID, productId.Value))
                    {
                        quantity += cartItem.Quantity;
                        break;
                    }
                }
            }
            return quantity;
        }

        /// <summary>
        ///  Gets total quantity ordered for the product in the cart by catalog.
        /// </summary>
        /// <returns></returns>
        public virtual decimal GetCartQuantity()
        {
            decimal quantity = 0;
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                quantity += cartItem.Quantity;
                foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                {
                    quantity += group.SelectedQuantity;
                }
            }
            return quantity;
        }

        /// <summary>
        /// to check if required product is group product then returns 1
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>quantity</returns>
        public virtual decimal GetGroupProductQuantity(int? productId)
        {
            decimal quantity = 0;
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                if (Equals(cartItem.Product.ProductID, productId) && cartItem?.Product?.ZNodeGroupProductCollection?.Count > 0)
                {
                    quantity = 1;
                    break;
                }
                else if (cartItem?.Product?.ZNodeGroupProductCollection?.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                    {
                        if (Equals(group.ProductID, productId))
                        {
                            quantity = group.SelectedQuantity;
                            break;
                        }
                    }
                }
            }
            return quantity;
        }

        /// <summary>
        /// to check if required product is group product then returns its total cart quantity
        /// </summary>
        /// <param name="productId">productId</param>
        /// /// <param name="childProductId">childProductId</param>
        /// <returns>quantity of the cart of group product.</returns>
        protected virtual decimal GetGroupProductCartQuantity(int? productId, int? childProductId)
        {
            decimal quantity = 0;
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                if (cartItem?.Product?.ZNodeGroupProductCollection?.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                    {
                        if (Equals(cartItem.Product.ProductID, productId) && Equals(group.ProductID, childProductId))
                        {
                            quantity += group.SelectedQuantity;
                        }
                    }
                }
            }
            return quantity;
        }

        /// <summary>
        /// Get shopping cart Sub total.
        /// </summary>
        /// <param name="currentItem">The current item in the shopping cart.</param>
        /// <returns>shopping cart subtotal</returns>
        public virtual decimal GetCartSubTotal(ZnodeShoppingCart ShoppingCart)
        {
            decimal subTotal = 0;
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                subTotal += cartItem.PromotionalPrice * cartItem.Quantity;

                foreach (ZnodeProductBaseEntity addon in cartItem.Product.ZNodeAddonsProductCollection)
                {
                    if (addon.FinalPrice > 0)
                    {
                        decimal addonPrice = addon.FinalPrice;
                        addonPrice = addonPrice * (addon.SelectedQuantity > 0 ? addon.SelectedQuantity : cartItem.Quantity);
                        if (addonPrice <= addon.DiscountAmount)
                        {
                            addonPrice = addon.FinalPrice - addon.DiscountAmount;
                        }
                        subTotal += addonPrice;
                    }
                }

                foreach (ZnodeProductBaseEntity configurable in cartItem.Product.ZNodeConfigurableProductCollection)
                {
                    if (configurable.FinalPrice > 0 && cartItem.Product.FinalPrice == 0)
                    {
                        decimal configurablePrice = configurable.FinalPrice;
                        configurablePrice = configurablePrice * cartItem.Quantity;
                        if (configurablePrice <= configurable.DiscountAmount)
                        {
                            configurablePrice = configurable.FinalPrice - configurable.DiscountAmount;
                        }
                        subTotal += configurablePrice;
                    }
                }
                foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                {
                    if (group.FinalPrice > 0 && cartItem.Product.FinalPrice == 0)
                    {
                        decimal groupPrice = group.FinalPrice;
                        groupPrice = groupPrice * group.SelectedQuantity;
                        if (groupPrice <= group.DiscountAmount)
                        {
                            groupPrice = group.FinalPrice - group.DiscountAmount;
                        }
                        subTotal += groupPrice;
                    }
                }
            }
            return subTotal;
        }

        /// <summary>
        /// Sets Promotional Price And Discount of cart item.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="discount"></param>
        public virtual void SetPromotionalPriceAndDiscount(ZnodeShoppingCartItem cartItem, decimal discount, decimal cartQuantity = 0)
        {
            cartQuantity = cartQuantity == 0 ? cartItem.Quantity : cartQuantity;
            decimal lineItemOrderDiscount = 0;

            if (cartItem.Product.ZNodeConfigurableProductCollection.Count == 0
                    && cartItem.Product.ZNodeGroupProductCollection.Count == 0)
            {
                lineItemOrderDiscount = cartItem.Product.OrderDiscountAmount > 0 ? cartItem.Product.OrderDiscountAmount / cartQuantity : 0;
                cartItem.PromotionalPrice = cartItem.PromotionalPrice > 0 ? cartItem.PromotionalPrice - discount - lineItemOrderDiscount : cartItem.PromotionalPrice;
            }

            foreach (ZnodeProductBaseEntity addon in cartItem.Product.ZNodeAddonsProductCollection)
            {
                if (addon.FinalPrice > 0)
                {
                    decimal addonPrice = addon.FinalPrice;
                    addonPrice  = addonPrice > 0 ? addonPrice - addon.DiscountAmount - addon.OrderDiscountAmount : addonPrice;
                }
            }

            foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
            {
                if (group.FinalPrice > 0)
                {
                    lineItemOrderDiscount = group.OrderDiscountAmount > 0 ? group.OrderDiscountAmount / cartQuantity : 0;
                    cartItem.PromotionalPrice = cartItem.PromotionalPrice > 0 ? cartItem.PromotionalPrice - group.DiscountAmount - lineItemOrderDiscount : cartItem.PromotionalPrice;
                }
            }

            if (cartItem.ExtendedPriceDiscount > 0 && cartQuantity > 0)
            {
                lineItemOrderDiscount = cartItem.Product.OrderDiscountAmount > 0 ? cartItem.Product.OrderDiscountAmount / cartQuantity : 0;
                decimal promotionalPrice = (cartItem.ExtendedPrice - cartItem.ExtendedPriceDiscount -  cartItem.Product.OrderDiscountAmount) / cartQuantity;
                cartItem.PromotionalPrice = promotionalPrice;
            }
            if (cartItem.PromotionalPrice <= 0)
            {
                cartItem.PromotionalPrice = 0;
            }
        }

        /// <summary>
        /// Checks is Discount Applicable for item
        /// </summary>
        /// <param name="discountAmount"></param>
        /// <param name="finalPrice"></param>
        /// <returns>true if applicable else false</returns>
        public virtual bool IsDiscountApplicable(decimal discountAmount, decimal finalPrice)
        {
            bool isDiscountApplicable = true;
            try
            {
                if (discountAmount != 0)
                {
                    if (discountAmount >= finalPrice)
                    {
                        isDiscountApplicable = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, "Promotion", TraceLevel.Error, ex);
            }
            return isDiscountApplicable;
        }

        //to add order line item discount
        public virtual List<OrderDiscountModel> SetOrderDiscountDetails(string discountCode, decimal discountAmount, OrderDiscountTypeEnum discountType, List<OrderDiscountModel> orderDiscount, DiscountLevelTypeIdEnum discountApplicable = DiscountLevelTypeIdEnum.LineItemLevel, decimal? discountMultiplier = null)
        {
            if (discountAmount == 0)
                return orderDiscount;

            if (HelperUtility.IsNull(orderDiscount) || orderDiscount?.Count == 0)
                orderDiscount = new List<OrderDiscountModel>();
            int discountAppliedSequence = ShoppingCart.GetDiscountAppliedSequence(discountCode);

            orderDiscount.Add(new OrderDiscountModel
            {
                OmsDiscountTypeId = (int)discountType,
                DiscountAmount = discountAmount,
                OriginalDiscount = discountAmount,
                DiscountCode = discountCode,
                PromotionName = GetPromotionName(),
                DiscountLevelTypeId = (int)discountApplicable,
                PromotionTypeId = PromotionBag?.PromotionTypeId,
                DiscountAppliedSequence= discountAppliedSequence,
                DiscountMultiplier = discountMultiplier,
                PromotionMessage = PromotionBag?.PromotionMessage
            });
            return orderDiscount;
        }

        //Check user entered coupon code and couponcode from database by ignoring its case
        public bool CheckCouponCodeValid(string userEnteredCoupon, string promotionCoupon) =>
            string.Equals(userEnteredCoupon, promotionCoupon, System.StringComparison.InvariantCultureIgnoreCase);

        //set valid coupon
        public void SetValidCoupon(string couponCode, bool isValid = true)
        {
            foreach (CouponModel item in PromotionBag.Coupons)
            {
                if (CheckCouponCodeValid(item.Code, couponCode))
                {
                    item.CouponValid = isValid;
                }
            }
        }

        //set coupon applied
        public void SetCouponApplied(string couponCode, bool isValid = true)
        {
            foreach (CouponModel item in PromotionBag.Coupons)
            {
                if (CheckCouponCodeValid(item.Code, couponCode))
                {
                    item.CouponApplied = isValid;
                }
            }
        }

        //to get order discount type 
        public OrderDiscountTypeEnum GetDiscountType(string couponCode)
        => string.IsNullOrEmpty(couponCode) ? OrderDiscountTypeEnum.PROMOCODE : OrderDiscountTypeEnum.COUPONCODE;

        //to get order discount code 
        public string GetDiscountCode(string promoCode, string couponCode = "")
        => string.IsNullOrEmpty(couponCode) ? promoCode : couponCode;

        //to check coupon is unique & applied to cart
        public bool IsUniqueCouponApplied(ZnodePromotionBag promo, bool isCouponApplied)
            => (promo.IsUnique && isCouponApplied);

        //to check coupon is applied to existing order thats quantity already deducted
        public bool IsExistingOrderCoupon(string couponcode)
        {
            if (!string.IsNullOrEmpty(couponcode))
            {
                int orderId = GetOrderIdFromCart();
                if (orderId > 0)
                {
                    IZnodePromotionHelper promotionHelper = GetService<IZnodePromotionHelper>(); 
                    return promotionHelper.IsExistingOrderCoupon(orderId, couponcode);
                }
            }
            return false;
        }

        public bool IsCouponApplicableForLineItem(string couponCode)
        {
            bool isValidCoupon = false;
            if (HelperUtility.IsNotNull(ShoppingCart.DiscountAppliedSequence))
              isValidCoupon = ShoppingCart.DiscountAppliedSequence.ContainsKey(couponCode);
           return isValidCoupon;
        }
        public virtual decimal GetAmountOffShippingDiscount(decimal shippingCost, decimal promotionDiscount)
        {
            decimal discountAmount = (shippingCost <= promotionDiscount) ? shippingCost : promotionDiscount;
            return discountAmount;
        }
        public virtual decimal GetPercentOffShippingDiscount(decimal shippingCost, decimal shippingDiscount, decimal promotionDiscount)
        {
            decimal discountAmount = (shippingCost - shippingDiscount) * (promotionDiscount / 100);
            return discountAmount;
        }
        public virtual bool SetShippingLevelDiscount(decimal discountAmount,string couponCode = "")
        {
            OrderDiscountModel orderDiscount = new OrderDiscountModel();
            orderDiscount.DiscountMultiplier = ShoppingCart.GetShippingDiscountMultiplier(discountAmount);
            orderDiscount.DiscountCode = GetDiscountCode(PromotionBag.PromoCode, couponCode);
            OrderDiscountTypeEnum discountType = GetDiscountType(couponCode);
            orderDiscount.OmsDiscountTypeId = Convert.ToInt32(discountType);
            orderDiscount.DiscountLevelTypeId = Convert.ToInt32(DiscountLevelTypeIdEnum.ShippingLevel);
            orderDiscount.PromotionName = PromotionBag?.PromotionName;
            orderDiscount.PromotionTypeId = PromotionBag?.PromotionTypeId;
            orderDiscount.PromotionMessage = PromotionBag?.PromotionMessage;
            ShoppingCart.OrderLevelDiscountDetails = SetOrderDiscountDetails(orderDiscount.DiscountCode, discountAmount, discountType, ShoppingCart.OrderLevelDiscountDetails, DiscountLevelTypeIdEnum.ShippingLevel, orderDiscount.DiscountMultiplier);
            return HelperUtility.IsNotNull(ShoppingCart.OrderLevelDiscountDetails);
        }
        #endregion

        #region Private Method

        //to get orderId from cart
        private int GetOrderIdFromCart()
        {
            if (HelperUtility.IsNotNull(ShoppingCart))
                return ShoppingCart?.OrderId ?? 0;
            return 0;
        }
        //to check couponcode is valid 
        private bool IsValidCouponCode(string couponCode)
        {
            foreach (CouponModel item in PromotionBag.Coupons)
            {
                if (CheckCouponCodeValid(item.Code, couponCode) && item.AvailableQuantity > 0)
                {
                    if (GetOrderIdFromCart() > 0)
                    {
                        item.IsExistInOrder = IsExistingOrderCoupon(couponCode);
                    }
                    return true;
                }
                else if (CheckCouponCodeValid(item.Code, couponCode) && IsExistingOrderCoupon(couponCode))
                {
                    item.IsExistInOrder = true;
                    return true;
                }
            }
            return false;
        }

        //to add promotion message to cart
        private void AddMessageToCart(int? couponIndex, CouponModel coupon, bool isCouponApplied)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(PromotionBag.PromotionMessage) && isCouponApplied)
            {
                ShoppingCart.ClearPreviousErrorMessages();
                message = $"{ PromotionBag.PromotionMessage}";
                ShoppingCart.AddPromoDescription = $"{ PromotionBag.PromotionMessage} { coupon.Code}";
            }
            else if (isCouponApplied)
            {
                ShoppingCart.AddPromoDescription = $"Coupon code accepted {coupon.Code}";
                message = GlobalSetting_Resources.MessageCouponAccepted;
            }
            else if(!isCouponApplied)
            {
                message = Api_Resources.MessageMaximumApplicableDiscount;
            }
            ShoppingCart.Coupons[couponIndex.Value].CouponMessage = message;
            ShoppingCart.Coupons[couponIndex.Value].IsExistInOrder = coupon.IsExistInOrder;
        }

        protected virtual string GetPromotionName()
        {
            if (HelperUtility.IsNotNull(PromotionBag))
                return PromotionBag.PromotionName;
            return null;
        }      
        #endregion
    }
}
