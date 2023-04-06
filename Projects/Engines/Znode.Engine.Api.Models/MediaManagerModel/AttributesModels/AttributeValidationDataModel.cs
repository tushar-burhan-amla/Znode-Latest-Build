namespace Znode.Engine.Api.Models
{
    public class AttributeValidationDataModel : BaseModel
    {
        public int AttributeValidationId { get; set; }
        public int? AttributeId { get; set; }
        public int? InputValidationRuleId { get; set; }
        public int? InputValidationId { get; set; }
        public string Name { get; set; }
    }
}
