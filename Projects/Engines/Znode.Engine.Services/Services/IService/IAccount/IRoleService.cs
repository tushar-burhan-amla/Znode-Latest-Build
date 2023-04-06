using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IRoleService
    {
        /// <summary>
        /// This function help to create role and save it in to table.
        /// </summary>
        /// <param name="model">Role Model</param>
        /// <returns>Returns last created row.</returns>
        RoleModel CreateRole(RoleModel model);

        /// <summary>
        /// This method gets list of all permissions.
        /// </summary>
        /// <param name="filters">Filter tuple to get permissions.</param>
        /// <returns>Returns list of permissions.</returns>
        PermissionListModel GetPermissions(FilterCollection filters);

        /// <summary>
        /// This method gets list of all roles.
        /// </summary>
        /// <returns>Returns list of roles.</returns>
        RoleListModel GetRoles(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete role on the basis of role id.
        /// </summary>
        /// <param name="roleIds">Role ids to be deleted.</param>
        /// <returns>Returns true/false.</returns>
        bool DeleteRole(ParameterModel roleIds);

        /// <summary>
        /// Gets role on the basis of role id.
        /// </summary>
        /// <param name="roleId">Role id.</param>
        /// <returns>Returns RoleModel.</returns>
        RoleModel GetRole(string roleId);

        /// <summary>
        /// Updates role on the basis of role id.
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <param name="model">RoleModel</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateRole(RoleModel model);

        /// <summary>
        /// Get role menu permissions with role menu.
        /// </summary>
        /// <param name="filters">Filter tuple</param>
        /// <returns>Returns RoleMenuListModel</returns>
        RoleMenuListModel GetRolesMenusPermissionsWithRoleMenus(FilterCollection filters);

        /// <summary>
        /// Manages role menu and access permission.
        /// </summary>
        /// <param name="data">List<DataObjectModel> data</param>
        /// <returns>Returns true/false</returns>
        bool EditManagedRolePermissions(List<DataObjectModel> data, string roleId);

        /// <summary>
        /// Get Role permission details.
        /// </summary>
        /// <param name="userName">is a user Name</param>
        /// <returns>list of role permission</returns>
        IEnumerable<RolePermissionModel> GetPermissionListByUserName(string userName);
    }
}
