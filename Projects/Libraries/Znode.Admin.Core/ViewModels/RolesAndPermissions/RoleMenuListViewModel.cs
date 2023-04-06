using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.ViewModels
{
    //Role Menu List View Model
    public class RoleMenuListViewModel : BaseViewModel
    {
        public RoleMenuListViewModel()
        {
            RoleMenus = new List<RoleMenuModel>();
            Menus = new List<MenuModel>();
            Permissions = new List<PermissionsModel>();
            RoleMenuAccessMapper = new List<RoleMenuAccessMapperViewModel>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<MenuModel> Menus { get; set; }

        public List<PermissionsModel> Permissions { get; set; }

        public List<RoleMenuModel> RoleMenus { get; set; }
        public List<RoleMenuAccessMapperViewModel> RoleMenuAccessMapper { get; set; }
    }
}