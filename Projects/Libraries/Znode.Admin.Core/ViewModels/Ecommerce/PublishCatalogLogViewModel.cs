namespace Znode.Engine.Admin.ViewModels
{
    public class PublishCatalogLogViewModel : BaseViewModel
    {
        public int PublishCatalogLogId { get; set; }
        public int? PublishCatalogId { get; set; }
        public bool? IsCatalogPublished { get; set; }
        public string PublishStatus { get; set; }
        public string UserName { get; set; }
        public int PublishCategoryCount { get; set; }
        public int PublishProductCount { get; set; }
        public string LastPublishedDate { get; set; }
    }
}