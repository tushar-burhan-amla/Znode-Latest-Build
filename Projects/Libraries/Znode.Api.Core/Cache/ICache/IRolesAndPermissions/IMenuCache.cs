namespace Znode.Engine.Api.Cache
{
    public interface IMenuCache
    {
        /// <summary>
        /// Gets menu.
        /// </summary>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>Returns string.</returns>
        string GetMenus(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets list of menus by menu id.
        /// </summary>
        /// <param name="menuId"> Menu id.</param>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>Returns string.</returns>
        string GetMenu(int menuId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of all available and selected Permissions for actions of perticular controller.
        /// </summary>
        /// <param name="menuId">Id of menu.</param>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>Returns string.</returns>
        string GetMenuActionsPermissionList(int menuId, string routeUri, string routeTemplate);
    }
}
