namespace Znode.Engine.Api.Models
{
    public class CountryModel : BaseModel
    {
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int PortalCountryId { get; set; }
        public int PortalId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
