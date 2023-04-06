namespace Znode.Engine.Api.Models
{
    public class PaymentSettingValidationModel : BaseModel
    {
        public int PaymentSettingId { get; set; }
        public string PaymentDisplayName { get; set; }
    }
}
