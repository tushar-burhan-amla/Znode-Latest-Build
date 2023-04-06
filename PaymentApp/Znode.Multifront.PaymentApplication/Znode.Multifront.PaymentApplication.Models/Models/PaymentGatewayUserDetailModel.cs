namespace Znode.Multifront.PaymentApplication.Models
{
    public class PaymentGatewayUserDetailModel : BaseModel
    {
        public string user_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }
}
