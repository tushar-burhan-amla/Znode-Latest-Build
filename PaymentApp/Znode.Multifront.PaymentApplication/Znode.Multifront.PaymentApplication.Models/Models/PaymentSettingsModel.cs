namespace Znode.Multifront.PaymentApplication.Models
{
    public class PaymentSettingsModel : BaseModel
    {
        public int PaymentSettingId { get; set; }
        public int PaymentTypeId { get; set; }
        public int DisplayOrder { get; set; }

        public int? ProfileID { get; set; }
        public int? PaymentGatewayId { get; set; }

        public bool IsActive { get; set; }
        public bool PreAuthorize { get; set; }
        public bool TestMode { get; set; }
        public bool EnablePODocUpload { get; set; }
        public bool IsPODocRequired { get; set; }

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
        public string PaymentCode { get; set; }
        public string GatewayCode { get; set; }
        public string PaymentTypeCode { get; set; }

        public string CustomerGUID { get; set; }
        
    }
}
