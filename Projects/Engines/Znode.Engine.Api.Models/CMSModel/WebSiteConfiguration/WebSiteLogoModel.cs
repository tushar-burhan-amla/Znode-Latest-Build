using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebSiteLogoModel : BaseModel
    {
        public int CMSPortalThemeId { get; set; }
        public int PortalId { get; set; }
        public int MediaId { get; set; }
        public int FaviconId { get; set; }
        public int? CMSThemeCSSId { get; set; }
        public string WebSiteTitle { get; set; }
        public string WebsiteDescription { get; set; }
        public int CMSThemeId { get; set; }
        public string LogoUrl { get; set; }
        public string FaviconUrl { get; set; }
        public string PortalName { get; set; }
        public string ThemeName { get; set; }
        public string ParentThemeName { get; set; }
        public List<WebSiteThemeWidgetModel> ThemeWidgetList { get; set; }
        public DynamicContentModel DynamicContent { get; set; }
    }
}
