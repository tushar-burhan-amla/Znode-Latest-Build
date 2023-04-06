namespace Znode.Engine.WebStore.ViewModels
{
    public class InventorySKUViewModel : BaseViewModel
    {
        public string SKU { get; set; }

        public decimal Quantity { get; set; }

        public decimal? ReOrderLevel { get; set; }
        public int InventoryId { get; set; }
        public int? WarehouseId { get; set; }
        public string ListName { get; set; }
        public string WarehouseName { get; set; }
        public string WarehouseCode { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public string ExternalId { get; set; }
        public bool IsDefaultWarehouse { get; set; }
        public decimal? AllLocationQuantity { get; set; }
        public string WarehouseCityName { get; set; }
        public string WarehouseStateName { get; set; }
        public string WarehousePostalCode { get; set; }
        public string WarehouseAddress { get; set; }
    }
}
