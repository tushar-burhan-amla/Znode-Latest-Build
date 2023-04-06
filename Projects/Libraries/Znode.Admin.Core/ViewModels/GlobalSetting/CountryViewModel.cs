namespace Znode.Engine.Admin.ViewModels
{
    public class CountryViewModel : BaseViewModel
    {
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string PortalCountryIds { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int PortalCountryId { get; set; }
        public int PortalId { get; set; }
    }
}