using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class OrderOldValueModel : BaseModel
    {
        public string OrderStatus { get; set; }
        public string ShippingCarrierMethodName { get; set; }
        public string ShippingAmount { get; set; }
        public List<OrderLineItemModel> OrderLineItems { get; set; }
        public string OrderState { get; set; }
        public int ShippingId { get; set; }

        public OrderOldValueModel()
        {
            OrderLineItems = new List<OrderLineItemModel>();
        }
    }
}