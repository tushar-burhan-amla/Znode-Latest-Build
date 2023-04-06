using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    //Role Menu List Model
    public class RoleMenuListModel : BaseListModel
    {
        public List<RoleMenuModel> RoleMenus { get; set; }
        public List<MenuModel> Menus { get; set; }
        public List<PermissionsModel> Permissions { get; set; }
        public List<RoleMenuAccessMapperModel> RoleMenuAccessMapper { get; set; }

        public RoleMenuListModel()
        {
            RoleMenus = new List<RoleMenuModel>();
            Menus = new List<MenuModel>();
            Permissions = new List<PermissionsModel>();
            RoleMenuAccessMapper = new List<RoleMenuAccessMapperModel>();
        }
    }
}
