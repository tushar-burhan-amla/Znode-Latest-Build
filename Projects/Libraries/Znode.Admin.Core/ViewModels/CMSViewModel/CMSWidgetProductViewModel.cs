namespace Znode.Engine.Admin.ViewModels
{
    public class CMSWidgetProductViewModel : BaseViewModel
    {
        public int PublishProductId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int? localeId { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOfMapping { get; set; }
        public int CMSWidgetProductId { get; set; }
        public string ProductType { get; set; }
        public string ImagePath { get; set; }
        public int? DisplayOrder { get; set; }
    }
}