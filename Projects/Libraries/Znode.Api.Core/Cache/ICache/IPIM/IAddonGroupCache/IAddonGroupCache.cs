namespace Znode.Engine.Api.Cache
{
    public interface IAddonGroupCache 
    {
        /// <summary>
        /// Gets Add-on Group
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>String data.</returns>
        string GetAddonGroup(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of add-on Groups.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>String data.</returns>
        string GetAddonGroupList(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets list of unassociated product list from addon.
        /// </summary>
        /// <param name="addonGroupId">Main Add-on group ID.</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Response containing unassociated product list.</returns>
        string GetUnassociatedAddonGroupProducts(int addonGroupId, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets list of associated product list from addon.
        /// </summary>
        /// <param name="addonGroupId">Main Add-on group ID.</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Response containing associated product list.</returns>
        string GetAssociatedAddonGroupProducts(int addonGroupId, string routeUri, string routeTemplate);
    }
}
