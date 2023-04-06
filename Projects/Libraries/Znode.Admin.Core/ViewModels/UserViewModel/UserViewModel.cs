using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Api.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    /// <summary>
    /// View Model for User.
    /// </summary>
    public class UserViewModel : BaseViewModel
    {
        /// <summary>
        /// Deafult Constructor.
        /// </summary>
        public UserViewModel()
        {

        }

        public int UserId { get; set; }

        public string CompanyName { get; set; }
        public bool EmailOptIn { get; set; }
        public bool? EnableCustomerPricing { get; set; }
        public string ExternalId { get; set; }
        public bool? IsActive { get; set; }
        public int? ProfileId { get; set; }

        public string FullName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.Username, ResourceType = typeof(Admin_Resources))]
        public string UserName { get; set; }

        public string PasswordQuestion { get; set; }

        public string PasswordAnswer { get; set; }

        public string AspNetUserId { get; set; }
        public string BaseUrl { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredUserName)]
        [EmailAddress(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidUsername)]
        public string EmailAddress { get; set; }

        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public int? MediaId { get; set; }
        public string MediaPath { get; set; }

        public IEnumerable<RolePermissionModel> RolePermission { get; set; }
        public IEnumerable<GlobalAttributeValuesModel> UserGlobalAttributes { get; set; }
    }
}