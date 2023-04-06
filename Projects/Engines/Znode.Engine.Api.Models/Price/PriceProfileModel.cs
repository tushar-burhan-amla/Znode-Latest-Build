namespace Znode.Engine.Api.Models
{
    public class PriceProfileModel : BaseModel
    {
        public int PortalProfileId { get; set; }
        public int PriceListId { get; set; }
        public int ProfileId { get; set; }
        public int? PortalId { get; set; }
        public int? PriceListProfileId { get; set; }
        public int Precedence { get; set; }
        public string ProfileName { get; set; }
        public int IsAssociated { get; set; }
        public string StoreName { get; set; }
    }
}
