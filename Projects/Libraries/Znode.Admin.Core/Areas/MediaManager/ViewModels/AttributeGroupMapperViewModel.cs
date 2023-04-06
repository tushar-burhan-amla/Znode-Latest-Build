namespace Znode.Engine.Admin.ViewModels
{
    public class AttributeGroupMapperViewModel : BaseViewModel
    {
        public int MediaAttributeGroupMapperId { get; set; }
        public int? MediaAttributeId { get; set; }
        public int? MediaAttributeGroupId { get; set; }
        public int? AttributeDisplayOrder { get; set; }
        public string AttributeLabel { get; set; }
        public string AttributeType { get; set; }

        public virtual AttributesViewModel Attribute { get; set; }
        public virtual AttributeGroupViewModel AttributeGroup { get; set; }

        public string Code { get; set; }
        public bool IsRequired { get; set; }
        public int AttributeTypeId { get; set; }
        public bool? IsLocalizable { get; set; }
        public bool IsFilterable { get; set; }
        public bool IsSystemDefined { get; set; }
    }
}