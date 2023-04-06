using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PlaceOrderModel : BaseModel
    {
        public int OmsOrderId { get; set; }
        public int OmsOrderStateId { get; set; }
        public decimal DiscountAmount { get; set; }
        public string OrderNumber { get; set; }
        public string PaymentCode { get; set; }
        public int PublishStateId { get; set; }
        public int? PaymentTypeId { get; set; }
        public int PaymentStatusId { get; set; }
        public int PortalId { get; set; }
        public decimal SalesTax { get; set; }
        public decimal TaxRate { get; set; }
        public bool IsOldOrder { get; set; }
        public int? ShippingTypeId { get; set; }
        public decimal VAT { get; set; }
        public decimal GST { get; set; }
        public decimal PST { get; set; }
        public decimal HST { get; set; }
        public decimal Total { get; set; }
        public int BillingAddressId { get; set; }
        public int ShippingAddressId { get; set; }
        public decimal TaxCost { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal SubTotal { get; set; }
        public decimal OverDueAmount { get; set; }
        public string ShippingNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string CouponCode { get; set; }
        public string PromoDescription { get; set; }
        public int? ReferralUserId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public int? OmsPaymentStateId { get; set; }
        public int? PaymentSettingId { get; set; }
        public string PaymentTransactionToken { get; set; }
        public DateTime? ShipDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string PoDocument { get; set; }
        public string ExternalId { get; set; }
        public string CreditCardNumber { get; set; }
        public bool IsShippingCostEdited { get; set; }
        public bool IsTaxCostEdited { get; set; }
        public decimal ShippingDifference { get; set; }
        public decimal? EstimateShippingCost { get; set; }
        public string TransactionId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardType { get; set; }
        public int? CreditCardExpMonth { get; set; }
        public int? CreditCardExpYear { get; set; }
        public decimal? TotalAdditionalCost { get; set; }
        public string PaymentDisplayName { get; set; }

        public string PaymentExternalId { get; set; }
        public DateTime? InHandDate { get; set; }
        public string IpAddress { get; set; }
        public string JobName { get; set; }
        public string ShippingConstraintCode { get; set; }
        public decimal? ShippingDiscount { get; set; }
        public decimal? ShippingHandlingCharges { get; set; }
        public decimal? ReturnCharges { get; set; }
        public bool? IsCalculateTaxAfterDiscount { get; set; }
        public string CultureCode { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal OrderTotalWithoutVoucher { get; set; }
        public decimal ImportDuty { get; set; }
        public string CurrencyCode { get; set; }
        public int ShippingId { get; set; }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        public string AdditionalInstructions { get; set; }
        public List<PlaceOrderDiscountModel> OrderDiscounts { get; set; }
        public List<PlaceOrderLineItemModel> LineItems { get; set; }
        public PlaceOrderAddressModel BillingAddressModel { get; set; }
        //to hold Remaining Order Amount while doing partial payment
        public decimal RemainingOrderAmount { get; set; }

        // To save and get the data against AccountId
        public int? AccountId { get; set; }
     }
}
