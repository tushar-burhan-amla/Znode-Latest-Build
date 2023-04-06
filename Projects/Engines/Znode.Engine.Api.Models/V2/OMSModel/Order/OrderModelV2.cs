using System;

namespace Znode.Engine.Api.Models.V2
{
    public class OrderModelV2 : BaseModel
    {
        public int OmsOrderId { get; set; }
        public int PortalId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal Total { get; set; }

        public string OrderNumber { get; set; }
        public string OrderState { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentDisplayName { get; set; }        
    }
}
