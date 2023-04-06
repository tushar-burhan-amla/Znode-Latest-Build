namespace Znode.Engine.Api.Models
{
    public class TaxClassPortalModel : BaseModel
    {
        public int TaxClassPortalId { get; set; }
        public int TaxClassId { get; set; }
        public int PortalId { get; set; }
        public string StoreName { get; set; }
        public string TaxClassIds { get; set; }
        public bool IsUnAssociated { get; set; }
    }
}
