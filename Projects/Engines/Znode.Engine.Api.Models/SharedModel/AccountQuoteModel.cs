using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AccountQuoteModel : BaseModel
    {
        public int OmsQuoteId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public int OmsOrderStateId { get; set; }
        public int ShippingId { get; set; }
        public int ShippingAddressId { get; set; }
        public int BillingAddressId { get; set; }
        public int ApproverUserId { get; set; }
        public int AccountId { get; set; }
        public decimal QuoteOrderTotal { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal TaxCost { get; set; }
        public string AdditionalInstruction { get; set; }
        public string UserName { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string OrderStatus { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string BillingAddressHtml { get; set; }
        public string ShippingAddressHtml { get; set; }
        public string ShippingCountryCode { get; set; }
        public string CartItemCount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
        public string ShippingName { get; set; }
        public int PublishCatalogId { get; set; }
        public int LocaleId { get; set; }
        public bool IsQuoteLineItemUpdated { get; set; }
        public List<AccountQuoteLineItemModel> AccountQuoteLineItemList { get; set; }
        public List<OrderNotesModel> OrderNotes { get; set; }
        public AddressModel BillingAddressModel { get; set; }
        public AddressModel ShippingAddressModel { get; set; }
        public ShoppingCartModel ShoppingCart { get; set; }
        public string StoreName { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
        public bool IsLevelApprovedOrRejected { get; set; }
        public string ChildOrderStatus { get; set; }
        public string BillingAccountNumber { get; set; }
        public bool IsConvertedToOrder { get; set; }
        public string OrderNumber { get; set; }
        public bool IsUpdated { get; set; }
        public string Comments { get; set; }
        public List<QuoteApprovalModel> QuoteApproverComments { get; set; }
        public string PaymentDisplayName { get; set; }
        public int? PaymentSettingId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PODocumentName { get; set; }
        public string PublishState { get; set; }
        public Nullable<decimal> ShippingCost { get; set; }
        public int OmsOrderId { get; set; }
        public string OrderType { get; set; }
        public string AccountCode { get; set; }
        public string ShippingTypeClassName { get; set; }
        public string JobName { get; set; }
        public DateTime? InHandDate { get; set; }
        public string ShippingConstraintCode { get; set; }
        public string ShippingTypeDescription { get; set; }
        public string ShippingMethod { get; set; }
        public string AccountNumber { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ShippingDiscount { get; set; }
        public decimal? ShippingHandlingCharges { get; set; }
        public string QuoteNumber { get; set; }
        public decimal? ImportDuty { get; set; }
        public string QuoteTypeCode { get; set; }
    }
}
