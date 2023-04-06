using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;
using System.Web.Mvc;

namespace Znode.Engine.Admin.ViewModels
{
    public class WebSiteLogoViewModel : BaseViewModel
    {
        public int CMSPortalThemeId { get; set; }
        public int PortalId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorThemeRequired)]
        public int? CMSThemeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredWebsiteLogo)]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredWebsiteLogo)]
        public int MediaId { get; set; }
        public int? FaviconId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCSSRequired)]
        public int? CMSThemeCSSId { get; set; }

        [MaxLength(500, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorWebsiteTitleLength)]
        [Display(Name = ZnodeAdmin_Resources.LabelWebSiteTitle, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredWebSiteTitle)]
        public string WebSiteTitle { get; set; }
        public string WebsiteDescription { get; set; }
        public string LogoUrl { get; set; }
        public string PortalName { get; set; }
        public string ThemeName { get; set; }
        public string ParentThemeName { get; set; }
        public string FaviconUrl { get; set; }

        public List<WebSiteThemeWidgetViewModel> ThemeWidgetList { get; set; }
        public CMSWidgetsListViewModel Widgets { get; set; }
        public string FileName { get; set; }

        public List<SelectListItem> ThemeList { get; set; }
        public List<SelectListItem> CSSList { get; set; }

        public string Theme { get; set; }
        public string CSS { get; set; }
        public DynamicContentViewModel DynamicContent { get; set; }
    }
}