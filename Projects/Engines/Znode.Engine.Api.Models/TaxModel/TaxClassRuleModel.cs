namespace Znode.Engine.Api.Models
{
    public class TaxClassRuleModel:BaseModel
    {
        public string SKU { get; set; }
        public string DestinationCountryCode { get; set; }
        public int TaxClassId { get; set; }
    }
}
