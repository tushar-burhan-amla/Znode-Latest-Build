namespace Znode.Engine.Admin.ViewModels
{
    public class GlobalAttributeGroupMapperViewModel
    {
        public int GlobalAttributeGroupMapperId { get; set; }
        public int? GlobalAttributeId { get; set; }
        public int? GlobalAttributeGroupId { get; set; }

        public string AttributeCode { get; set; }
        public bool IsRequired { get; set; }
        public int GlobalAttributeTypeId { get; set; }
        public string AttributeType { get; set; }
        public bool? IsLocalizable { get; set; }
        public string AttributeName { get; set; }
        public int? DisplayOrder { get; set; }
    }
}