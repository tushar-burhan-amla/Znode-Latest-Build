using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    // Menu List View Model
    public class MenuListViewModel : BaseViewModel
    {
        public MenuListViewModel()
        {
            ParentMenus = new List<MenuViewModel>();
            Menus = new List<MenuViewModel>();
        }
        public List<MenuViewModel> ParentMenus { get; set; }

        public List<MenuViewModel> Menus { get; set; }

        public List<PermissionsViewModel> Permissions { get; set; }
      
        public string RoleId { get; set; }
        public DropDownOptions Data { get; set; }
       
        public string RoleName { get; set; }
        public GridModel GridModel { get; set; }
    }
}