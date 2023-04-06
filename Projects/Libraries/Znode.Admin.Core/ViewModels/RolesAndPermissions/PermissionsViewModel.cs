namespace Znode.Engine.Admin.ViewModels
{
    //Permissions View Model
    public class PermissionsViewModel
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string PermissionCode { get; set; }
        public string TypeOfPermission { get; set; }
        public bool IsActive { get; set; }

    }
}