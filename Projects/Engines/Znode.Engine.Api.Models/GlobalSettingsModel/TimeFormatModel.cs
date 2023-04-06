namespace Znode.Engine.Api.Models
{
    public class TimeFormatModel : BaseModel
    {
        public int TimeFormatId { get; set; }
        public string TimeFormat { get; set; }
        public int? ConversionFactor { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}



