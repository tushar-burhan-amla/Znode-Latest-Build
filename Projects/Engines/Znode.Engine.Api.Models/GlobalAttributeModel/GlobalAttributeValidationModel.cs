namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeValidationModel : BaseModel
    {
        public int GlobalAttributeValidationId { get; set; }
        public int? GlobalAttributeId { get; set; }
        public int? InputValidationId { get; set; }
        public int? InputValidationRuleId { get; set; }
        public string Name { get; set; }
    }
}
