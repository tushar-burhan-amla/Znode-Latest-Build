namespace Znode.Engine.Api.Models
{
    public class InventoryWarehouseMapperModel : BaseModel
    {
        public int InventoryWarehouseId { get; set; }
        public int WarehouseId { get; set; }
        public int InventoryId { get; set; }
        public int InventoryListId { get; set; }
        public bool IsActive { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string SKU { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public string ProductName { get; set; }
    }
}
