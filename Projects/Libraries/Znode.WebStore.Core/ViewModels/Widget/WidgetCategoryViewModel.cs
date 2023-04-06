namespace Znode.Engine.WebStore.ViewModels
{
    public class WidgetCategoryViewModel : BaseViewModel
    {
        public int WidgetCategoryId { get; set; }
        public int PublishCategoryId { get; set; }
        public int MappingId { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOFMapping { get; set; }
        public CategoryViewModel CategoryViewModel { get; set; }
    }
}