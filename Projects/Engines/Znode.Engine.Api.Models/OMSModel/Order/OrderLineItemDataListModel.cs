using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class OrderLineItemDataListModel : BaseListModel
    {
        public OrderLineItemDataListModel()
        {
            OrderLineItemDetails = new List<OrderLineItemDataModel>();
        }

        public string OrderNumber { get; set; }

        public List<OrderLineItemDataModel> OrderLineItemDetails { get; set; }
    }
}