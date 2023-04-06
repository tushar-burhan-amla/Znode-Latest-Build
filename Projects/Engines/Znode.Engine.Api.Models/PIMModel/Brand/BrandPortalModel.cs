namespace Znode.Engine.Api.Models
{
    public class BrandPortalModel : BaseModel
    {
        public string PortalIds { get; set; }
        public int BrandId { get; set; }
        public bool IsUnAssociated { get; set; }
    }
}

