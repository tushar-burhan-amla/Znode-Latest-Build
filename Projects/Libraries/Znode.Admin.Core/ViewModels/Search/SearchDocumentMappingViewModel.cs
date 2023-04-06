namespace Znode.Engine.Admin.ViewModels
{
    public class SearchDocumentMappingViewModel : BaseViewModel
    {
        public int ID { get; set; }
        public int BoostableItemId { get; set; }
        public int PublishCatalogId { get; set; }
        public string PropertyName { get; set; }
        public bool FieldBoostable { get; set; }
        public decimal Boost { get; set; }
    }
}