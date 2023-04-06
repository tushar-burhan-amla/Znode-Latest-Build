namespace Znode.Engine.Api.Models
{
    public class AttributeGroupMapperModel : BaseModel
    {
        public int MediaAttributeGroupMapperId { get; set; } 
        public int? MediaAttributeId { get; set; }
        public int? MediaAttributeGroupId { get; set; }
        public int? AttributeDisplayOrder { get; set; }
        public string AttributeLabel { get; set; }
        public string AttributeType { get; set; }
        public bool IsSystemDefined { get; set; }

        public AttributesDataModel Attribute { get; set; }
        public AttributeGroupModel AttributeGroup { get; set; }
    }
}
