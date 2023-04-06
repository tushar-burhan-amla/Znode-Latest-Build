using System.Collections.Generic;

namespace Znode.Multifront.PaymentApplication.Models
{
    public class PaymentGatewayModel : BaseModel
    {
        public int PaymentGatewayId { get; set; }
        public string GatewayCode { get; set; }
        public string GatewayName { get; set; }
        public string WebsiteURL { get; set; }
        public string ClassName { get; set; }
    }

    public class PaymentGatewayListModel
    {
        public List<PaymentGatewayModel> PaymentGateways { get; set; }          
    }
}
