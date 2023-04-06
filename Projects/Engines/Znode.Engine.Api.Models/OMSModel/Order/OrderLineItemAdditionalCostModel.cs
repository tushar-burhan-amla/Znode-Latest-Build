using System;

namespace Znode.Engine.Api.Models
{
    public class OrderLineItemAdditionalCostModel : BaseModel
    {
        public int OmsOrderLineItemsAdditionalCostId { get; set; }
        public Nullable<int> OmsOrderLineItemsId { get; set; }
        public string KeyName { get; set; }
        public decimal KeyValue { get; set; }
    }
}
