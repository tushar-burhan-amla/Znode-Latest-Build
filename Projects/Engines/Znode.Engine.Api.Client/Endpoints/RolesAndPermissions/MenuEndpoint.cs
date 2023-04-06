namespace Znode.Engine.Api.Client.Endpoints
{
    public class MenuEndpoint : BaseEndpoint
    {
        //Create menu endpoint
        public static string Create() => $"{ApiRoot}/menu";

        //Get menu list endpoint
        public static string GetMenuList() => $"{ApiRoot}/menu/list";

        //Get menu list by parent menu id apart from already existing menus endpoint
        public static string GetMenuListByParentMenuId(string preSelectedMenuIds) => $"{ApiRoot}/menubyparent/{preSelectedMenuIds}";

        //Delete menu endpoint
        public static string Delete() => $"{ApiRoot}/deletemenu";

        //Update menu endpoint
        public static string Update(int menuId) => $"{ApiRoot}/menu/{menuId}";

        //Get menu by menu id endpoint
        public static string GetMenuByMenuId(int menuId) => $"{ApiRoot}/menus/{menuId}";

        //Get unselected menu by menu id endpoint
        public static string GetUnSelectedMenus() => $"{ApiRoot}/getunselectedmenus";

        //Get menu by menu id endpoint
        public static string GetMenuActionsPermissionList(int menuId) => $"{ApiRoot}/menu/getmenuactionspermissionlist/{menuId}";

        //Update Action Permission Endpoint
        public static string UpdateActionPermissions() => $"{ApiRoot}/menu/updateactionpermissions";
    }
}
