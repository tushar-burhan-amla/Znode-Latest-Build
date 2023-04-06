namespace Znode.Engine.Api.Models.Responses
{
    public class PaymentSettingResponse : BaseResponse
    {
        public PaymentSettingModel PaymentSetting { get; set; }

        public PaymentSettingCredentialModel PaymentSettingCredentials { get; set; }
    }
}
