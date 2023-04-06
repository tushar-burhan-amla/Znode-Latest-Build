using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeInputValidationModel : BaseModel
    {
        public int AttributeValidationId { get; set; }
        public int? AttributeTypeId { get; set; }
        public string AttributeTypeName { get; set; }
        public string ValidationName { get; set; }
        public int? DisplayOrder { get; set; }
        public string DefaultValue { get; set; }
        public bool? IsList { get; set; }
        public string ControlName { get; set; }
        public string Name { get; set; }

        public List<GlobalAttributeValidationRuleModel> Rules { get; set; }
    }
}
