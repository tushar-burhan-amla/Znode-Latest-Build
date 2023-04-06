namespace Znode.Engine.Api.Models
{
    public class CatalogEntityModel : BaseModel
    {
        public int ZnodeCatalogId { get; set; }
        public string CatalogName { get; set; }
        public string RevisionType { get; set; }
        public int LocaleId { get; set; }
        public int VersionId { get; set; }
    }
}
