namespace Znode.Engine.Api.Models
{
    public class PaymentGatewayModel
    {
        public int PaymentGatewayId { get; set; }
        public string GatewayCode { get; set; }
        public string GatewayName { get; set; }
        public string WebsiteURL { get; set; }
        public string ClassName { get; set; }
    }
}
