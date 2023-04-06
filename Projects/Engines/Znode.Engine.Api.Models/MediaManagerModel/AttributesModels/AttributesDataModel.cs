
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributesDataModel : BaseModel
    {
        public int MediaAttributeId { get; set; }
        public int? AttributeTypeId { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeName { get; set; }
        public string AttributeTypeName { get; set; }
        public bool? IsRequired { get; set; } = true;
        public bool? IsLocalizable { get; set; } = true;
        public bool? IsFilterable { get; set; } = true;
        public int? AttributeValidationId { get; set; }
        public int? AttributeGroupId { get; set; }
        public bool? IsSystemDefined { get; set; }
        public string HelpDescription { get; set; }
        public int? DisplayOrder { get; set; }

        public List<AttributesValidationModel> ValidationRule { get; set; }
        
    }
}
