using System;
using System.Collections.Generic;

using Znode.Engine.Api.Models;

namespace Znode.Libraries.ECommerce.Entities
{
    public interface IZnodeShoppingCartEntities
    {
        string ErrorMessage { get; }

        // Gets or sets a collection of shopping cart items
        List<ZnodeShoppingCartItem> ShoppingCartItems { get; set; }

        // Gets or sets a value indicating whether any promotion applied
        bool IsAnyPromotionApplied { get; set; }

        // Gets or sets the Shipping object
        ZnodeShipping Shipping { get; set; }

        // Gets or sets the payment object
        ZnodePayment Payment { get; set; }

        // Gets or sets the User Address
        UserAddressModel UserAddress { get; set; }

        // Gets or sets the TotalPartialRefundAmount
        decimal TotalPartialRefundAmount { get; set; }

        // Gets the total shipping cost of line items in the shopping cart.
        decimal ShippingCost { get;  set; }


        // Gets the total shipping difference of line items in the shopping cart.
        decimal ShippingDifference { get; set; }


        // Gets or sets the discounted applied to this order.
        decimal OrderLevelDiscount { get; set; }

        // Gets or sets the Gift Card Amount.
        decimal GiftCardAmount
        {
            get; set;
        }

        // Gets or sets the Gift Card Balance.
        decimal GiftCardBalance
        {
            get; set;
        }

        // Gets or sets the gift card number
        string GiftCardNumber { get; set; }

        // Gets or sets the Gift Card Message.
        string GiftCardMessage { get; set; }

        // Gets or sets a value indicating whether the gift card number is valid.
        bool IsGiftCardValid { get; set; }

        // Gets or sets a value indicating whether the line item is returned or not.
        bool IsLineItemReturned { get; set; }

        // Gets or sets a value indicating whether the gift card is applied to the shopping cart.
        bool IsGiftCardApplied { get; set; }

        // Gets or sets the CSR Discount for internal calculation.
        decimal CSRDiscount
        {
            get; set;
        }

        // Gets or sets the CSR Discount Amount.
        decimal CSRDiscountAmount { get; set; }

        // Gets or sets the CSR Discount Description.
        string CSRDiscountDescription { get; set; }

        // Gets or sets the CSR Discount Message.
        string CSRDiscountMessage { get; set; }

        // Gets or sets the CSR Discount Applied.
        bool CSRDiscountApplied { get; set; }

        // Gets or sets the CSR Discount Edited.
        bool CSRDiscountEdited { get; set; }

        // Gets or sets the custom shipping cost amount.
        decimal? CustomShippingCost { get; set; }

        // Get or sets the estimate shipping cost.
        decimal? EstimateShippingCost { get; set; }

        // Gets or sets the custom tax cost amount.
        decimal? CustomTaxCost { get; set; }

        // Gets or sets the Order Attribute to be save along with order submit.
        string OrderAttribute { get; set; }

        //Gets or sets the externalId for ERP data.
        string ExternalId { get; set; }

        // Gets or sets the shipping cost for this order.
        decimal OrderLevelShipping { get; set; }

        // Gets or sets the tax cost for this order.
        decimal OrderLevelTaxes { get; set; }
        

        // Gets or sets the total tax cost of items in the shopping cart.
        decimal TaxCost { get; set; }

        // Gets or sets the total sales tax cost of items in the shopping cart
        decimal SalesTax { get; set; }

        // Gets or sets the total tax cost of items in the shopping cart.
        decimal VAT { get; set; }

        // Gets or sets the total tax cost of items in the shopping cart
        decimal PST { get; set; }

        // Gets or sets the total tax cost of items in the shopping cart.
        decimal HST { get; set; }

        // Gets or sets the total tax cost of items in the shopping cart.
        decimal GST { get; set; }

        // Gets or sets the total import duty of items in the shopping cart.
        decimal ImportDuty { get; set; }

        List<TaxSummaryModel> TaxSummaryList { get; set; }

        List<string> TaxMessageList { get; set; }

        // Gets or sets the sales tax rate (%)
        decimal TaxRate { get; set; }

        // Gets or sets the sales tax rate (%)
        decimal TaxOnShipping { get; set; }

        // Gets the count of items in the shopping cart
        int Count
        {
            get;
        }

        // Gets the total cost of items in the shopping cart before shipping and taxes
        decimal SubTotal { get; set; }

        // Gets total discount of applied to the items in the shopping cart.
        decimal Discount { get; set; }


        // Gets the total cost after shipping, taxes and promotions
        decimal OrderTotalWithoutVoucher { get; set; }


        decimal Total { get; set; }

        decimal TotalAdditionalCost { get; set; }

        // Gets all of the promotion descriptions that have been added to the cart.
        string PromoDescription { get; }

        // Gets or sets the promotion description
        string AddPromoDescription { get; set; }

        // Gets or sets a message to the shopping cart main error message.
        string AddErrorMessage { get; set; }

        // for paypal express checkout for Api
        string Token { get; set; }

        string PayerId { get; set; }

        //for setting PortalId in case of franchise admin
        int? PortalId { get; set; }

        int? UserId { get; set; }

        string LoginUserName { get; set; }

