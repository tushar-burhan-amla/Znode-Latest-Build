using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class AccountPermissionViewModel : BaseViewModel
    {
        [MaxLength(300, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorPermissionsName)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRequiredPermissionName)]
        public string AccountPermissionName { get; set; }

        public int AccountPermissionId { get; set; }
        public int AccountId { get; set; }
        public string PermissionsName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRequiredPermissionAccess)]
        public int AccessPermissionId { get; set; }

        public List<SelectListItem> AccessPermissions { get; set; }
    }
}