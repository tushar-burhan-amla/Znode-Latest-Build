using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class OrderWarehouseModel
    {
        public OrderWarehouseModel()
        {
            LineItems = new List<OrderWarehouseLineItemsModel>();
        }
        public int OrderId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }
        public List<OrderWarehouseLineItemsModel> LineItems { get; set; }
        public int WarehouseId { get; set; }
        public int OmsOrderLineItemsId { get; set; }
        public string WarehouseName { get; set; }
    }
}
