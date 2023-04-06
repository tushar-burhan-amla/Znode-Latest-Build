namespace Znode.Engine.Admin.ViewModels
{
    public class PIMAttributeGroupMapperViewModel
    {
        public int PimAttributeGroupMapperId { get; set; }
        public int? PimAttributeId { get; set; }
        public int? PimAttributeGroupId { get; set; }

        public string AttributeCode { get; set; }
        public bool IsRequired { get; set; }
        public int PimAttributeTypeId { get; set; }
        public string AttributeType { get; set; }
        public bool? IsLocalizable { get; set; }
        public bool IsSystemDefined { get; set; }
        public string AttributeName { get; set; }
        public int? DisplayOrder { get; set; }
    }
}