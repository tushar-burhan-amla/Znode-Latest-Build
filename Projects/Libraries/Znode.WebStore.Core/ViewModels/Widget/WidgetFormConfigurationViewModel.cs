namespace Znode.Engine.WebStore.ViewModels
{
    public class WidgetFormConfigurationViewModel : BaseViewModel
    {
        public int CMSMappingId { get; set; }
        public int LocaleId { get; set; }
        public string FormCode { get; set; }
        public string FormBuilderId { get; set; }
        public string TypeOfMapping { get; set; }
        public string DisplayName { get; set; }
    }
}
