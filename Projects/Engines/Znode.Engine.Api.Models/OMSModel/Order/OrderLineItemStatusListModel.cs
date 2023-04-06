using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class OrderLineItemStatusListModel : BaseListModel
    {
        public List<OrderLineItemStatusModel> OrderLineItemStatusList { get; set; }

        public OrderLineItemStatusListModel()
        {
            OrderLineItemStatusList = new List<OrderLineItemStatusModel>();
        }
    }
}
