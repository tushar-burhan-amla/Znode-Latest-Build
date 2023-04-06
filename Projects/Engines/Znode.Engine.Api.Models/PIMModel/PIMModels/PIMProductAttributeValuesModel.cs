namespace Znode.Engine.Api.Models
{
    public class PIMProductAttributeValuesModel : BaseModel
    {
        public int PimAttributeId { get; set; }
        public int PimAttributeFamilyId { get; set; }
        public int? PimAttributeGroupId { get; set; }
        public int? AttributeTypeId { get; set; }
        public string AttributeTypeName { get; set; }
        public string AttributeCode { get; set; }
        public bool IsRequired { get; set; }
        public bool IsLocalizable { get; set; }
        public bool IsFilterable { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public int? PimAttributeValueId { get; set; }
        public int? PimAttributeDefaultValueId { get; set; }
        public string AttributeDefaultValue { get; set; }
        public string AttributeDefaultValueCode { get; set; }
        public int RowId { get; set; }
        public bool? IsEditable { get; set; }
        public string ControlName { get; set; }
        public string ValidationName { get; set; }
        public string SubValidationName { get; set; }
        public string RegExp { get; set; }
        public string ValidationValue { get; set; }
        public bool? IsRegExp { get; set; }
        public string HelpDescription { get; set; }
        public bool? IsConfigurableAttribute { get; set; }
        public int PimProductId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int? DisplayOrder { get; set; }
        public string ProductType { get; set; }
        public string AttributeFamily { get; set; }
        public string ProductImage { get; set; }
        public bool IsDownloadable { get; set; }
        public string FilesName { get; set; }
        public bool IsDefault { get; set; }
    }
}
