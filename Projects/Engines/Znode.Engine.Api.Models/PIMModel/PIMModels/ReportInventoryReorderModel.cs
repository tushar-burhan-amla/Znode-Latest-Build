namespace Znode.Engine.Api.Models
{
    public class ReportInventoryReorderModel
    {
        public string SKU { get; set; }
        public string Quantity { get; set; }
        public string ProductName { get; set; }
        public string ReOrderLevel { get; set; }
        public string WarehouseName { get; set; }
        public string StoreName { get; set; }
        public string ProductStatus { get; set; }
        public string UnitOfMeasurement { get; set; }
    }
}
