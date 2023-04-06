namespace Znode.Engine.Api.Models
{
    public class DateFormatModel : BaseModel
    {
        public int DateFormatId { get; set; }
        public string DateFormat { get; set; }
        public int? ConversionFactor { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}
