namespace Znode.Engine.Admin.ViewModels
{
    public class EntityAttributeDetailsViewModel : BaseViewModel
    {
        public int GlobalAttributeId { get; set; }
        public int? GlobalAttributeValueId { get; set; }
        public int? GlobalAttributeDefaultValueId { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeValue { get; set; }
        public int LocaleId { get; set; }
    }
}
