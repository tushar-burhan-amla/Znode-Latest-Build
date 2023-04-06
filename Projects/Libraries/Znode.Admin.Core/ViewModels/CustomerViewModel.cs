using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        [Display(Name = ZnodeAdmin_Resources.LabelFullName, ResourceType = typeof(Admin_Resources))]
        public string FullName { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelCompanyName, ResourceType = typeof(Admin_Resources))]
        public string CompanyName { get; set; }

        public string LoginName { get; set; }
        public bool IsConfirmed { get; set; }
        public int ProfileId { get; set; }
        public bool IsUserAdmin { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelUserId, ResourceType = typeof(Admin_Resources))]
        public string AspNetUserId { get; set; }

        public int UserId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelEmail, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress)]
        public string Email { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPhoneNumber, ResourceType = typeof(Admin_Resources))]
        [MaxLength(30, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PhoneNumberMaxLengthErrorMessage)]
        public string PhoneNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFirstName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredFirstName")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.FirstNameLengthErrorMessage)]
        public string FirstName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLastName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredLastName")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.LastNameLengthErrorMessage)]
        public string LastName { get; set; }
        public string CreateUser { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelUserNameEmail, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredUserId")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.UserIdLengthErrorMessage)]
        [RegularExpression(AdminConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress)]
        public string UserName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAccount, ResourceType = typeof(Admin_Resources))]
        public int? AccountId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelDepartment, ResourceType = typeof(Admin_Resources))]
        public int? DepartmentId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelExternalId, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.ExternalIdCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string ExternalId { get; set; }

        public string PermissionsName { get; set; }
        public string DepartmentName { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelRoleName, ResourceType = typeof(Admin_Resources))]
        public string RoleName { get; set; }
        public string CustomerPaymentGUID { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }

        public bool EmailOptIn { get; set; }
        public bool SMSOptIn { get; set; }
        public bool IsLock { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelStore, ResourceType = typeof(Admin_Resources))]
        public List<SelectListItem> Portals { get; set; }
        public List<SelectListItem> Accounts { get; set; }
        public List<SelectListItem> Departments { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> Permissions { get; set; }
        public List<SelectListItem> UserApprovalList { get; set; }
        public string AccountPermissionList { get; set; }

        [Range(0, 999999, ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorMaxBudget, ErrorMessageResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^\d{0,}(\.\d{1,6})?$", ErrorMessageResourceName = ZnodeAdmin_Resources.InvalidMaxBudget, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelBudgetAmount, ResourceType = typeof(Admin_Resources))]
        public decimal? BudgetAmount { get; set; }
        public string[] PortalIds { get; set; }
        public int? PortalId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPermissionName, ResourceType = typeof(Admin_Resources))]
        public string PermissionCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelApprovalName, ResourceType = typeof(Admin_Resources))]
        public int? ApprovalUserId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelUserType, ResourceType = typeof(Admin_Resources))]
        public string RoleId { get; set; }
        public bool IsSelectAllPortal { get; set; }
        public bool IsGuestUser { get; set; }
        public int? AccountPermissionAccessId { get; set; }
        public string ApprovalName { get; set; }
        public int? AccountUserOrderApprovalId { get; set; }
        public string PermissionOptions { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TextStoreName, ResourceType = typeof(Admin_Resources))]
        [StringLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        public string StoreName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAccountName, ResourceType = typeof(Admin_Resources))]
        public string AccountName { get; set; }
        public bool IsEmailSentFailed { get; set; }
        public string ShippingPostalCode { get; set; }
        public string BillingPostalCode { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public int SalesRepId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.SalesRep, ResourceType = typeof(Admin_Resources))]
        public string SalesRepUserName { get; set; }
        public string SalesRepFullName { get; set; }
        public string AccountCode { get; set; }
        public bool IsTradeCentricUser { get; set; }
    }
}