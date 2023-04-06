using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PaymentGatewayListModel 
    {
        public PaymentGatewayListModel()
        {
            PaymentGateways = new List<PaymentGatewayModel>();
        }
        public List<PaymentGatewayModel> PaymentGateways { get; set; }
    }
}
