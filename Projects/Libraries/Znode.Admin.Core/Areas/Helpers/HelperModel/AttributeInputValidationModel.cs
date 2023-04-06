using System.Collections.Generic;

namespace Znode.Engine.Admin.AttributeValidationHelpers
{
    public class AttributeInputValidationModel
    {
        public int AttributeValidationId { get; set; }
        public int? AttributeTypeId { get; set; }
        public string ValidationName { get; set; }
        public int? DisplayOrder { get; set; }
        public string DefaultValue { get; set; }
        public bool? IsList { get; set; }
        public string ControlName { get; set; }
        public string Name { get; set; }
        public List<AttributeValidationRuleModel> Rules { get; set; }
        public Property ControlProperty { get; set; }
       
    }
}