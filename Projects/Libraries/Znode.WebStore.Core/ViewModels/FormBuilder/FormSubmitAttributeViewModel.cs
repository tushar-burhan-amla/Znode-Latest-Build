namespace Znode.Engine.WebStore.ViewModels
{
    public class FormSubmitAttributeViewModel : BaseViewModel
    {
        public int GlobalAttributeId { get; set; }
        public int? GlobalAttributeValueId { get; set; }
        public int? GlobalAttributeDefaultValueId { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeValue { get; set; }
        public int LocaleId { get; set; }
        public int? FormTemplateId { get; set; }
    }
}
