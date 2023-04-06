namespace Znode.Engine.Api.Models
{
    public class TimeZoneModel : BaseModel
    {
        public int TimeZoneId { get; set; }
        public string TimeZoneDetailsCode { get; set; }
        public string TimeZoneDetailsDesc { get; set; }
        public int DifferenceInSeconds { get; set; }
        public string DaylightBeginsAt { get; set; }
        public string DaylightEndsAt { get; set; }
        public int? DSTInSeconds { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDefault { get; set; }
    }
}
