using System;

namespace Znode.Engine.WebStore.ViewModels
{
    public class SubmitOrderViewModel : SubmitPaymentViewModel
    {
        public int UserId { get; set; }
        public int ShippingAddressId { get; set; }
        public int BillingAddressId { get; set; }
        public int PaymentSettingId { get; set; }
        public int ShippingOptionId { get; set; }
        public string ShippingOptionCode { get; set; }
        public string AdditionalInstruction { get; set; }
        public string PayPalToken { get; internal set; }
        public bool IsFromPayPalExpress { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PODocumentName { get; set; }
        public string CreditCardNumber { get; set; }
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public bool IsFromAmazonPay { get; set; }
        public string CardType { get; set; }
        public int? CreditCardExpMonth { get; set; }
        public int? CreditCardExpYear { get; set; }
        public bool IsSendForApproval { get; set; }
        public string CardDetails { get; set; }
        public string OrderNumber { get; set; }
        public string AccessToken { get; set; }
        public string IpAddress { get; set; }
        public DateTime? InHandDate { get; set; }
        public string JobName { get; set; }
        public string ShippingConstraintCode { get; set; }
        public bool IsACHPayment { get; set; }
        public string CyberSourceToken { get; set; }
        public bool IsOrderFromAdmin { get; set; }

    }
}