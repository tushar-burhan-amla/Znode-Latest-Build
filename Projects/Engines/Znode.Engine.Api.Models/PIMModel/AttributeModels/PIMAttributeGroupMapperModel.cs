namespace Znode.Engine.Api.Models
{
    public class PIMAttributeGroupMapperModel :BaseModel
    {
        public int PimAttributeGroupMapperId { get; set; }
        public int PimAttributeId { get; set; }
        public int? PimAttributeGroupId { get; set; }
        public int? AttributeDisplayOrder { get; set; }
        public bool IsSystemDefined { get; set; }

        public PIMAttributeModel Attribute { get; set; }
    }
}
