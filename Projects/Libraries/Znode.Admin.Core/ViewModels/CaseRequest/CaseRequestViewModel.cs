using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CaseRequestViewModel : BaseViewModel
    {

        [Display(Name = ZnodeAdmin_Resources.LabelFirstName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredFirstName)]
        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.FirstNameLengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string FirstName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLastName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredLastName)]
        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.LastNameLengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string LastName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.HeaderCompanyName, ResourceType = typeof(Admin_Resources))]
        public string CompanyName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelEmailID, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField)]
        [RegularExpression(AdminConstants.EmailRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress)]
        public string EmailId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPhoneNumber, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredPhoneNumber)]
        [MaxLength(30, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PhoneNumberMaxLengthErrorMessage)]
        public string PhoneNumber { get; set; }

        public string Message { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCreatedDate, ResourceType = typeof(Admin_Resources))]
        public string CreatedDate { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCaseOrigin, ResourceType = typeof(Admin_Resources))]
        public string CaseOrigin { get; set; }

        [MaxLength(500, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorBannerTitle)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.BannerTitleRequiredMessage)]
        public string Title { get; set; }

        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredDescription)]
        [UIHint("RichTextBox")]
        [AllowHtml]
        public string Description { get; set; }

        public string CreateUser { get; set; }

        [Required(ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSelectStore, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelStoreName, ResourceType = typeof(Admin_Resources))]
        public string StoreName { get; set; }

        public string CaseStatusName { get; set; }
        public string CasePriorityName { get; set; }
        public string CaseTypeName { get; set; }

        public List<SelectListItem> PortalList { get; set; }
        public List<SelectListItem> CaseStatusList { get; set; }
        public List<SelectListItem> CasePriorityList { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelEmailSubject, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredEmailSubject)]
        public string EmailSubject { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredEmailMessage)]
        [Display(Name = ZnodeAdmin_Resources.LabelEmailMessage, ResourceType = typeof(Admin_Resources))]
        [AllowHtml]
        [UIHint("RichTextBox")]
        public string EmailMessage { get; set; }

        [FileMaxSizeValidation(AdminConstants.ImageMaxFileSize, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ImageMaxFileSizeErrorMessage)]
        [FileTypeValidation(AdminConstants.FileTypes, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.FileTypeErrorMessage)]
        [Display(Name = ZnodeAdmin_Resources.LabelFileName, ResourceType = typeof(Admin_Resources))]
        [UIHint("FileUploader")]
        public HttpPostedFileBase FilePath { get; set; }

        public int CaseRequestId { get; set; }

        [Required(ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSelectStore, ErrorMessageResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }
        public int? AccountId { get; set; }
        public int? UserId { get; set; }
        public int? OwnerAccountId { get; set; }
        public int CaseStatusId { get; set; }
        public int CasePriorityId { get; set; }
        public int CaseTypeId { get; set; } = 1;
        public string AttachedPath { get; set; }
        public int? CaseRequestHistoryId { get; set; }
        public string UserName { get; set; }
    }
}