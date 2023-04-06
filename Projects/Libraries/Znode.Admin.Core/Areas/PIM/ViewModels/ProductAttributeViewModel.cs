namespace Znode.Engine.Admin.ViewModels
{
    public class ProductAttributeViewModel : BaseViewModel
    {
        public int ProductId { get; set; }
        public int LocaleId { get; set; }
        public string AssociatedProducts { get; set; }
        public string ConfigureAttributeIds { get; set; }
        public string ConfigureFamilyIds { get; set; }
        public int ProductAttributeId { get; set; }
        public string ProductAttributeCode { get; set; }
        public string ProductAttributeValue { get; set; }
        public int ProductAttributeFamilyId { get; set; }
        public int? ProductAttributeValueId { get; set; }
        public int? ProductAttributeDefaultValueId { get; set; }
    }
}