namespace Znode.Engine.Api.Client.Endpoints
{
    public class RoleEndpoint : BaseEndpoint
    {
        //Create role endpoint
        public static string Create() => $"{ApiRoot}/role";

        //Get permissions endpoint
        public static string GetPermissionList() => $"{ApiRoot}/permission/list";

        //Get role list endpoint
        public static string GetRoleList() => $"{ApiRoot}/role/list";

        //Delete role endpoint
        public static string Delete() => $"{ApiRoot}/deleterole";

        //Update role endpoint
        public static string Update(string roleId) => $"{ApiRoot}/role/{roleId}";

        //Get role by role id endpoint
        public static string GetRoleByRoleId(string roleId) => $"{ApiRoot}/roles/{roleId}";

        //Get Roles Menus Permissions With Role Menus endpoint
        public static string GetRolesMenusPermissionsWithRoleMenus() => $"{ApiRoot}/getrolesmenuspermissionswithrolemenus";

        //Get permission list based on UserName.
        public static string GetRolePermissionByUserName() => $"{ApiRoot}/rolespermission/getrolepermission";
    }
}
