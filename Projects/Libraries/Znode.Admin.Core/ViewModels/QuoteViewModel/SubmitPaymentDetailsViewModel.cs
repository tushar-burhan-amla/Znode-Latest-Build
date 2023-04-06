using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SubmitPaymentDetailsViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorPaymentSettingIdRequired)]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorPaymentSettingIdRequired)]
        public int PaymentSettingId { get; set; }
        public string CustomerProfileId { get; set; }
        public string CustomerPaymentId { get; set; }
        public string CustomerGuid { get; set; }
        public string PaymentToken { get; set; }
        public string PaymentCode { get; set; }
        public string CustomerShippingAddressId { get; set; }
        public string PayPalReturnUrl { get; set; }
        public string PayPalCancelUrl { get; set; }
        public string PaymentType { get; set; }
        public string AmazonPayReturnUrl { get; set; }
        public string AmazonPayCancelUrl { get; set; }
        public string AmazonOrderReferenceId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PODocumentName { get; set; }
        public string CreditCardNumber { get; set; }
        public string CyberSourceToken { get; set; }
        public string Email { get; set; }

        public string IsSaveCreditCard { get; set; }
        public string GatewayCode { get; set; }
        public string TransactionId { get; set; }

        public string CardHolderName { get; set; }

        public System.Guid PaymentGUID { get; set; }
        public string OrderId { get; set; }
    }
}
