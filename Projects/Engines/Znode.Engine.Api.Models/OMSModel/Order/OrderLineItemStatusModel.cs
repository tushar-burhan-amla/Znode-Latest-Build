using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class OrderLineItemStatusModel : BaseModel
    {
        public List<int> FailedOrderLineItemIds { get; set; }
        public int OrderLineItemsId { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}