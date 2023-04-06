using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IRoleClient : IBaseClient
    {
        /// <summary>
        /// This function help to create role and save it in to table.
        /// </summary>
        /// <param name="model">Role Model</param>
        /// <returns>Returns last inserted row.</returns>
        RoleModel CreateRole(RoleModel model);

        /// <summary>
        /// This method gets list of all permissions.
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <returns>Returns list of permissions</returns>
        PermissionListModel GetPermissionList(FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// This methods gets list of all permissions.
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>Returns list of permissions.</returns>
        PermissionListModel GetPermissionList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// This methods gets list of all roles.
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <returns>Returns list of roles.</returns>
        RoleListModel GetRoleList(FilterCollection filters, SortCollection sorts);

        /// <summary>
        ///  This methods gets list of all roles.
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of roles.</returns>
        RoleListModel GetRoleList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// This methods deletes role on the basis of role id.
        /// </summary>
        /// <param name="roleIds">Role Id</param>
        /// <returns>Returns true/false.</returns>
        bool DeleteRole(ParameterModel roleIds);

        /// <summary>
        /// This method gets role on the basis of role id.
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <returns>Returns RoleModel.</returns>
        RoleModel GetRole(string roleId);

        /// <summary>
        /// This method gets role on the basis of role id.
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <param name="expands">ExpandCollection expands</param>
        /// <returns>Returns RoleModel.</returns>
        RoleModel GetRole(string roleId, ExpandCollection expands);

        /// <summary>
        /// This method updates role on the basis of role id.
        /// </summary>
        /// <param name="roleId">Role Id</param>
        /// <param name="model">RoleModel</param>
        /// <returns>Returns RoleModel.</returns>
        RoleModel UpdateRole(string roleId, RoleModel model);

        /// <summary>
        /// This method  gets roles, menus and permissions with role menus.
        /// </summary>
        /// <param name="filters">Filter Collection filters.</param>
        /// <returns>Returns RoleMenuListModel</returns>
        RoleMenuListModel GetRolesMenusPermissionsWithRoleMenus(FilterCollection filters);

        /// <summary>
        /// Get list of permission by UserName.
        /// </summary>
        /// <param name="userName">is a UserName </param>
        /// <returns>list of role permission</returns>
        IEnumerable<RolePermissionModel> GetRolePermissionByUserName(string userName);
    }
}
