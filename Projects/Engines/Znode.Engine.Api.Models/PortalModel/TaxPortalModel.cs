namespace Znode.Engine.Api.Models
{
    public class TaxPortalModel : BaseModel
    {
        public int TaxPortalId { get; set; }
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public string AvataxUrl { get; set; }
        public string AvalaraAccount { get; set; }
        public string AvalaraLicense { get; set; }
        public string AvalaraCompanyCode { get; set; }
        public string AvalaraFreightIdentifier { get; set; }
        public bool AvataxIsTaxIncluded { get; set; }
    }
}
