using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CMSFormWidgetConfigurationViewModel : BaseViewModel
    {
        public int CMSFormWidgetConfigurationId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }

        public int CMSWidgetsIdLocaleId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }

        [Display(Name = ZnodeAdmin_Resources.FormBuilderId, ResourceType = typeof(Admin_Resources))]
        public int FormBuilderId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredFormName)]
        public string FormTitle { get; set; }
        public string ButtonText { get; set; }
        public bool IsTextMessage { get; set; }
        [MaxLength(500, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorTextMesage)]
        public string TextMessage { get; set; }
        public string RedirectURL { get; set; }

        public string DisplayName { get; set; }

        public string WidgetName { get; set; }
        public int CMSWidgetsId { get; set; }

        public string FileName { get; set; }

        public List<SelectListItem> Locales { get; set; }
        public List<FormBuilderViewModel> FormBuilder { get; set; }
        public bool EnableCMSPreview { get; set; }
    }
}
