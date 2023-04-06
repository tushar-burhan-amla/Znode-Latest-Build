using System;

namespace Znode.Engine.Admin.ViewModels
{
    public class SubmitPaymentViewModel : BaseViewModel
    {
        public int OmsOrderId { get; set; }
        public int PaymentSettingId { get; set; }
        public int PaymentApplicationSettingId { get; set; }
        public string CustomerProfileId { get; set; }
        public string CustomerPaymentId { get; set; }
        public string CustomerGuid { get; set; }
        public string PaymentToken { get; set; }
        public int UserId { get; set; }
        public int ShippingOptionId { get; set; }
        public int BillingAddressId { get; set; }
        public int ShippingAddressId { get; set; }
        public int PortalId { get; set; }
        public int PortalCatalogId { get; set; }
        public bool EnableAddressValidation { get; set; }
        public bool RequireValidatedAddress { get; set; }
        public string AdditionalInfo { get; set; }
        public string PayPalReturnUrl { get; set; }
        public string PayPalCancelUrl { get; set; }
        public string Token { get; set; }
        public string CreditCardNumber { get; set; }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        public string CardType { get; set; }
        public int? CreditCardExpMonth { get; set; }
        public int? CreditCardExpYear { get; set; }
        public string CardExpiration { get; set; }
        public string CardDetails { get; set; }
        public string CustomerShippingAddressId { get; set; }
        public string CardSecurityCode { get; set; }
        public string PaymentCode { get; set; }
        public DateTime? InHandDate { get; set; }
        public string JobName { get; set; }
        public string ShippingConstraintCode { get; set; }
        public string TransactionId { get; set; }
        public string CyberSourceToken { get; set; }

        public string IsSaveCreditCard { get; set; }
        public string CardHolderName { get; set; }

        public System.Guid PaymentGUID { get; set; }

        public string GatewayCode { get; set; }
        public string OrderNumber { get; set; }
        public bool IsAdminRequestUrl { get; set; }

        public bool IsACHPayment { get; set; }

        public bool IsOrderFromAdmin { get; set; }
        public string PaymentMethodNonce { get; set; } //This field is specific for braintree PCI compliance.
    }
}