using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    //Role List View Model
    public class RoleListViewModel : BaseViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<MenuModel> Menus { get; set; }

        public List<PermissionsViewModel> Permissions { get; set; }

        public List<RoleViewModel> Roles { get; set; }

        public List<RoleMenuAccessMapperViewModel> RoleMenuAccessMapper { get; set; }

        public GridModel GridModel { get; set; }

        public string SelectedMenuIds { get; set; }

        public RoleListViewModel()
        {
            Menus = new List<MenuModel>();
            Permissions = new List<PermissionsViewModel>();
            Roles = new List<RoleViewModel>();
            RoleMenuAccessMapper = new List<RoleMenuAccessMapperViewModel>();
            GridModel = new GridModel();
        }
    }
}