namespace Znode.Engine.Api.Models
{
    public class PublishedSelectValuesEntityModel : BaseModel
    {
        public int VersionId { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
        public string SwatchText { get; set; }
        public int? DisplayOrder { get; set; }
        public string Path { get; set; }
        public int? MediaConfigurationId { get; set; }
        public int VariantDisplayOrder { get; set; }
        public string VariantImagePath { get; set; }
        public string VariantSKU { get; set; }
    }
}
