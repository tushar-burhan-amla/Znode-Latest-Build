
namespace Znode.Engine.Admin.ViewModels
{
    public class ExportPriceViewModel
    {
        public string SKU { get; set; }
        public string RetailPrice { get; set; }
        public string SalesPrice { get; set; }
        public string SKUActivationDate { get; set; }
        public string SKUExpirationDate { get; set; }
        public string TierStartQuantity { get; set; }
        public string TierPrice { get; set; }
    }
}