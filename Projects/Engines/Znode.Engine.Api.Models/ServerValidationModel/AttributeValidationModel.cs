namespace Znode.Engine.Api.Models
{
    public class AttributeValidationModel
    {
        public int? AttributeId { get; set; }
        public int? AttributeTypeId { get; set; }
        public string AttributeTypeName { get; set; }
        public string AttributeCode { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsLocalizable { get; set; }
        public bool? IsFilterable { get; set; }
        public string AttributeName { get; set; }
        public string ControlName { get; set; }
        public string ValidationName { get; set; }
        public string SubValidationName { get; set; }
        public string ValidationValue { get; set; }
        public string RegExp { get; set; }
        public bool? IsRegExp { get; set; }

    }
}
