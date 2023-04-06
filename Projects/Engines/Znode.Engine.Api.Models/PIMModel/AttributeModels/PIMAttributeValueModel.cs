namespace Znode.Engine.Api.Models
{
    public class PIMAttributeValueModel : BaseModel
    {
        public int PimAttributeValueId { get; set; }
        public int? PimAttributeFamilyId { get; set; }
        public int? PimProductId { get; set; }
        public int? PimAttributeId { get; set; }
        public int? PimAttributeDefaultValueId { get; set; }
        public string AttributeCode { get; set; }
    }
}
