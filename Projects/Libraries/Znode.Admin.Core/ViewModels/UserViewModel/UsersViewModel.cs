using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{

    public class UsersViewModel : BaseViewModel
    {
        /// <summary>
        /// Constructor for UsersViewModel
        /// </summary>
        public UsersViewModel()
        {
            RoleList = new List<SelectListItem>();
        }

        [Display(Name = ZnodeAdmin_Resources.LabelUserId, ResourceType = typeof(Admin_Resources))]
        public string AspNetUserId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelEmail, ResourceType = typeof(Admin_Resources))]
        [EmailAddress(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "ValidEmailAddress", ErrorMessage = "")]
        public string Email { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPhoneNumber, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorFieldLengthValidation)]
        public string PhoneNumber { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFirstName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredFirstName")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorFieldLengthValidation)]
        public string FirstName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelLastName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredLastName")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorFieldLengthValidation)]
        public string LastName { get; set; }

        public string CreateUser { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelUserNameEmail, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = "RequiredUserId")]
        [RegularExpression(AdminConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress)]
        public string UserName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAccount, ResourceType = typeof(Admin_Resources))]
        public int UserId { get; set; }

        public string RoleId { get; set; }
        public string RoleName { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelRoleName, ResourceType = typeof(Admin_Resources))]
        public List<SelectListItem> RoleList { get; set; }

        public int PortalId { get; set; }
        public bool IsLock { get; set; }
        public string FullName { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorFieldLengthValidation)]
        public string ExternalId { get; set; }
        public int ParentAccountId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStores, ResourceType = typeof(Admin_Resources))]
        public List<SelectListItem> Portals { get; set; }        
        public string[] PortalIds { get; set; }

        public string PortalIdString { get; set; }
        public bool IsSelectAllPortal { get; set; }
        public bool IsEmailSentFailed { get; set; }
        public int? MediaId { get; set; }
        public string MediaPath { get; set; } = "";      
        public bool IsVerified { get; set; }
    }
}

