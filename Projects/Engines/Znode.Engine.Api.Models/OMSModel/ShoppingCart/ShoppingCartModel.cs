using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class ShoppingCartModel : BaseModel
    {
        public int OmsOrderStatusId { get; set; }
        public int ShippingAddressId { get; set; }
        public string ExternalId { get; set; }
        public int BillingAddressId { get; set; }
        public int PortalId { get; set; }
        public int LocaleId { get; set; }
        public int ShippingId { get; set; }
        public int PublishedCatalogId { get; set; }
        public int OmsOrderDetailsId { get; set; }
        public int OmsQuoteId { get; set; }
        public int? OmsOrderId { get; set; }
        public int? UserId { get; set; }
        public int? ProfileId { get; set; }
        public int SelectedAccountUserId { get; set; }
        public string OrderAttribute { get; set; }
        public string AdditionalNotes { get; set; }
        public string AdditionalInstructions { get; set; }
        public string BillingEmail { get; set; }
        public string CookieMappingId { get; set; }
        public string FeedbackUrl { get; set; }
        public string GiftCardMessage { get; set; }
        public string GiftCardNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PODocumentName { get; set; }
        public string Token { get; set; }
        public string PayerId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySuffix { get; set; }
        public string CSRDiscountDescription { get; set; }
        public string CSRDiscountMessage { get; set; }
        public string OrderStatus { get; set; }
        public decimal Discount { get; set; }
        public decimal GiftCardAmount { get; set; }
        public decimal GiftCardBalance { get; set; }
        public decimal OrderLevelDiscount { get; set; }
        public decimal OrderLevelShipping { get; set; }
        public decimal OrderLevelTaxes { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal ShippingDifference { get; set; }
        public decimal TaxCost { get; set; }
        public decimal TaxRate { get; set; }
        public decimal SalesTax { get; set; }
        public decimal CSRDiscountAmount { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public decimal? OverDueAmount { get; set; }
        public decimal? Vat { get; set; }
        public decimal? ImportDuty { get; set; }
        public decimal? Gst { get; set; }
        public decimal? Hst { get; set; }
        public decimal? Pst { get; set; }
        public decimal? CustomShippingCost { get; set; }
        public decimal? CustomTaxCost { get; set; }
        public bool IsQuoteOrder { get; set; }
        public bool GiftCardApplied { get; set; }      
        public bool GiftCardValid { get; set; }
        public bool MultipleShipToEnabled { set; get; }
        public bool CSRDiscountApplied { get; set; }
        public bool FreeShipping { get; set; }
        public bool IsGatewayPreAuthorize { get; set; }
        public string CreditCardNumber { get; set; }
        public string CardType { get; set; }
        public string CcCardExpiration { get; set; }
        public string TransactionId { get; set; }
        public string RemoveAutoAddonSKU { get; set; }
        public bool IsParentAutoAddonRemoved { get; set; }
        public bool IsLineItemReturned { get; set; }
        public DateTime? OrderDate { get; set; }
        public UserModel UserDetails { get; set; }
        public PaymentModel Payment { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public AddressModel BillingAddress { get; set; }
        public OrderShippingModel Shipping { get; set; }
        public List<ShoppingCartItemModel> ShoppingCartItems { get; set; }
        public List<OrderShipmentDataModel> OrderShipment { get; set; }
        public List<CouponModel> Coupons { get; set; }
        public bool IsCchCalculate { get; set; }
        public List<ReturnOrderLineItemModel> ReturnItemList { get; set; }
        public bool IsEmailSend { get; set; }
        public bool IsAllowWithOtherPromotionsAndCoupons { get; set; }

        public bool IsLevelApprovedOrRejected { get; set; }
        public bool IsLastApprover { get; set; }

        //This Property is Used if Same User Added Product from different browsers then to maintain the cart items.
        public bool IsMerged { get; set; }

        public bool IsSplitCart { get; set; }
        public decimal? EstimateShippingCost { get; set; }
        public int? CreditCardExpMonth { get; set; }
        public int? CreditCardExpYear { get; set; }
        public int? QuotePaymentSettingId { get; set; }
        public bool IsPendingPayment { get; set; }
        public string QuoteTypeCode { get; set; }

        public decimal? TotalAdditionalCost { get; set; }
        public string CustomerServiceEmail { get; set; }
        public string OrderNumber { get; set; }
        public bool IsCalculateTaxAndShipping { get; set; } = true;
        public bool IsCalculatePromotionAndCoupon { get; set; } = true;

        public ShoppingCartModel()
        {
            ShoppingCartItems = new List<ShoppingCartItemModel>();
            Coupons = new List<CouponModel>();
            Shipping = new OrderShippingModel();
            Vouchers = new List<VoucherModel>();
            InvalidOrderLevelShippingDiscount = new List<OrderPromotionModel>();
            InvalidOrderLevelShippingPromotion = new List<PromotionModel>();
        }
        public string CultureCode { get; set; }
        public int PublishStateId { get; set; }
        public int PortalPaymentGroupId { get; set; }
        public bool IsTaxCostEdited { get; set; }

        public bool IsFromAdminOrder { get; set; }
        public bool IsFromManageOrder { get; set; }
        public bool IsOrderFromWebstore { get; set; }
        public string IpAddress { get; set; }
        public DateTime? InHandDate { get; set; }
        [MaxLength(100, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorMessageMaxLength)]
        public string JobName { get; set; }
        [MaxLength(50, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorMessageMaxLength)]
        public string ShippingConstraintCode { get; set; }
        public List<VoucherModel> Vouchers { get; set; }
        public bool IsCalculateVoucher { get; set; }
        public bool IsAllVoucherRemoved { get; set; }
        public bool IsShippingBasedCoupon { get; set; }
        public decimal ReturnCSRDiscount { get; set; }
        public decimal ShippingDiscount { get; set; }
        public decimal ShippingHandlingCharges { get; set; }
        public bool IsShippingRecalculate { get; set; }
        public decimal ReturnCharges { get; set; }
        public bool IsRemoveShippingDiscount { get; set; }
        public List<OrderPromotionModel> InvalidOrderLevelShippingDiscount { get; set; }
        public List<PromotionModel> InvalidOrderLevelShippingPromotion { get; set; }
        public bool IsShippingDiscountRecalculate { get; set; }
        public bool? IsCalculateTaxAfterDiscount { get; set; }
        public bool? IsPricesInclusiveOfTaxes { get; set; }
        public decimal? OrderTotalWithoutVoucher { get; set; }
        public bool IsOldOrder { get; set; }
        public bool IsCallLiveShipping { get; set; }
        public bool SkipCalculations { get; set; }
        public List<OrderDiscountModel> OrderLevelDiscountDetails { get; set; }
        public bool SkipShippingCalculations { get; set; }
        public List<ShippingOptionModel> ShippingOptions { get; set; }
        public virtual bool IsQuoteToOrder { get; set; }
        // Gets or sets the CSR Discount Edited.
        public bool CSRDiscountEdited { get; set; }
        public List<TaxSummaryModel> TaxSummaryList { get; set; }
        public List<string> TaxMessageList { get; set; }
        public bool IsPendingOrderRequest { get; set; }

        //This property is used to skip pre-order submit process/validations
        public bool SkipPreprocessing { get; set; }

        public bool IsTaxExempt { get; set; }
        public DateTime? TransactionDate { get; set; }
        public bool? AvataxIsSellerImporterOfRecord { get; set; }

        // To save and get the data against AccountId for user account association
        public int? QuotesAccountId { get; set; }
        public string PortalName { get; set; }
    }
}
