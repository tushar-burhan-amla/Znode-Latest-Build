namespace Znode.Engine.WebStore.ViewModels
{
    public class StockNotificationViewModel : BaseViewModel
    {
        public string ParentSKU { get; set; }
        public string SKU { get; set; }
        public string EmailId { get; set; }
        public decimal Quantity { get; set; }
        public int PortalID { get; set; }
        public int CatalogId { get; set; }
    }
}
