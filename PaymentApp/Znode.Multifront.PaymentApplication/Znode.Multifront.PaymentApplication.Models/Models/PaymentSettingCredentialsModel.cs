namespace Znode.Multifront.PaymentApplication.Models
{
    public class PaymentSettingCredentialsModel : BaseModel
    {
        public int PaymentSettingCredentialId { get; set; }
        public int? PaymentSettingID { get; set; }

        public string Partner { get; set; }
        public string Vendor { get; set; }
        public string GatewayUsername { get; set; }
        public string GatewayPassword { get; set; }
        public string TransactionKey { get; set; }

        public bool TestMode { get; set; }

    }
}
