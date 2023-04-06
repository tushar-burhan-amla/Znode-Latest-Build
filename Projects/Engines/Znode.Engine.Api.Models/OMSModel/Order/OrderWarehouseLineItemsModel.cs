using System;

namespace Znode.Engine.Api.Models
{
    public class OrderWarehouseLineItemsModel
    {
        public int OrderLineItemId { get; set; }
        public string SKU { get; set; }
        public string InventoryTracking { get; set; }
        public bool AllowBackOrder { get; set; }
        public decimal Quantity { get; set; }
        public Nullable<int> WarehouseId { get; set; }
    }
}
