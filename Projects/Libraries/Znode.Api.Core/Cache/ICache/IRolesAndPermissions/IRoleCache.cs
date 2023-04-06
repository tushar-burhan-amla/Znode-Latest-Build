namespace Znode.Engine.Api.Cache
{
    public interface IRoleCache
    {
        /// <summary>
        /// Method to get list of permissions.
        /// </summary>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>Returns string.</returns>
        string GetPermissions(string routeUri, string routeTemplate);

        /// <summary>
        /// Method to get list of roles
        /// </summary>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>Returns string.</returns>
        string GetRoles(string routeUri, string routeTemplate);

        /// <summary>
        /// Method to get role on the basis of role id.
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>Returns string.</returns>
        string GetRole(string roleId, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets role menu permissions with role menu.
        /// </summary>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>Returns string.</returns>
        string GetRolesMenusPermissionsWithRoleMenus(string routeUri, string routeTemplate);

        /// <summary>
        /// Get permission list based on UserName.
        /// </summary>
        /// <param name="userName">is a userName</param>
        /// <param name="routeUri">is request uri</param>
        /// <param name="routeTemplate">is a request template</param>
        /// <returns>Json string with role permission details.</returns>
        string GetRolePermission(string userName, string routeUri, string routeTemplate);
    }
}
