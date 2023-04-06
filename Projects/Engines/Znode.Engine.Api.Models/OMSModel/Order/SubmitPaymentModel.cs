using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SubmitPaymentModel : BaseModel
    {
        public SubmitPaymentModel()
        {
            CartItems = new List<CartItemModel>();
        }
        public int PaymentSettingId { get; set; }
        public int PaymentApplicationSettingId { get; set; }

        public int PaymentStatusId { get; set; }
        public string BillingCity { get; set; }
        public string BillingCountryCode { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public string BillingName { get; set; }
        public string BillingPhoneNumber { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingStateCode { get; set; }
        public string BillingStreetAddress1 { get; set; }
        public string BillingStreetAddress2 { get; set; }
        public string BillingEmailId { get; set; }
        public string CardDataToken { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string CardExpirationYear { get; set; }
        public string CardExpirationMonth { get; set; }
        public string CardSecurityCode { get; set; }
        public string CustomerPaymentProfileId { get; set; }
        public string CustomerProfileId { get; set; }
        public string GiftCardAmount { get; set; }
        public string CardExpiration
        {
            get { return CardExpirationMonth + "/" + CardExpirationYear; }
        }

        public string ShippingCity { get; set; }
        public string ShippingCountryCode { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingPhoneNumber { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingStateCode { get; set; }
        public string ShippingStreetAddress1 { get; set; }
        public string ShippingStreetAddress2 { get; set; }
        public string CustomerIpAddress { get; set; }
        public string Discount { get; set; }
        public string GatewayType { get; set; }
        public string GatewayLoginName { get; set; }
        public string GatewayLoginPassword { get; set; }
        public string GatewayCustom1 { get; set; }
        public string GatewayTransactionKey { get; set; }
        public string GatewayCurrencyCode { get; set; }
        public string GatewayToken { get; set; }

        public string OrderId { get; set; }

        public string ShippingCost { get; set; }
        public string ShippingHandlingCharges { get; set; }
        public string ShippingDiscount { get; set; }
        public string CSRDiscountAmount { get; set; }
        public string SubTotal { get; set; }
        public string TaxCost { get; set; }
        public string Total { get; set; }
        public string OrderTotalWithoutVoucher { get; set; }

        public string TransactionId { get; set; }
        public string RefundTransactionId { get; set; }
        public string ResponseText { get; set; }
        public string ResponseCode { get; set; }
        public string GUID { get; set; }
        public string TwoCOToken { get; set; }
        public string Vendor { get; set; }
        public string Partner { get; set; }
        public string PaymentToken { get; set; }
        public string CustomerGUID { get; set; }
        public bool IsMultipleShipAddress { get; set; }
        public bool GatewayPreAuthorize { get; set; }
        public bool GatewayTestMode { get; set; }
        public string CancelUrl { get; set; }
        public string ReturnUrl { get; set; }
        public List<CartItemModel> CartItems { get; set; }
        public string AmazonOrderReferenceId { get; set; }
        public string CompanyName { get; set; }
        public string CustomerShippingAddressId { get; set; }
        public string PaymentCode { get; set; }
        public string AccessToken { get; set; }
        public bool IsACHPayment { get; set; }
        public string CyberSourceToken { get; set; }
        public string Email { get; set; }

        public string IsSaveCreditCard { get; set; }

        public int UserId { get; set; }

        public System.Guid PaymentGUID { get; set; }
        public string GatewayCode { get; set; }

        public bool IsOrderFromAdmin { get; set; }
        public string PaymentMethodNonce { get; set; } //This field is specific for braintree PCI compliance.
        public string PortalName { get; set; }
    }
}