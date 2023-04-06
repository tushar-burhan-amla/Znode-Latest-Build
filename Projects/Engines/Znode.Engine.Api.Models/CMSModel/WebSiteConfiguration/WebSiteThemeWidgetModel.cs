namespace Znode.Engine.Api.Models
{
    public class WebSiteThemeWidgetModel :BaseModel
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
