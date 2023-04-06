namespace Znode.Engine.Admin.ViewModels
{
    public class InventoryWarehouseMapperViewModel : BaseViewModel
    {
        public int InventoryWarehouseId { get; set; }
        public int WarehouseId { get; set; }
        public int InventoryListId { get; set; }
        public bool IsActive { get; set; }
        public string ListCode { get; set; }
        public string ListName { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseCode { get; set; }
        public string SKU { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public int InventoryId { get; set; }
        public string ProductName { get; set; }
    }
}