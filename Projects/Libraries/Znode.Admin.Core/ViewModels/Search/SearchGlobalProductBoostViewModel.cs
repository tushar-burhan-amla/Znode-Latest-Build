namespace Znode.Engine.Admin.ViewModels
{
    public class SearchGlobalProductBoostViewModel : BaseViewModel
    {
        public int ID { get; set; }
        public int PublishCatalogId { get; set; }
        public int PublishProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Boost { get; set; }
        public string SKU { get; set; }
    }
}