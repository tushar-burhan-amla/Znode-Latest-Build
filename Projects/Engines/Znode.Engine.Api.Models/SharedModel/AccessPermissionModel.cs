using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class AccessPermissionModel : BaseModel
    {
        public int AccessPermissionId { get; set; }

        [MaxLength(300, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorPermissionsName)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRequiredPermissionName)]
        public string AccountPermissionName { get; set; }
        public int AccountId { get; set; }
        public int AccountPermissionId { get; set; }
        public string PermissionsName { get; set; }
        public string TypeOfPermission { get; set; }
        public string PermissionCode { get; set; }
        public bool IsActive { get; set; }
        public int AccountPermissionAccessId { get; set; }

        public List<AccountPermissionAccessModel> AccountPermissionAccessList { get; set; }
    }
}
