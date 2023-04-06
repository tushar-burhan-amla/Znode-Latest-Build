
namespace Znode.Engine.Admin.ViewModels
{
    public class RolePermissionViewModel :BaseViewModel
    {
        public int AccessPermissionsId { get; set; }
        public int? MenuId { get; set; }
        public string RequestUrlTemplate { get; set; }
    }
}