using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CMSTextWidgetConfigurationViewModel :BaseViewModel
    {
        public int CMSTextWidgetConfigurationId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }

        public string DisplayName { get; set; }

        public string WidgetName { get; set; }
        public string FileName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelContentHtml, ResourceType = typeof(Admin_Resources))]
        [AllowHtml]
        [UIHint("RichTextBox")]
        public string Text { get; set; }

        public List<SelectListItem> Locales { get; set; }
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public bool EnableCMSPreview { get; set; }
    }
}