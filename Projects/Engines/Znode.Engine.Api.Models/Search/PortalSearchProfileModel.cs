namespace Znode.Engine.Api.Models
{
    public class PortalSearchProfileModel : BaseModel
    {
        public int PortalSearchProfileId { get; set; }
        public int PublishCatalogId { get; set; }
        public int SearchProfileId { get; set; }
        public int PortalId { get; set; }
        public bool IsDefault { get; set; }
    }
}
