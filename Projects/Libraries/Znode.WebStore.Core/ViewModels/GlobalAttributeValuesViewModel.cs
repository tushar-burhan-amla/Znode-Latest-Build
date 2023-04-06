namespace Znode.Engine.WebStore.ViewModels
{
    public class GlobalAttributeValuesViewModel : BaseViewModel
    {
        public int GlobalEntityId { get; set; }
        public string EntityName { get; set; }
        public string EntityValue { get; set; }
        public int? GlobalAttributeGroupId { get; set; }
        public int GlobalAttributeId { get; set; }
        public int? AttributeTypeId { get; set; }
        public string AttributeTypeName { get; set; }
        public string AttributeCode { get; set; }
        public bool IsRequired { get; set; }
        public bool IsLocalizable { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public int? GlobalAttributeValueId { get; set; }
        public int? GlobalAttributeDefaultValueId { get; set; }
        public string AttributeDefaultValueCode { get; set; }
        public string AttributeDefaultValue { get; set; }
        public int RowId { get; set; }
        public bool IsEditable { get; set; }
        public string ControlName { get; set; }
        public string ValidationName { get; set; }
        public string SubValidationName { get; set; }
        public string RegExp { get; set; }
        public string ValidationValue { get; set; }
        public bool? IsRegExp { get; set; }
        public int? MediaId { get; set; }
        public Property ControlProperty { get; set; }
        public string HelpDescription { get; set; }

        public GlobalAttributeValuesViewModel()
        {
            ControlProperty = new Property();
        }
    }
}
