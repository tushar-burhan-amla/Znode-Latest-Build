using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    //Menu List Response
    public class MenuListResponse : BaseListResponse
    {
        public List<MenuModel> Menus { get; set; }
        public List<MenuModel> ParentMenus { get; set; }
    }
}
