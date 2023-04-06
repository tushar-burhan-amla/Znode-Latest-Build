namespace Znode.Engine.Admin.ViewModels
{
    public class WebSiteThemeWidgetViewModel : BaseViewModel
    {
        public int CMSThemeWidgetId { get; set; }
        public int CMSWidgetsId { get; set; }
        public int PortalId { get; set; }
        public string DisplayName { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public string Code { get; set; }
    }
}