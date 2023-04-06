namespace Znode.Engine.WebStore.ViewModels
{
    public class WidgetBrandViewModel : BaseViewModel
    {
        public int WidgetBrandId { get; set; }
        public int BrandId { get; set; }
        public int MappingId { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOFMapping { get; set; }
        public BrandViewModel BrandViewModel { get; set; }
    }
}