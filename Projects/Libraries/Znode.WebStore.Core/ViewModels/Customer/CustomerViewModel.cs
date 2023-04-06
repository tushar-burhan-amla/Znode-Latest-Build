using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        [Display(Name = ZnodeWebStore_Resources.LabelFullName, ResourceType = typeof(WebStore_Resources))]
        public string FullName { get; set; }
        [Display(Name = ZnodeWebStore_Resources.LabelCompanyName, ResourceType = typeof(WebStore_Resources))]
        public string CompanyName { get; set; }

        public string LoginName { get; set; }
        public bool IsConfirmed { get; set; }
        public int ProfileId { get; set; }
        public bool IsUserAdmin { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelUserId, ResourceType = typeof(WebStore_Resources))]
        public string AspNetUserId { get; set; }

        public int UserId { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelEmail, ResourceType = typeof(WebStore_Resources))]
        [RegularExpression(WebStoreConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.ValidEmailAddress)]
        public string Email { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelPhoneNumber, ResourceType = typeof(WebStore_Resources))]
        [MaxLength(30, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.PhoneNumberMaxLengthErrorMessage)]
        public string PhoneNumber { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelFirstName, ResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredFirstName")]
        [MaxLength(100, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.FirstNameLengthErrorMessage)]
        public string FirstName { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelLastName, ResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredLastName")]
        [MaxLength(100, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.LastNameLengthErrorMessage)]
        public string LastName { get; set; }
        public string CreateUser { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelUserNameEmail, ResourceType = typeof(WebStore_Resources))]
        [Required(ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = "RequiredUserId")]
        [MaxLength(100, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.UserIdLengthErrorMessage)]
        [RegularExpression(WebStoreConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.ValidEmailAddress)]
        public string UserName { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelAccount, ResourceType = typeof(WebStore_Resources))]
        public int? AccountId { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelDepartment, ResourceType = typeof(WebStore_Resources))]
        public int? DepartmentId { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelExternalId, ResourceType = typeof(WebStore_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeWebStore_Resources.ExternalIdCodeLengthErrorMessage, ErrorMessageResourceType = typeof(WebStore_Resources))]
        public string ExternalId { get; set; }

        public string PermissionsName { get; set; }
        public string DepartmentName { get; set; }
        [Display(Name = ZnodeWebStore_Resources.LabelRoleName, ResourceType = typeof(WebStore_Resources))]
        public string RoleName { get; set; }
        public string CustomerPaymentGUID { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }

        public bool EmailOptIn { get; set; }
        public bool IsLock { get; set; }
        [Display(Name = ZnodeWebStore_Resources.LabelStore, ResourceType = typeof(WebStore_Resources))]
        public List<SelectListItem> Portals { get; set; }
        public List<SelectListItem> Accounts { get; set; }
        public List<SelectListItem> Departments { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> Permissions { get; set; }
        public List<SelectListItem> UserApprovalList { get; set; }
        public string AccountPermissionList { get; set; }

        [Range(0, 999999, ErrorMessageResourceName = ZnodeWebStore_Resources.ErrorMaxBudget, ErrorMessageResourceType = typeof(WebStore_Resources))]
        [RegularExpression(@"^\d{0,}(\.\d{1,6})?$", ErrorMessageResourceName = ZnodeWebStore_Resources.InvalidMaxBudget, ErrorMessageResourceType = typeof(WebStore_Resources))]
        [Display(Name = ZnodeWebStore_Resources.LabelBudgetAmount, ResourceType = typeof(WebStore_Resources))]
        public decimal? BudgetAmount { get; set; }
        public string[] PortalIds { get; set; }
        public int? PortalId { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelPermissionName, ResourceType = typeof(WebStore_Resources))]
        public string PermissionCode { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelApprovalName, ResourceType = typeof(WebStore_Resources))]
        public int? ApprovalUserId { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelUserType, ResourceType = typeof(WebStore_Resources))]
        public string RoleId { get; set; }
        public bool IsSelectAllPortal { get; set; }
        public bool IsGuestUser { get; set; }
        public int? AccountPermissionAccessId { get; set; }
        public string ApprovalName { get; set; }
        public int? AccountUserOrderApprovalId { get; set; }
        public string PermissionOptions { get; set; }

        [Display(Name = ZnodeWebStore_Resources.TextStoreName, ResourceType = typeof(WebStore_Resources))]
        [StringLength(100, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.Errorlength)]
        public string StoreName { get; set; }

        [Display(Name = ZnodeWebStore_Resources.LabelAccountName, ResourceType = typeof(WebStore_Resources))]
        public string AccountName { get; set; }
        public bool IsEmailSentFailed { get; set; }
        public string ShippingPostalCode { get; set; }
        public string BillingPostalCode { get; set; }

    }
}