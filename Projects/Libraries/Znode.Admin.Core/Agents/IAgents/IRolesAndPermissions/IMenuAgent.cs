using System.Collections.Generic;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IMenuAgent
    {
        /// <summary>
        /// Method to save menu in database.
        /// </summary>
        /// <param name="model">MenuViewModel model</param>
        /// <param name="message">message</param>
        /// <returns>Returns true/false</returns>
        bool SaveMenu(MenuViewModel model, out string message, out int menuId);

        /// <summary>
        /// Method to get list of all menus along with list of all access permissions.
        /// </summary>
        /// <returns>Returns menu list and access permission list.</returns>
        MenuListViewModel GetMenuList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Method to get list of menus along with access permissions.
        /// </summary>
        /// <returns>Returns list of menus and permissions.</returns>
        MenuListViewModel GetMenuWithPermission();

        /// <summary>
        /// Gets list of submenus along with their permissions.
        /// </summary>
        /// <param name="id">string Parent Menu Id</param>
        /// <param name="preSelectedMenuIds">string of already selected menus.</param>
        /// <returns>Returns list of sub menus.</returns>
        MenuListModel GetSubMenuWithPermission(string id, string preSelectedMenuIds);

        /// <summary>
        /// Deletes menu by menu id.
        /// </summary>
        /// <param name="menuId">Menu Id</param>
        /// <param name="message">message</param>
        /// <returns>Returns true/false.</returns>
        bool DeleteMenu(string menuId, out string message);

        /// <summary>
        /// Update menu by menu id.
        /// </summary>  
        /// <param name="model">MenuViewModel model.</param>
        /// <param name="message">message</param>
        /// <returns>Returns true/false.</returns>
        bool UpdateMenu(MenuViewModel model, out string message);

        /// <summary>
        /// Get menu by menu id.
        /// </summary>
        /// <param name="menuId">Menu Id</param>
        /// <returns>Returns MenuViewModel.</returns>
        MenuViewModel GetMenu(int menuId);

        /// <summary>
        /// Gets list of unselected menus with permissions.
        /// </summary>
        /// <param name="menuIds">string of already selected menus.</param>
        /// <returns>Returns Menu List View Model.</returns>
        MenuListViewModel GetUnSelectedMenusWithPermission(string menuIds);

        /// <summary>
        /// Create multi select drop down.
        /// </summary>
        /// <param name="parentMenu">Collection Parent Menu</param>
        /// <returns>Returns multi select drop down.</returns>
        DropDownOptions CreateMultiSelectDDl(ICollection<MenuViewModel> parentMenu);

        /// <summary>
        /// Get the details required for assigning permissions to menu.
        /// </summary>
        /// <param name="menuId">Id of menu.</param>
        /// <param name="menuName">Name of menu.</param>
        /// <returns>Menu Action Permission View Model</returns>
        MenuActionsPermissionViewModel GetMenuActionsPermissionDetails(int menuId, string menuName);

        /// <summary>
        /// Update the permission values against action.
        /// </summary>
        /// <param name="menuActionsPermissionModel">MenuActionsPermissionModel</param>
        /// <returns>MenuActionsPermissionModel</returns>
        bool UpdateMenuActionPermissions(MenuActionsPermissionViewModel menuActionsPermissionViewModel);

    }
}
