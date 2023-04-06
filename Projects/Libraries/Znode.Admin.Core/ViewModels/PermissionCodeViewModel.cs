using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PermissionCodeViewModel : BaseViewModel
    {
        public int? AccountPermissionAccessId { get; set; }
        public int? AccountUserPermissionId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelPermissionName, ResourceType = typeof(Admin_Resources))]
        public string PermissionCode { get; set; }
        public string PermissionsName { get; set; }
        public int UserId { get; set; }
    }
}
