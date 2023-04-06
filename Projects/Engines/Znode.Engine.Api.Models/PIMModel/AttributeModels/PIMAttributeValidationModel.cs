namespace Znode.Engine.Api.Models
{
    public class PIMAttributeValidationModel : BaseModel
    {
        public int PimAttributeValidationId { get; set; }
        public int? PimAttributeId { get; set; }
        public int? InputValidationId { get; set; }
        public int? InputValidationRuleId { get; set; }
        public string Name { get; set; }
    }
}
