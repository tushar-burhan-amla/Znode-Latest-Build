using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CMSSearchWidgetConfigurationViewModel : BaseViewModel
    {
        public int CMSSearchWidgetId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }

        public string DisplayName { get; set; }

        public string WidgetName { get; set; }
        public string FileName { get; set; }

        public string AttributeCode { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSearchKeywordRequired)]
        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSearchKeywordLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelSearchKeyword, ResourceType = typeof(Admin_Resources))]

        [RegularExpression("[^,]+$", ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorMultipleSearchKeyWords, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string SearchKeyword { get; set; }

        public List<SelectListItem> Locales { get; set; }

        public List<SelectListItem> AttributeList { get; set; }
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public bool EnableCMSPreview { get; set; }
    }
}
