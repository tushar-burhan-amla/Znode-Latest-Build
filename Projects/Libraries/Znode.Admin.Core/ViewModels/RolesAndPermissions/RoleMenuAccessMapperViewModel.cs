namespace Znode.Engine.Admin.ViewModels
{
    //Role Menu Access Mapper ViewModel
    public class RoleMenuAccessMapperViewModel : BaseViewModel
    {
        public int RoleMenuAccessMapperId { get; set; }
        public int AccessPermissionsId { get; set; }
        public int RoleMenuId { get; set; }
        public int MenuId { get; set; }
    }
}