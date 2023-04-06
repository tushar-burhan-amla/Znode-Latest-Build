namespace Znode.Engine.WebStore.ViewModels
{
    public class WidgetProductViewModel : BaseViewModel
    {
        public int WidgetProductId { get; set; }
        public int PortalId { get; set; }
        public int MappingId { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOFMapping { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
        public ProductViewModel ProductViewModel { get; set; }
    }
}