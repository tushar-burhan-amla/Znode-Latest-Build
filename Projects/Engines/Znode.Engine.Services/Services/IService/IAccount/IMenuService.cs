using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IMenuService
    {
        /// <summary>
        /// Get menu list
        /// </summary>
        /// <param name="expands">Collection of  expands.</param>
        /// <param name="filters">Collection of  filters.</param>
        /// <param name="sorts">Collection of  sorts.</param>
        /// <param name="page">Paging data </param>
        /// <returns></returns>
        MenuListModel GetMenuList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// This method creates menu.
        /// </summary>
        /// <param name="model">MenuModel model.</param>
        /// <returns>Returns last inserted row.</returns>
        MenuModel CreateMenu(MenuModel model);

        /// <summary>
        /// This method gets list of all permissions.
        /// </summary>
        /// <returns>Returns list of permissions.</returns>
        PermissionListModel GetPermissions();

        /// <summary>
        /// This method gets menu list by parent menu id.
        /// </summary>
        /// <param name="parentMenuId">string parent menu id.</param>
        /// <param name="preSelectedMenuIds">string of already selected menu ids.</param>
        /// <returns>Returns list of menus.</returns>
        MenuListModel GetMenusByParentMenuId(string parentMenuId, string preSelectedMenuIds);

        /// <summary>
        /// This method deletes menu by menu id.
        /// </summary>
        /// <param name="menuIds">Menu ids to be deleted.</param>
        /// <returns>Returns true/false.</returns>
        bool DeleteMenu(ParameterModel menuIds);

        /// <summary>
        /// This method updates menu by menu id.
        /// </summary>
        /// <param name="menuId">Menu id</param>
        /// <param name="model">Menu model.</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateMenu(MenuModel model);

        /// <summary>
        /// This method gets menu by menu id.
        /// </summary>
        /// <param name="menuId">Menu id</param>
        /// <returns>Returns MenuModel.</returns>
        MenuModel GetMenu(int menuId);

        /// <summary>
        /// Gets list of all unselected menus.
        /// </summary>
        /// <param name="menuIds">string of already selected menus.</param>
        /// <returns>Returns Menu List Model.</returns>
        MenuListModel GetUnSelectedMenus(ParameterModel menuIds);

        /// <summary>
        /// Get list of all available and selected Permissions for actions of perticular controller.
        /// </summary>
        /// <param name="menuId">Id of menu.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>MenuActionsPermissionModel</returns>
        MenuActionsPermissionModel GetMenuActionsPermissionList(int menuId, NameValueCollection expands);

        /// <summary>
        /// Update the permission values against action.
        /// </summary>
        /// <param name="menuActionsPermissionModel">MenuActionsPermissionModel</param>
        /// <returns>ThemeModel</returns>
        bool UpdateMenuActionPermissions(MenuActionsPermissionModel menuActionsPermissionModel);
    }
}
