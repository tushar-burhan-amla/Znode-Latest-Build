using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    //Menu List Model
    public class MenuListModel : BaseListModel
    {
        public List<MenuModel> Menus { get; set; }
        public List<MenuModel> ParentMenus { get; set; }
        public PermissionListModel Permissions { get; set; }

        public MenuListModel()
        {
            Menus = new List<MenuModel>();
            ParentMenus = new List<MenuModel>();
            Permissions = new PermissionListModel();
        }
    }
}
