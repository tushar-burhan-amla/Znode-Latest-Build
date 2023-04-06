namespace Znode.Engine.Admin.AttributeValidationHelpers
{
    public class AttributeValidationRuleModel
    {
        public int InputValidationRuleId { get; set; }
        public int? InputValidationId { get; set; }
        public string ValidationRule { get; set; }
        public string ValidationName { get; set; }
        public int? DisplayOrder { get; set; }
        public string RegExp { get; set; }
    }
}