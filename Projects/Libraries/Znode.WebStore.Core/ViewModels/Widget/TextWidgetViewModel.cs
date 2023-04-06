namespace Znode.Engine.WebStore.ViewModels
{
    public class TextWidgetViewModel : BaseViewModel
    {
        public int TextWidgetConfigurationId { get; set; }
        public int ContentPageId { get; set; }
        public int LocaleId { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOFMapping { get; set; }
        public string Text { get; set; }
    }
}