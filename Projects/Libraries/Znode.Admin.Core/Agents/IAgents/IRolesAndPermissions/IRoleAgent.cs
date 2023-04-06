using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IRoleAgent
    {
        /// <summary>
        /// This function help to create role and save it in to table.
        /// </summary>
        /// <param name="model">Role View Model.</param>
        /// <returns>Returns model for last created role.</returns>
        RoleViewModel SaveRole(RoleViewModel model);

        /// <summary>
        /// This function gets list of roles.
        /// </summary>
        /// <returns>Returns list of roles.</returns>
        RoleListViewModel GetRoleList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// This function deletes role on the basis of role id.
        /// </summary>
        /// <param name="roleId">Role id.</param>
        /// <param name="message">message</param>
        /// <returns>Returns true/false</returns>
        bool DeleteRole(string roleId, out string message);

        /// <summary>
        /// This function gets role on the basis of role id.
        /// </summary>
        /// <param name="roleId">Role id.</param>
        /// <returns>Returns RoleViewModel</returns>
        RoleViewModel GetRole(string roleId);

        /// <summary>
        /// This function updates role on the basis of role id.
        /// </summary>
        /// <param name="model">Role view model</param>
        /// <param name="message">message</param>
        /// <returns>Returns true/false</returns>
        bool UpdateRole(RoleViewModel model, out string message);

        /// <summary>
        /// Gets role menu permissions with role menu.
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <param name="filters">Filter Collection</param>
        /// <returns>Returns RoleMenuListViewModel</returns>
        RoleMenuListViewModel GetRolesMenusPermissionsWithRoleMenus(string roleId, FilterCollection filters = null);

        /// <summary>
        /// Get list of role list with SelectListItem.
        /// </summary>
        /// <param name="filters">Filters for Roles.</param>
        /// <param name="sortCollection">Sorts for Roles.</param>
        /// <returns>Returns list of Roles.</returns>
        List<SelectListItem> RoleList(FilterCollection filters = null, SortCollection sortCollection = null);

        /// <summary>
        /// Get permission list based on userName.
        /// </summary>
        /// <param name="userName">is a userName</param>
        /// <returns>returns list of permission</returns>
        IEnumerable<RolePermissionViewModel> GetRolePermission(string userName);

        /// <summary>
        /// Gets the roles for admin.
        /// </summary>
        /// <returns>Returns roles for admin.</returns>
        List<SelectListItem> GetAdminRoles();

        /// <summary>
        /// Check Whether the Role Name is already exists.
        /// </summary>
        /// <param name="roleName">is a roleName</param>
        /// <param name="roleId">id for the Role</param>
        /// <returns>return the status in true or false</returns>
        bool CheckRoleNameExist(string roleName, string roleId);
    }
}
