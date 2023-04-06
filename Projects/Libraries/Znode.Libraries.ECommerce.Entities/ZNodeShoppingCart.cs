using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Libraries.ECommerce.Entities
{
    /// <summary>
    /// Represents Shopping cart and shopping cart items
    /// </summary>
    [Serializable()]
    public class ZnodeShoppingCart : ZnodeBusinessBase, IZnodeShoppingCartEntities
    {
        #region Member Variables

        private StringBuilder _PromoDescription = new StringBuilder();
        protected StringBuilder _ErrorMessage = new StringBuilder();
        private decimal _OrderLevelTaxes = 0;
        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _orderDetailRepository;

        #endregion Member Variables

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ZNodeShoppingCart class
        /// </summary>
        public ZnodeShoppingCart()
        {
            ShoppingCartItems = new List<ZnodeShoppingCartItem>();
            Shipping = new ZnodeShipping();
            Payment = new ZnodePayment();
            Coupons = new List<ZnodeCoupon>();
            Vouchers = new List<ZnodeVoucher>();
            _orderDetailRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
        }

        #endregion Constructors

        #region Public Properties

        // Gets the error messages that have been created in this cart.
        public string ErrorMessage { get; }

        // Gets or sets a collection of shopping cart items
        public List<ZnodeShoppingCartItem> ShoppingCartItems { get; set; }

        // Gets or sets a value indicating whether any promotion applied
        public bool IsAnyPromotionApplied { get; set; }

        // Gets or sets the Shipping object
        public ZnodeShipping Shipping { get; set; }

        // Gets or sets the payment object
        public ZnodePayment Payment { get; set; }

        // Gets or sets the User Address
        public UserAddressModel UserAddress { get; set; }

        private decimal? totalPartialRefundAmount = null;

        // Gets or sets the TotalPartialRefundAmount
        public virtual decimal TotalPartialRefundAmount
        {
            get
            {
                if (IsNotNull(totalPartialRefundAmount))
                    return totalPartialRefundAmount.GetValueOrDefault();

                decimal? totalrefundAmount = ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(x => x.PartialRefundAmount);
                return totalrefundAmount ?? 0;
            }

            set
            {
                totalPartialRefundAmount = value;
            }
        }

        private decimal? shippingCost = null;

        // Gets the total shipping cost of line items in the shopping cart.
        public virtual decimal ShippingCost
        {
            get
            {
                if (IsNotNull(shippingCost))
                    return shippingCost.GetValueOrDefault();
                
                decimal totalShippingCost = ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(x => x.ShippingCost) + OrderLevelShipping;
                decimal shippingDiscount = Shipping.ShippingDiscount;
                return !Equals(CustomShippingCost, null) ? CustomShippingCost.GetValueOrDefault() : totalShippingCost;
            }

            set
            {
                shippingCost = value;
            }
        }

        private decimal? shippingDifference = null;

        // Gets the total shipping difference of line items in the shopping cart.
        public virtual decimal ShippingDifference
        {
            get
            {
                if (IsNotNull(shippingDifference))
                    return shippingDifference.GetValueOrDefault();

                decimal _shippingDifference = 0;
                int? _orderId = this.OrderId;
                if (_orderId != null && _orderId > 0)
                {
                    ZnodeOmsOrderDetail orderDetail = _orderDetailRepository.Table.Where(x => x.OmsOrderId == _orderId && x.IsActive)?.FirstOrDefault();
                    if (orderDetail != null)
                    {
                        bool isShippingCostUpdated = orderDetail.IsShippingCostEdited ?? false;

                        if (isShippingCostUpdated || !Equals(CustomShippingCost, null))
                            return _shippingDifference;

                        if (IsLineItemReturned)
                        {
                            decimal _ordershippingCost = (orderDetail.ShippingCost ?? 0);
                            _ordershippingCost += (orderDetail.ShippingDifference ?? 0);
                            if (_ordershippingCost > 0 && ShippingCost > 0)
                            {
                                _shippingDifference = (_ordershippingCost - ShippingCost);
                                if (_shippingDifference > 0)
                                {
                                    _shippingDifference = _shippingDifference - CartItemShippingRefundAmount();
                                    if (_shippingDifference < 0)
                                        _shippingDifference = 0;
                                }
                            }
                        }
                        else
                        {
                            _shippingDifference = orderDetail.ShippingDifference ?? 0;
                        }
                    }
                }
                return _shippingDifference;
            }

            set
            {
                shippingDifference = value;
            }
        }

        // Gets or sets the discounted applied to this order.
        public decimal OrderLevelDiscount { get; set; }

        // Gets or sets the Gift Card Amount.
        public decimal GiftCardAmount { get; set; } = 0;

        // Gets or sets the Gift Card Balance.
        public decimal GiftCardBalance { get; set; } = 0;

        // Gets or sets the gift card number
        public string GiftCardNumber { get; set; }

        // Gets or sets the Gift Card Message.
        public string GiftCardMessage { get; set; }

        // Gets or sets a value indicating whether the gift card number is valid.
        public bool IsGiftCardValid { get; set; }

        // Gets or sets a value indicating whether the line item is returned or not.
        public bool IsLineItemReturned { get; set; }

        // Gets or sets a value indicating whether the gift card is applied to the shopping cart.
        public bool IsGiftCardApplied { get; set; }

        // Gets or sets the CSR Discount for internal calculation.
        public decimal CSRDiscount { get; set; } = 0;

        // Gets or sets the CSR Discount Amount.
        public decimal CSRDiscountAmount { get; set; }

        // Gets or sets the CSR Discount Description.
        public string CSRDiscountDescription { get; set; }

        // Gets or sets the CSR Discount Message.
        public string CSRDiscountMessage { get; set; }

        // Gets or sets the CSR Discount Applied.
        public bool CSRDiscountApplied { get; set; }

        // Gets or sets the custom shipping cost amount.
        public decimal? CustomShippingCost { get; set; }

        // Get or sets the estimate shipping cost.
        public decimal? EstimateShippingCost { get; set; }

        // Gets or sets the custom tax cost amount.
        public decimal? CustomTaxCost { get; set; }

        // Gets or sets the Order Attribute to be save along with order submit.
        public string OrderAttribute { get; set; }

        //Gets or sets the externalId for ERP data.
        public string ExternalId { get; set; }

        // Gets or sets the shipping cost for this order.
        public decimal OrderLevelShipping { get; set; } = 0;

        // Gets or sets the tax cost for this order.
        public decimal OrderLevelTaxes
        {
            get
            {
                return !Equals(CustomTaxCost, null) ? CustomTaxCost.GetValueOrDefault() : _OrderLevelTaxes + SalesTax + VAT + HST + PST + GST;
            }

            set { _OrderLevelTaxes = value; }
        }

        private decimal? taxCost = null;

        // Gets or sets the total tax cost of items in the shopping cart.
        public virtual decimal TaxCost
        {
            get
            {
                if (IsNotNull(taxCost))
                    return taxCost.GetValueOrDefault();

                decimal totalTaxCost = !Equals(CustomTaxCost, null) ? CustomTaxCost.GetValueOrDefault() : (ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(x => x.TaxCost) + TaxOnShipping);
                return totalTaxCost;
            }

            set
            {
                taxCost = value;
            }
        }

        // Gets or sets the total sales tax cost of items in the shopping cart
        public decimal SalesTax { get; set; }

        // Gets or sets the total tax cost of items in the shopping cart.
        public decimal VAT { get; set; }

        // Gets or sets the total Import Duty of items in the shopping cart.
        public decimal ImportDuty { get; set; }
        
        // Gets or sets the total tax cost of items in the shopping cart
        public decimal PST { get; set; }

        // Gets or sets the total tax cost of items in the shopping cart.
        public decimal HST { get; set; }

        // Gets or sets the total tax cost of items in the shopping cart.
        public decimal GST { get; set; }

        // Gets or sets the sales tax rate (%)
        public decimal TaxRate { get; set; }

        // Gets or sets the sales tax rate (%)
        public decimal TaxOnShipping { get; set; }

        // Gets the count of items in the shopping cart
        public int Count
        {
            get { return ShoppingCartItems.Count; }
        }

        private decimal? subTotal = null;

        // Gets the total cost of items in the shopping cart before shipping and taxes
        public virtual decimal SubTotal
        {
            get
            {
                if (IsNotNull(subTotal))
                    return subTotal.GetValueOrDefault();

                return ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(x => x.ExtendedPrice);
            }
            
            set
            {
                subTotal = value;
            }
        }

        private decimal? discount = null;

        // Gets total discount of applied to the items in the shopping cart.
        public virtual decimal Discount
        {
            get
            {
                if (IsNotNull(discount))
                    return discount.GetValueOrDefault();

               decimal totalDiscount = OrderLevelDiscount + ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(x => x.ExtendedPriceDiscount + x.DiscountAmount);

                totalDiscount = totalDiscount + TotalPartialRefundAmount;
                            

                return totalDiscount;
            }

            set
            {
                discount = value;
            }
        }

        private decimal? orderTotalWithoutVoucher = null;

        // Gets the total cost after shipping, taxes and promotions
        public virtual decimal OrderTotalWithoutVoucher
        {

            get
            {
                if (HelperUtility.IsNotNull(orderTotalWithoutVoucher))
                    return orderTotalWithoutVoucher.GetValueOrDefault();

                return (SubTotal - Discount) + ShippingCost + ShippingHandlingCharges + ReturnCharges + OrderLevelTaxes - CSRDiscount - (ShippingCost > 0 ? Shipping.ShippingDiscount : 0);
            }
            set
            {
                orderTotalWithoutVoucher = value;
            }
        }

        private decimal? total = null;

        public virtual decimal Total
        {
            get
            {
                if (HelperUtility.IsNotNull(total))
                    return total.GetValueOrDefault();

                return OrderTotalWithoutVoucher - GiftCardAmount;
            }
            set
            {
                total = value;
            }
        }

        public virtual decimal TotalAdditionalCost { get; set; }

        private string promoDescription = string.Empty;

        // Gets all of the promotion descriptions that have been added to the cart.
        public string PromoDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(promoDescription))
                    return promoDescription;

                StringBuilder promoDesc = new StringBuilder(_PromoDescription.ToString());

                foreach (ZnodeShoppingCartItem item in ShoppingCartItems)
                {
                    if (promoDesc.Length > 0)
                        _PromoDescription.Append("<br/>");

                    promoDesc.Append(item.PromoDescription.ToString());
                }
                return promoDesc.ToString();
            }

            set
            {
                promoDescription = value;
            }
        }

        // Gets or sets the promotion description
        public string AddPromoDescription
        {
            get { return _PromoDescription.ToString(); }

            set { _PromoDescription.Append(value); }
        }

        // Gets or sets a message to the shopping cart main error message.
        public string AddErrorMessage
        {
            get
            {
                if (_ErrorMessage.Length > 0)
                    _ErrorMessage.Append("<br/>");

                _ErrorMessage.Append(ErrorMessage);

                return _ErrorMessage.ToString();
            }

            set { _ErrorMessage.Append(value); }
        }

        // for paypal express checkout for Api
        public string Token { get; set; }

        public string PayerId { get; set; }

        //for setting PortalId in case of franchise admin
        public int? PortalId { get; set; }

        public int? UserId { get; set; }

        public string LoginUserName { get; set; }

        /// <summary>
        /// To get set the ip address
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// To get set the InHandDate
        /// </summary>
        public DateTime? InHandDate { get; set; }

        /// <summary>
        /// To get set the Job Name
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// To get set the Shipping Constraint Code
        /// </summary>
        public string ShippingConstraintCode { get; set; }

        //depends on the first coupon applied.
        public bool CartAllowsMultiCoupon { get; set; }

        public int? OrderLineItemRelationshipId { get; set; }

        public bool SkipShippingCalculations { get; set; }

        public bool FreeShipping { get; set; }
        
        public List<TaxSummaryModel> TaxSummaryList { get; set; }

        public List<string> TaxMessageList { get; set; }

        #endregion Public Properties

        /// <summary>
        /// Gets or sets Znode Coupons for applied in shopping cart.
        /// </summary>
        public List<ZnodeCoupon> Coupons { get; set; }

        /// <summary>
        /// Gets or sets Znode product description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Clear the previous error messages.
        /// </summary>
        public void ClearPreviousErrorMessages()
        {
            _ErrorMessage = new StringBuilder();
            _PromoDescription = new StringBuilder();
        }

        public int? ProfileId { get; set; }

        // Gets or sets a OrderId of shopping cart
        public int? OrderId { get; set; }

        // Gets or sets a OrderDate of shopping cart
        public DateTime? OrderDate { get; set; }

        // Gets or sets the discount description applied to this line item.
        public List<OrderDiscountModel> OrderLevelDiscountDetails { get; set; }

        public int LocalId { get; set; }
        public int PublishedCatalogId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySuffix { get; set; }
        public int PublishStateId { get; set; }
        //Get and set Personalise Value list
        public Dictionary<string, object> PersonaliseValuesList { get; set; }
        public List<PersonaliseValueModel> PersonaliseValueDetail { get; set; }

        public string ApproximateArrival { get; set; }
        public string CreditCardNumber { get; set; }
        public bool IsCchCalculate { get; set; }
        public List<ReturnOrderLineItemModel> ReturnItemList { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }
        public string CardType { get; set; }
        public int? CreditCardExpMonth { get; set; }
        public int? CreditCardExpYear { get; set; }

        public bool IsAllowWithOtherPromotionsAndCoupons { get; set; }
        public bool IsCalculatePromotionAndCoupon { get; set; }

        public string CultureCode { get; set; }

        public List<ZnodeVoucher> Vouchers { get; set; }
        public bool IsCalculateVoucher { get; set; }
        public bool IsAllVoucherRemoved { get; set; }
        public bool IsRemoveShippingDiscount { get; set; }
        public List<OrderPromotionModel> InvalidOrderLevelShippingDiscount { get; set; }
        public List<PromotionModel> InvalidOrderLevelShippingPromotion { get; set; }
        public bool IsShippingDiscountRecalculate { get; set; }
        public bool? IsCalculateTaxAfterDiscount { get; set; }
        public bool? IsPricesInclusiveOfTaxes { get; set; }
        public decimal ShippingDiscount { get; set; }
        public decimal ShippingHandlingCharges { get; set; }
        public bool IsShippingRecalculate { get; set; }
        public decimal ReturnCharges { get; set; }
        public bool IsQuoteOrder { get; set; }
        /// <summary>
        /// DiscountCode and Discount Applied sequence
        /// </summary>
        public Dictionary<string, int> DiscountAppliedSequence { get; set; }

        /// <summary>
        /// DiscountCode and Discount Product Quantity
        /// </summary>
        public Dictionary<string, decimal> DiscountedProductQuantity { get; set; }
        public bool IsOldOrder { get; set; }
        //Use to decide call live shipping method or not
        public bool IsCallLiveShipping { get; set; }
        public bool IsFromAdminOrder { get; set; }
        public bool CSRDiscountEdited { get; set; }
        public virtual bool IsQuoteToOrder { get; set; }

        public bool IsPendingOrderRequest { get; set; }

        public bool IsTaxExempt { get; set; }
        public bool? AvataxIsSellerImporterOfRecord { get; set; }
        /// <summary>
        /// The flag is used to validate order details at API level.
        /// </summary>
        public bool SkipPreprocessing { get; set; }

        #region Public Method
        public string BusinessIdentificationNumber { get; set; }

        // Calculate and distribute CSR discount on line item.
        public virtual void CalculateLineItemCSRDiscount()
        {
            if (CSRDiscountAmount > 0)
            {
                if (GetMaximumApplicableDiscountOnSubtotal() > 0)
                {
                    OrderDiscountModel orderDiscountModel = new OrderDiscountModel();
                    orderDiscountModel.DiscountCode = OrderDiscountTypeEnum.CSRDISCOUNT.ToString();
                    orderDiscountModel.DiscountAmount = CSRDiscountAmount;
                    orderDiscountModel.DiscountLevelTypeId = (int)DiscountLevelTypeIdEnum.CSRLevel;
                    orderDiscountModel.OmsDiscountTypeId = (int)OrderDiscountTypeEnum.CSRDISCOUNT;
                    orderDiscountModel.DiscountMultiplier = GetDiscountMultiplier(CSRDiscountAmount);
                    DistributeOrderDiscountAmount(orderDiscountModel);
                }
            }
        }

        // Return Shipping discount multiplier.
        public virtual decimal GetShippingDiscountMultiplier(decimal totalShippingDiscount)
        {
            decimal shippingDiscountMultiplier = 0;
            if (totalShippingDiscount > 0 && this?.ShoppingCartItems?.Count > 0 && ShippingCost > 0)
            {
                shippingDiscountMultiplier = totalShippingDiscount / ShippingCost;
            }
            return shippingDiscountMultiplier;
        }

        // Return Max applicable subtotal
        public virtual decimal GetMaximumApplicableDiscountOnSubtotal()
        {
            decimal totalDiscount = OrderLevelDiscount + ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Sum(x => x.ExtendedPriceDiscount + x.DiscountAmount);
            decimal subTotal = SubTotal - totalDiscount;
            return subTotal;
        }

        // Return Order/CSR discount multiplier.
        public virtual decimal GetDiscountMultiplier(decimal discountAmount)
        {
            decimal discountMultiplier = 0;
            decimal maxApplicableSubtotal = GetMaximumApplicableDiscountOnSubtotal();
            if (discountAmount > 0 && maxApplicableSubtotal > 0)
            {
                if (discountAmount > maxApplicableSubtotal)
                    discountAmount = maxApplicableSubtotal;
                discountMultiplier = discountAmount / maxApplicableSubtotal;
            }

            return discountMultiplier;
        }

        // Return Max applicable line item value after applying discount.
        public virtual decimal GetMaximumApplicableDiscountOnLineItem(decimal discountAmount, decimal quantity, ZnodeProductBaseEntity product, ZnodeProductBaseEntity childProduct = null, string discountCode = "", bool isConfigurableProduct = false)
        {
            decimal lineItemDiscount = 0;
            decimal lineItemOrderDiscount = 0;
            decimal maxApplicableDiscount = 0;
            decimal extendedPricelineItemDiscount = GetExtendedPriceDiscountForLineItem(product, discountCode); 
            if (HelperUtility.IsNotNull(childProduct))
            {
                lineItemDiscount = childProduct.DiscountAmount > 0 ? childProduct.DiscountAmount : 0;
                lineItemOrderDiscount = childProduct.OrderDiscountAmount > 0 ? childProduct.OrderDiscountAmount / quantity : 0;
                maxApplicableDiscount =  childProduct.PromotionalPrice - GetParentChildTotalDiscount(product, childProduct) - extendedPricelineItemDiscount;
            }
            else
            {
                lineItemDiscount = product.DiscountAmount > 0 ? product.DiscountAmount : 0;
                lineItemOrderDiscount = product.OrderDiscountAmount > 0 ? product.OrderDiscountAmount / quantity : 0;
                maxApplicableDiscount = product.PromotionalPrice - GetParentChildTotalDiscount(product, childProduct) - extendedPricelineItemDiscount;
            }
            maxApplicableDiscount = discountAmount > maxApplicableDiscount ? maxApplicableDiscount : discountAmount;
            // If maxApplicableDiscount is in negative then will set as 0;
            if (maxApplicableDiscount < 0)
                maxApplicableDiscount = 0;
            return maxApplicableDiscount;
        }


        // Return discount price of line item.
        public virtual decimal GetDiscountedPrice(ZnodeProductBaseEntity product, decimal quantity, ZnodeProductBaseEntity childproduct = null, string discountCode = "", bool isConfigurableProduct = false)
        {
            decimal discountedPrice = 0;
            decimal orderDiscount = product.OrderDiscountAmount > 0 ? product.OrderDiscountAmount / quantity : 0;
            decimal extendedPriceLineItemDiscount = GetExtendedPriceDiscountForLineItem(product, discountCode);
            decimal basePrice = (childproduct == null) ? product.PromotionalPrice :  childproduct.PromotionalPrice;
            discountedPrice = basePrice - GetParentChildTotalDiscount(product, childproduct) - extendedPriceLineItemDiscount;
            return discountedPrice;
        }

        // Get ExtendedPrice Discount in case of ZnodeCartPromotionPercentOffXifYPurchased/ZnodeCartPromotionAmountOffXifYPurchased
        protected virtual decimal GetExtendedPriceDiscountForLineItem(ZnodeProductBaseEntity product, string discountCode)
        {
            decimal lineItemExtendedPriceDiscount = 0;
            decimal discountedProductQuantity = 0;

            // Get discountedProductQuantity if Promotion Type is ZnodeCartPromotionPercentOffXifYPurchased and ZnodeCartPromotionAmountOffXifYPurchased. 
            if (HelperUtility.IsNotNull(DiscountedProductQuantity) && !string.IsNullOrEmpty(discountCode) && DiscountedProductQuantity.ContainsKey(discountCode))
                discountedProductQuantity = DiscountedProductQuantity[discountCode];

            ZnodeShoppingCartItem cartItem = ShoppingCartItems.Cast<ZnodeShoppingCartItem>().Where(x => x.ParentProductSKU == product.SKU).FirstOrDefault();
            if (HelperUtility.IsNotNull(cartItem))
            {
                discountedProductQuantity = discountedProductQuantity > 0 ? discountedProductQuantity : cartItem.Quantity;
                if (cartItem.ExtendedPriceDiscount > 0 && discountedProductQuantity > 0)
                    lineItemExtendedPriceDiscount = cartItem.ExtendedPriceDiscount / discountedProductQuantity;
            }

            return lineItemExtendedPriceDiscount;
        }


        // Split Order and CSR discount on line Items.
        public virtual decimal DistributeOrderDiscountAmount(OrderDiscountModel orderDiscountModel)
        {
            decimal appliedDiscount = 0;
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCartItems)
            {
                if (cartItem.Product.ZNodeGroupProductCollection.Count == 0 && cartItem.Product.ZNodeConfigurableProductCollection.Count==0)
                {
                    appliedDiscount += ApplyLineItemOrderDiscount(cartItem.Product, cartItem.Quantity, orderDiscountModel);
                }

                if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                    {
                        appliedDiscount += ApplyLineItemOrderDiscount(group, cartItem.Quantity, orderDiscountModel);
                    }
                }

                if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity config in cartItem.Product.ZNodeConfigurableProductCollection)
                    {
                        appliedDiscount += ApplyLineItemOrderDiscount(cartItem.Product, cartItem.Quantity, orderDiscountModel, config);
                    }
                }


                if (cartItem.Product.ZNodeAddonsProductCollection.Count > 0)
                {
                    foreach (ZnodeProductBaseEntity addon in cartItem.Product.ZNodeAddonsProductCollection)
                    {
                        appliedDiscount += ApplyLineItemOrderDiscount(addon, cartItem.Quantity, orderDiscountModel);
                    }
                }


            }
            return appliedDiscount;
        }

        // Set discount sequence value in which Promotion/Coupons gets applied.
        public virtual int GetDiscountAppliedSequence(string discountCode)
        {
            int discountAppliedSequence = 0;
            if (HelperUtility.IsNull(DiscountAppliedSequence) || DiscountAppliedSequence?.Count == 0)
                DiscountAppliedSequence = new Dictionary<string, int>();

            if (!DiscountAppliedSequence.ContainsKey(discountCode))
            {
                discountAppliedSequence = DiscountAppliedSequence.OrderByDescending(x => x.Value).FirstOrDefault().Value + 1;
                DiscountAppliedSequence.Add(discountCode, discountAppliedSequence);
            }
            else
                discountAppliedSequence = DiscountAppliedSequence[discountCode];

            return discountAppliedSequence;
        }

        #endregion

        #region Protected Method 

        protected virtual decimal GetParentChildTotalDiscount(ZnodeProductBaseEntity product, ZnodeProductBaseEntity childProduct = null)
        {
            decimal totalAppliedDiscount = 0;
            if (HelperUtility.IsNotNull(product?.OrdersDiscount))
            {
                foreach (var item in product?.OrdersDiscount)
                {
                    if (item.PromotionTypeId != (int)PromotionTypeEnum.ZnodeCartPromotionPercentOffXifYPurchased && item.PromotionTypeId != (int)PromotionTypeEnum.ZnodeCartPromotionAmountOffXifYPurchased)
                        totalAppliedDiscount = totalAppliedDiscount + item.OriginalDiscount.GetValueOrDefault();
                }
            }

            if (HelperUtility.IsNotNull(childProduct?.OrdersDiscount))
            {
                foreach (var item in childProduct?.OrdersDiscount)
                {
                    if (item.PromotionTypeId != (int)PromotionTypeEnum.ZnodeCartPromotionPercentOffXifYPurchased && item.PromotionTypeId != (int)PromotionTypeEnum.ZnodeCartPromotionAmountOffXifYPurchased)
                        totalAppliedDiscount = totalAppliedDiscount + item.OriginalDiscount.GetValueOrDefault();
                }
            }

            return totalAppliedDiscount;
        }

        #endregion

        #region Private Method

        //to get cart item refund amount applied for Shipping in case of IsReturnShipping
        private decimal CartItemShippingRefundAmount()
        {
            decimal refundAmount = 0;
            if (this?.ReturnItemList?.Count > 0 && this?.ShoppingCartItems?.Count > 0)
            {
                foreach (ReturnOrderLineItemModel item in this?.ReturnItemList)
                {
                    if (item.IsShippingReturn)
                    {
                        refundAmount += GetRefundAmountByQuantity(item);
                    }
                }
            }
            return refundAmount;
        }

        // to get refund amount by quantity for cart item
        private decimal GetRefundAmountByQuantity(ReturnOrderLineItemModel cartItem)
        {
            return cartItem.IsShippingReturn ? cartItem.ShippingCost : 0;
        }


        // Set line Item order discount
        private decimal ApplyLineItemOrderDiscount(ZnodeProductBaseEntity product, decimal quantity, OrderDiscountModel orderDiscountModel, ZnodeProductBaseEntity childProduct = null, bool isConfigurableProduct = false)
        {
            decimal lineItemOrderDiscount = 0;
            if (product.FinalPrice > 0.0M)
            {
                lineItemOrderDiscount = GetOrderDiscountAmount(product, orderDiscountModel.DiscountMultiplier.GetValueOrDefault(), quantity,orderDiscountModel.DiscountCode, childProduct, isConfigurableProduct);
                if (lineItemOrderDiscount > 0)
                {
                    if (orderDiscountModel.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.CSRDISCOUNT)
                        product.CSRDiscountAmount += lineItemOrderDiscount;
                    else
                        product.OrderDiscountAmount += lineItemOrderDiscount;
                    orderDiscountModel.DiscountAmount = lineItemOrderDiscount / quantity;
                    product.OrdersDiscount = SetOrderDiscountDetails(orderDiscountModel, product.OrdersDiscount);
                }
            }
            return lineItemOrderDiscount;
        }        
        // To get Order Promotion/Coupon and CSR discount
        private decimal GetOrderDiscountAmount(ZnodeProductBaseEntity product, decimal discountMultiplier, decimal quantity, string discountCode, ZnodeProductBaseEntity childProduct, bool isConfigurableProduct = false)
        {
            if (product.FinalPrice > 0.0M)
            {
                decimal finalPrice = GetDiscountedPrice(product, quantity, childProduct, discountCode,isConfigurableProduct);
                decimal maxApplicableDiscount = GetMaximumApplicableDiscountOnLineItem(finalPrice * discountMultiplier, quantity, product, childProduct, discountCode, isConfigurableProduct) * quantity;
                return maxApplicableDiscount;
            }
            return 0;
        }

        // Bind line item discount in list
        private List<OrderDiscountModel> SetOrderDiscountDetails(OrderDiscountModel orderDiscountModel, List<OrderDiscountModel> orderDiscount)
        {
            if (orderDiscountModel.DiscountAmount == 0)
                return orderDiscount;

            if (HelperUtility.IsNull(orderDiscount) || orderDiscount?.Count == 0)
                orderDiscount = new List<OrderDiscountModel>();

            int discountAppliedSequence = GetDiscountAppliedSequence(orderDiscountModel.DiscountCode);

            orderDiscount.Add(new OrderDiscountModel
            {
                OmsDiscountTypeId = orderDiscountModel.OmsDiscountTypeId,
                DiscountAmount = orderDiscountModel.DiscountAmount,
                OriginalDiscount = orderDiscountModel.DiscountAmount,
                DiscountCode = orderDiscountModel.DiscountCode,
                PromotionName = orderDiscountModel.PromotionName,
                DiscountLevelTypeId = orderDiscountModel.DiscountLevelTypeId,
                PromotionTypeId = orderDiscountModel.PromotionTypeId,
                DiscountAppliedSequence = discountAppliedSequence,
                PromotionMessage = orderDiscountModel.PromotionMessage
            });
            return orderDiscount;
        }
        #endregion Private Method
    }
}