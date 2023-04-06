namespace Znode.Engine.Api.Models
{
    public class SearchDocumentMappingModel : BaseModel
    {
        public int SearchDocumentMappingId { get; set; }
        public int PublishCatalogId { get; set; }
        public string PropertyName { get; set; }
        public bool FieldBoostable { get; set; }
        public decimal Boost { get; set; }
    }
}
