using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    //Role Menu List Response
    public class RoleMenuListResponse : BaseListResponse
    {
        public List<RoleMenuModel> RoleMenus { get; set; }
        public List<PermissionsModel> Permissions { get; set; }
        public List<MenuModel> Menus { get; set; }
        public List<RoleMenuAccessMapperModel> AccessMapper { get; set; }
    }
}
