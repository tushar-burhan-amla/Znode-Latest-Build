using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class OrderModel : BaseModel
    {
        public int OmsOrderDetailsId { get; set; }
        public int OmsOrderId { get; set; }
        public int PortalId { get; set; }
        public int PortalCatalogId { get; set; }
        public int UserId { get; set; }
        public int? ItemCount { get; set; }
        public int AddressId { get; set; }
        public int OmsOrderStateId { get; set; }
        public int ShippingId { get; set; }
        public int? PaymentTypeId { get; set; }
        public int? PaymentSettingId { get; set; }
        public int? ReferralUserId { get; set; }
        public int? OmsPaymentStateId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime OrderModifiedDate { get; set; }
        public string BillingPostalCode { get; set; }
        public string ShippingPostalCode { get; set; }
        public AddressModel BillingAddress { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public string AdditionalInstructions { get; set; }
        public string TrackingNumber { get; set; }
        public string CouponCode { get; set; }
        public string PromoDescription { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PoDocument { get; set; }
        public string OrderNumber { get; set; }
        public string ExternalId { get; set; }
        public string PaymentTransactionToken { get; set; }
        public string ReceiptHtml { get; set; }
        public string KeyReceiptHtml { get; set; }
        public string StoreName { get; set; }
        public string OrderState { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }
        public string PaymentDisplayName { get; set; }
        public string PaymentExternalId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CurrencyCode { get; set; }
        public string ShippingTypeName { get; set; }
        public string TrackingUrl { get; set; }
        public string BillingAddressHtml { get; set; }
        public string OrderItem { get; set; }
        public string ShippingNumber { get; set; }
        public string PODocumentPath { get; set; }
        public string CustomerPaymentGUID { get; set; }
        public decimal TaxCost { get; set; }
        public decimal? RemainingOrderAmount { get; set; }
        public decimal ReturnImportDuty { get; set; }      
        public decimal ShippingCost { get; set; }
        public decimal ShippingDifference { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal CSRDiscountAmount { get; set; }
        public decimal GiftCardAmount { get; set; }
        public decimal Total { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal OverDueAmount { get; set; }
        public decimal SalesTax { get; set; }
        public decimal VAT { get; set; }
        public decimal GST { get; set; }
        public decimal PST { get; set; }
        public decimal HST { get; set; }
        public decimal ImportDuty { get; set; }
        public decimal? LineItemReturnAmount { get; set; }

        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime OrderDate { get; set; }
        public DateTime? WebServiceDownloadDate { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsQuoteOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsMultipleShipping { get; set; }
        public bool IsEmailSend { get; set; }
        public bool IsAvailabelInventoryAndMinMaxQuantity { get; set; }
        public bool IsLineItemShipped { get; set; }
        public bool IsShippingCostEdited { get; set; }
        public bool IsTaxCostEdited { get; set; }
        public bool IsLineItemReturned { get; set; }
        public bool IsValidForRma { get; set; }
        public bool IsEmailNotificationForRma { get; set; }
        public List<OrderLineItemModel> OrderLineItems { get; set; }
        public List<OrderLineItemModel> ReturnedOrderLineItems { get; set; }
        public List<OrderDiscountModel> OrdersDiscount { get; set; }
        public List<OrderNotesModel> OrderNotes { get; set; }
        public Dictionary<string, string> OrderHistory { get; set; }
        public Dictionary<string, OrderLineItemHistoryModel> OrderLineItemHistory { get; set; }
        public ShoppingCartModel ShoppingCartModel { get; set; }
        public List<OrderPaymentStateModel> OrderPaymentStateModelList { get; set; }
        public string UpdatePageType { get; set; }
        public int AccountId { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public bool? IsInRMA { get; set; }
        public string OrderTotalWithCurrency { get; set; }
        public string OrderDateWithTime { get; set; }
        public string CreditCardNumber { get; set; }
        public decimal ReturnTaxCost { get; set; }
        public decimal ReturnShippingCost { get; set; }
        public decimal ReturnSubTotal { get; set; }
        public decimal ReturnTotal { get; set; }
        public PortalTrackingPixelModel PortalTrackingPixel { get; set; }
        public OrderHistoryListModel OrderHistoryList { get; set; }
        public ReturnOrderLineItemListModel ReturnItemList { get; set; }
        public OrderOldValueModel OrderOldValue { get; set; }

        public decimal? EstimateShippingCost { get; set; }

        //This property tracks the line items whose status has been changed
        public string ModifiedLineItemSkus { get; set; }


        public OrderModel()
        {
            BillingAddress = new AddressModel();
            ShippingAddress = new AddressModel();
            OrderHistory = new Dictionary<string, string>();
            OrderLineItemHistory = new Dictionary<string, OrderLineItemHistoryModel>();
            OrderHistoryList = new OrderHistoryListModel();
            OrderOldValue = new OrderOldValueModel();
            PaymentHistoryList = new PaymentHistoryListModel();
        }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        public string ShippingTypeClassName { get; set; }
        public string CustomerServiceEmail { get; set; }
        public string CustomerServicePhoneNumber { get; set; }
        public string ShippingCode { get; set; }
        public string CardType { get; set; }
        public int? CreditCardExpMonth { get; set; }
        public int? CreditCardExpYear { get; set; }
        public string TransactionId { get; set; }
        public decimal? TotalAdditionalCost { get; set; }
        public int OmsQuoteId { get; set; }
        public List<QuoteApprovalModel> QuoteApproverComments { get; set; }

        public string CultureCode { get; set; }
        public string PublishState { get; set; }
        public int PublishStateId { get; set; }
        public string AVSCode { get; set; }
        public string CcExpiration { get; set; }

        public bool IsFromAdminOrder { get; set; }
        public bool IsFromManageOrder { get; set; }
        public string IpAddress { get; set; }
        public DateTime? InHandDate { get; set; }
        [MaxLength(100, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorMessageMaxLength)]
        public string JobName { get; set; }
        [MaxLength(50, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorMessageMaxLength)]
        public string ShippingConstraintCode { get; set; }
        public string GiftCardNumber { get; set; }
        public string PayPalExpressResponseText { get; set; }
        public bool IsAnyPendingReturn { get; set; }
        public Nullable<decimal> ShippingDiscount { get; set; }
        public Nullable<decimal> ShippingHandlingCharges { get; set; }
        public Nullable<decimal> ReturnCharges { get; set; }
        public bool? IsCalculateTaxAfterDiscount { get; set; }
        public bool? IsPricesInclusiveOfTaxes { get; set; }        
        public bool IsAnOldOrder { get; set; }
        public decimal OrderTotalWithoutVoucher { get; set; }
        public decimal ReturnVoucherAmount { get; set; }
        public bool IsOldOrder { get; set; }
        public List<TaxSummaryModel> TaxSummaryList { get; set; }
        public bool IsFromReturnedReceipt { get; set; }
        public PaymentHistoryListModel PaymentHistoryList { get; set; }   //Added new property to save payment history list
        public bool? AvataxIsSellerImporterOfRecord { get; set; }
        public bool IsTradeCentricUser { get; set; }
    }
}
