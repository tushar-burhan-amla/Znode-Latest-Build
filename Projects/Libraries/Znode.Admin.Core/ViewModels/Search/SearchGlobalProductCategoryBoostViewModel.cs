namespace Znode.Engine.Admin.ViewModels
{
    public class SearchGlobalProductCategoryBoostViewModel : BaseViewModel
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        public string PublishCategoryName { get; set; }
        public int PublishCatalogId { get; set; }
        public int PublishCategoryProductId { get; set; }
        public int PublishCategoryId { get; set; }
        public int PublishProductId { get; set; }
        public decimal Boost { get; set; }
        public string SKU { get; set; }
    }
}