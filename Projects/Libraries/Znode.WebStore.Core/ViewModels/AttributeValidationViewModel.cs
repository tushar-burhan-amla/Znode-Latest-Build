namespace Znode.Engine.WebStore.ViewModels
{
    public class AttributeValidationViewModel : BaseViewModel
    {
        public int PimAttributeFamilyId { get; set; }
        public int? AttributeTypeId { get; set; }
        public string AttributeTypeName { get; set; }
        public string AttributeCode { get; set; }
        public bool IsRequired { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public string AttributeDefaultValue { get; set; }
        public string AttributeDefaultValueCode { get; set; }
        public string ControlName { get; set; }
        public string ValidationName { get; set; }
        public string SubValidationName { get; set; }
        public string ValidationValue { get; set; }
        public Property ControlProperty { get; set; }
        public AttributeValidationViewModel()
        {
            ControlProperty = new Property();
        }
    }
}