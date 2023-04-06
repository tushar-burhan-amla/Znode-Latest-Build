namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeGroupMapperModel : BaseModel
    {
        public int GlobalAttributeGroupMapperId { get; set; }
        public int GlobalAttributeId { get; set; }
        public int? GlobalAttributeGroupId { get; set; }
        public int? AttributeDisplayOrder { get; set; }

        public GlobalAttributeModel Attribute { get; set; }
    }
}
