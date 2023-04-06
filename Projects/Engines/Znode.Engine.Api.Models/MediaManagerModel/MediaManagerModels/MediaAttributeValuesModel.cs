namespace Znode.Engine.Api.Models
{
    public class MediaAttributeValuesModel : BaseModel
    {
        public int? MediaCategoryId { get; set; }
        public int MediaId { get; set; }
        public int? MediaPathId { get; set; }
        public int? AttributeFamilyId { get; set; }
        public int? AttributeId { get; set; }
        public int? AttributeTypeId { get; set; }
        public string AttributeTypeName { get; set; }
        public string AttributeCode { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsLocalizable { get; set; }
        public bool? IsFilterable { get; set; }
        public string AttributeName { get; set; }
        public string MediaAttributeValue { get; set; }
        public int? MediaAttributeValueId { get; set; }
        public int? DefaultAttributeValueId { get; set; }
        public string DefaultAttributeValue { get; set; }
        public string MediaPath { get; set; }
        public string MediaAttributeThumbnailPath { get; set; }
        public int RowId { get; set; }
        public bool? IsEditable { get; set; }
        public string ControlName { get; set; }
        public string ValidationName { get; set; }
        public string SubValidationName { get; set; }
        public string RegExp { get; set; }
        public string ValidationValue { get; set; }
        public bool? IsRegExp { get; set; }
        public string AttributeGroupName { get; set; }
        public string FamilyCode { get; set; }
        public string HelpDescription { get; set; }
    }
}
