namespace Znode.Engine.Api.Models
{
    public class PaymentSettingModel : BaseModel
    {
        public int PaymentSettingId { get; set; }
        public int PaymentApplicationSettingId { get; set; }
        public int PaymentTypeId { get; set; }
        public int DisplayOrder { get; set; }
        public int? ProfileId { get; set; }
        public int? PaymentGatewayId { get; set; }

        public bool IsActive { get; set; }
        public bool PreAuthorize { get; set; }
        public bool TestMode { get; set; }
        public bool IsPoDocUploadEnable { get; set; }
        public bool IsPoDocRequire { get; set; }
        public bool IsBillingAddressOptional { get; set; }
        public bool IsUsedForOfflinePayment { get; set; }
        
        public bool? EnableVisa { get; set; }
        public bool? EnableMasterCard { get; set; }
        public bool? EnableAmex { get; set; }
        public bool? EnableDiscover { get; set; }
        public bool? EnableRecurringPayments { get; set; }
        public bool? EnableVault { get; set; }
        public bool? IsRMACompatible { get; set; }

        public string Partner { get; set; }
        public string Vendor { get; set; }
        public string GatewayUsername { get; set; }
        public string GatewayPassword { get; set; }
        public string TransactionKey { get; set; }
        public string GatewayName { get; set; }
        public string PaymentTypeName { get; set; }
        public string PaymentName { get; set; }
        public string PaymentDisplayName { get; set; }
        public string PaymentExternalId { get; set; }
        public bool IsCaptureDisable { get; set; }
        public bool IsApprovalRequired { get; set; }
        public string PaymentCode { get; set; }
        public string GatewayCode { get; set; }
        public bool IsCallToPaymentAPI { get; set; }
        public string PaymentTypeCode { get; set; }
        public bool IsOABRequired { get; set; }
        public int? PortalPaymentGroupId { get; set; }
        public string PublishState { get; set; }
    }
}
