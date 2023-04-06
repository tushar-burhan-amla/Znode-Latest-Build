using System;

namespace Znode.Engine.Api.Models
{
    public class AttributesValidationModel:BaseModel
    {
        public int MediaAttributeValidationId { get; set; }
        public Nullable<int> MediaAttributeId { get; set; }
        public Nullable<int> InputValidationId { get; set; }
        public Nullable<int> InputValidationRuleId { get; set; }
        public string Name { get; set; }
    }
}
