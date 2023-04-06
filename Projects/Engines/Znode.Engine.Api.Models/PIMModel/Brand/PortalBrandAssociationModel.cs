namespace Znode.Engine.Api.Models
{
   public class PortalBrandAssociationModel : BaseModel
    {
        public int PortalId { get; set; }
        public string BrandIds { get; set; }
        public bool IsAssociated { get; set; }
    }
}