        /// <summary>
        /// To get set the ip address
        /// </summary>
        string IpAddress { get; set; }

        /// <summary>
        /// To get set the InHandDate
        /// </summary>
        DateTime? InHandDate { get; set; }

        /// <summary>
        /// To get set the Job Name
        /// </summary>
        string JobName { get; set; }

        /// <summary>
        /// To get set the Shipping Constraint Code
        /// </summary>
        string ShippingConstraintCode { get; set; }

        //depends on the first coupon applied.
        bool CartAllowsMultiCoupon { get; set; }

        int? OrderLineItemRelationshipId { get; set; }



        /// <summary>
        /// Gets or sets Znode Coupons for applied in shopping cart.
        /// </summary>
        List<ZnodeCoupon> Coupons { get; set; }

        /// <summary>
        /// Gets or sets Znode product description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Clear the previous error messages.
        /// </summary>
        void ClearPreviousErrorMessages();


        int? ProfileId { get; set; }

        // Gets or sets a OrderId of shopping cart
        int? OrderId { get; set; }

        // Gets or sets a OrderDate of shopping cart
        DateTime? OrderDate { get; set; }

        // Gets or sets the discount description applied to this line item.
        List<OrderDiscountModel> OrderLevelDiscountDetails { get; set; }

        int LocalId { get; set; }
        int PublishedCatalogId { get; set; }
        string CurrencyCode { get; set; }
        string CurrencySuffix { get; set; }
        int PublishStateId { get; set; }
        //Get and set Personalise Value list
        Dictionary<string, object> PersonaliseValuesList { get; set; }
        List<PersonaliseValueModel> PersonaliseValueDetail { get; set; }

        string ApproximateArrival { get; set; }
        string CreditCardNumber { get; set; }
        bool IsCchCalculate { get; set; }
        List<ReturnOrderLineItemModel> ReturnItemList { get; set; }
        string Custom1 { get; set; }
        string Custom2 { get; set; }
        string Custom3 { get; set; }
        string Custom4 { get; set; }
        string Custom5 { get; set; }
        string CardType { get; set; }
        int? CreditCardExpMonth { get; set; }
        int? CreditCardExpYear { get; set; }

        bool IsAllowWithOtherPromotionsAndCoupons { get; set; }
        bool IsCalculatePromotionAndCoupon { get; set; }

        string CultureCode { get; set; }

        List<ZnodeVoucher> Vouchers { get; set; }
        bool IsCalculateVoucher { get; set; }
        bool IsAllVoucherRemoved { get; set; }
        bool IsRemoveShippingDiscount { get; set; }
        List<OrderPromotionModel> InvalidOrderLevelShippingDiscount { get; set; }
        List<PromotionModel> InvalidOrderLevelShippingPromotion { get; set; }
        bool IsShippingDiscountRecalculate { get; set; }
        bool? IsCalculateTaxAfterDiscount { get; set; }
        bool? IsPricesInclusiveOfTaxes { get; set; }        
        decimal ShippingDiscount { get; set; }
        decimal ShippingHandlingCharges { get; set; }
        bool IsShippingRecalculate { get; set; }
        decimal ReturnCharges { get; set; }
        bool IsQuoteOrder { get; set; }
        /// <summary>
        /// DiscountCode and Discount Applied sequence
        /// </summary>
        Dictionary<string, int> DiscountAppliedSequence { get; set; }

        /// <summary>
        /// DiscountCode and Discount Product Quantity
        /// </summary>
        Dictionary<string, decimal> DiscountedProductQuantity { get; set; }
        bool IsOldOrder { get; set; }
        //Use to decide call live shipping method or not
        bool IsCallLiveShipping { get; set; }

        bool IsFromAdminOrder { get; set; }
        //To check is quote to order
        bool IsQuoteToOrder { get; set; }

        // Calculate and distribute CSR discount on line item.
        void CalculateLineItemCSRDiscount();


        // Return Shipping discount multiplier.
        decimal GetShippingDiscountMultiplier(decimal totalShippingDiscount);

        // Return Max applicable subtotal
        decimal GetMaximumApplicableDiscountOnSubtotal();
        // Return Order/CSR discount multiplier.
        decimal GetDiscountMultiplier(decimal discountAmount);


        // Return Max applicable line item value after applying discount.
        decimal GetMaximumApplicableDiscountOnLineItem(decimal discountAmount, decimal quantity, ZnodeProductBaseEntity product, ZnodeProductBaseEntity childProduct = null, string discountCode = "", bool isConfigurableProduct = false);



        // Return discount price of line item.
        decimal GetDiscountedPrice(ZnodeProductBaseEntity product, decimal quantity, ZnodeProductBaseEntity childproduct = null, string discountCode = "", bool isConfigurableProduct = false);

        // Split Order and CSR discount on line Items.
        decimal DistributeOrderDiscountAmount(OrderDiscountModel orderDiscountModel);
        int GetDiscountAppliedSequence(string discountCode);

        bool IsPendingOrderRequest { get; set; }

        // Gets or sets the IsSellerImporterOfRecord
        bool? AvataxIsSellerImporterOfRecord { get; set; }
        /// <summary>
        /// The flag is used to validate order details at API level.
        /// </summary>
        bool SkipPreprocessing { get; set; }
    }
}