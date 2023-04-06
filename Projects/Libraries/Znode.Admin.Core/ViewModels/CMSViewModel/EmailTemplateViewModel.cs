using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class EmailTemplateViewModel : BaseViewModel
    {
        public int EmailTemplateId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.EmailTemplateNameRequiredMessage)]
        [Display(Name = ZnodeAdmin_Resources.LabelEmailTemplateName, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.AlphaNumericOnlyValidation, ErrorMessageResourceName = ZnodeAdmin_Resources.AlphaNumericOnly, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.EmailTemplateNameLengthErrorMessage)]
        public string TemplateName { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.EmailTemplateDescriptionsLengthErrorMessage)]
        [Display(Name = ZnodeAdmin_Resources.LabelDescription,ResourceType =typeof(Admin_Resources))]
        public string Descriptions { get; set; }       
       
       
        [Display(Name = ZnodeAdmin_Resources.LabelEmailContent, ResourceType = typeof(Admin_Resources))]        
        [AllowHtml]
        [UIHint("RichTextBox")]
        public string Html { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.EmailTemplateSubjectLengthErrorMessage)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.SubjectRequiredMessage)]
        public string Subject { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLocale, ResourceType = typeof(Admin_Resources))]
        public int LocaleId { get; set; }
        public List<SelectListItem> Locale { get; set; }
        public string EmailTemplateTokens { get; set; }
        public string EmailTemplateTokensPartOne { get; set; }
        public string EmailTemplateTokensPartTwo { get; set; }

        
        [Display(Name = ZnodeAdmin_Resources.LabelSMSContent, ResourceType = typeof(Admin_Resources))]
        public string SmsContent { get; set; }
    }
}