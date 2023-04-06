using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;


namespace Znode.Engine.Admin.ViewModels

{
    public class FormWidgetEmailConfigurationViewModel : BaseViewModel
    {
        public int FormWidgetEmailConfigurationId { get; set; }
        public int CMSContentPagesId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.NotificationEmailId, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress)]
        public string NotificationEmailId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.NotificationEmailTemplateId, ResourceType = typeof(Admin_Resources))]
        public int NotificationEmailTemplateId { get; set; }

        public string NotificationEmailTemplate { get; set; }

        [Display(Name = ZnodeAdmin_Resources.AcknowledgementEmailTemplateId, ResourceType = typeof(Admin_Resources))]
        public int AcknowledgementEmailTemplateId { get; set; }

        public string AcknowledgementEmailTemplate { get; set; }
        public string WidgetsKey { get; set; }
        public string FormTitle { get; set; }
        public int CMSWidgetsId { get; set; }
        public int CheckEmailType { get; set; }
        public string TypeOFMapping { get; set; }
        public string DisplayName { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }
    }
}
