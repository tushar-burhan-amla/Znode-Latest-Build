namespace Znode.Engine.Admin.ViewModels
{
    public class CityViewModel : BaseViewModel
    {
        public int CityID { get; set; }
        public string CityName { get; set; }
        public string CityType { get; set; }
        public string ZIP { get; set; }
        public string ZIPType { get; set; }
        public string MyProperty { get; set; }
        public string CountyCode { get; set; }
        public string StateCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string CountyFIPS { get; set; }
        public string StateFIPS { get; set; }
        public string MSACode { get; set; }
        public string TimeZone { get; set; }
        public decimal UTC { get; set; }
        public char DST { get; set; }
    }
}