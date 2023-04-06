
namespace Znode.Engine.Admin.ViewModels
{
    public class ExportInventoryViewModel
    {
        public string SKU { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReOrderLevel { get; set; }
        public string ExternalId { get; set; }
    }
}