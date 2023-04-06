using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IMenuClient : IBaseClient
    {
        /// <summary>
        /// This function help to create Application menu and save it in to table.
        /// </summary>
        /// <param name="model">Menu Model</param>
        /// <returns>Returns Last inserted Row</returns>
        MenuModel CreateMenu(MenuModel model);

        /// <summary>
        /// This function help to get all menus in the form of list.
        /// </summary>
        /// <param name="filters">Filters for where condition</param>
        /// <param name="sorts">Sort order</param>
        /// <returns>Returns Menu List</returns>
        MenuListModel GetMenuList(FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// This function help to get all menus in the form of list with pagenation.
        /// </summary>
        /// <param name="filters">Filters for where condition</param>
        /// <param name="sorts">Sort order</param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Record per page</param>
        /// <returns>Gets all menu list.</returns>
        MenuListModel GetMenuList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// This function gets list of menus on the basis of parent menu id.
        /// </summary>
        /// <param name="model">Parameter Model</param>
        /// <param name="preSelectedMenuIds">Already selected menu ids.</param>
        /// <returns>Returns MenuListModel</returns>
        MenuListModel GetMenuListByParentMenuId(ParameterModel model, string preSelectedMenuIds);

        /// <summary>
        /// This function helps to delete menus by menu id.
        /// </summary>
        /// <param name="menuIds">Menu ids</param>
        /// <returns>Returns true/false.</returns>
        bool DeleteMenu(ParameterModel menuIds);

        /// <summary>
        /// This function helps to update a menu by menu id.
        /// </summary>
        /// <param name="menuId">Menu id.</param>
        /// <param name="model">Menu Model.</param>
        /// <returns>Returns updated menu.</returns>
        MenuModel UpdateMenu(int menuId, MenuModel model);

        /// <summary>
        /// This function gets menu by menu id.
        /// </summary>
        /// <param name="menuId">Menu id.</param>
        /// <returns>Returns menu model.</returns>
        MenuModel GetMenu(int menuId);

        /// <summary>
        /// This function gets menu by menu id.
        /// </summary>
        /// <param name="menuId">Menu id.</param>
        /// <param name="expands">Expands.</param>
        /// <returns>Returns menu model.</returns>
        MenuModel GetMenu(int menuId, ExpandCollection expands);

        /// <summary>
        /// Gets list of unselected menus apart from menu ids already existing.
        /// </summary>
        /// <param name="menuIds">Comma separated menu ids if any</param>
        /// <returns>Returns Menu list model.</returns>
        MenuListModel GetUnSelectedMenus(ParameterModel menuIds);

        /// <summary>
        /// Get list of all available and selected Permissions for actions of perticular controller.
        /// </summary>
        /// <param name="menuId">Id of menu.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>MenuActionsPermissionModel</returns>
        MenuActionsPermissionModel GetMenuActionsPermissionList(int menuId, ExpandCollection expands);

        /// <summary>
        /// Update the permission values against action.
        /// </summary>
        /// <param name="menuActionsPermissionModel">MenuActionsPermissionModel</param>
        /// <returns>MenuActionsPermissionModel</returns>
        MenuActionsPermissionModel UpdateActionPermissions(MenuActionsPermissionModel menuActionsPermissionModel);
    }
}
