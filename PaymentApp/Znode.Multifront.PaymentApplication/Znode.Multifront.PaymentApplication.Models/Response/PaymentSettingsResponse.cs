namespace Znode.Multifront.PaymentApplication.Models
{
    public class PaymentSettingsResponse : BaseResponse
    {
        public PaymentSettingsModel PaymentSetting { get; set; }

        public PaymentSettingCredentialsModel PaymentSettingCredentials { get; set; }

        public PaymentSettingListModel PaymentSettings { get; set; }
    }
}
