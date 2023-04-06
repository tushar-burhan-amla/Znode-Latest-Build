using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IAddonGroupAgent
    {
        /// <summary>
        /// Creates an add-on group.
        /// </summary>
        /// <param name="addonGroup">Add-on group to be created.</param>
        /// <returns>Created add-on group.</returns>
        AddonGroupViewModel CreateAddonGroup(AddonGroupViewModel addonGroup);

        /// <summary>
        /// Gets an add-on group.
        /// </summary>
        /// <param name="addonGroupId">Add-on Group ID to get the record.</param>
        /// <returns>Add-on Group View model.</returns>
        AddonGroupViewModel GetAddonGroup(int addonGroupId);

        /// <summary>
        /// Updates an add-on group.
        /// </summary>
        /// <param name="addonGroup">Addon group to be updated.</param>
        /// <returns>Updated add-on group.</returns>
        AddonGroupViewModel UpdateAddonGroup(AddonGroupViewModel addonGroup);

        /// <summary>
        /// Get list of add-on group list.
        /// </summary>
        /// <param name="filters">Filter collection for add-on groups.</param>
        /// <param name="sortCollection">Sort collection for add-on group.</param>
        /// <param name="expands">Expands for add-on groups.</param>
        /// <param name="pageIndex">Page index for add-on group.</param>
        /// <param name="pageSize">Page size for add-on group.</param>
        /// <returns>List of add-on groups.</returns>
        AddonGroupListViewModel GetAddonGroupList(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? pageSize = null);

        /// <summary>
        /// Deletes add-on groups.
        /// </summary>
        /// <param name="addonGroupIds">Add-on group IDs to be deleted.</param>
        /// <param name="errorMessag">Error message assigned according to error code.</param>
        /// <returns>True if add-on group is deleted;False if add-on group fails to delete.</returns>
        bool DeleteAddonGroup(string addonGroupIds, out string errorMessag);

        /// <summary>
        /// Add-on Group to get current locale..
        /// </summary>
        /// <param name=""></param>
        /// <returns>Add-on Group to get current locale.</returns>
        int GetLocaleValueForAddon();

        /// <summary>
        /// Associated selected products to addon group.
        /// </summary>
        /// <param name="addonGroupId">Parent add-on group ID.</param>
        /// <param name="associatedProductIds">Associated product IDs.</param>
        /// <returns>Returns true if all the products are associated successfully.</returns>
        bool AssociateAddonGroupProduct(int addonGroupId, string associatedProductIds);

        /// <summary>
        /// Gets list of associated products to addon group.
        /// </summary>
        /// <param name="addonGroupId">Main addon group ID.</param>
        /// <param name="sortCollection">Sort collection for list of associated products to addon group</param>
        /// <param name="expands">Expands for list of associated products to addon group</param>
        /// <param name="filters">Filters for list of associated products to addon group</param>
        /// <param name="pageIndex">Page index for list of associated products to addon group</param>
        /// <param name="pageSize">Page size for list of associated products to addon group</param>
        /// <returns>List of associated products to addon group</returns>
        ProductDetailsListViewModel GetAssociatedAddonGroupProduct(int addonGroupId, int localeId, SortCollection sortCollection = null, ExpandCollection expands = null, FilterCollection filters = null, int? pageIndex = null, int? pageSize = null);

        /// <summary>
        /// Gets list of unassociated products for the addon group.
        /// </summary>
        /// <param name="addonGroupId">Addon group ID for which unassociated products will be shown.</param>
        /// <param name="localeId">Locale ID of products to be associated.</param>
        /// <param name="sortCollection">Sort for list of unassociated products for the addon group</param>
        /// <param name="expands">Expands for list of unassociated products for the addon group</param>
        /// <param name="filters">Filters for list of unassociated products for the addon group</param>
        /// <param name="pageIndex">Page index for list of unassociated products for the addon group</param>
        /// <param name="pageSize">Page size for list of unassociated products for the addon group</param>
        /// <returns>List of unassociated products for the addon group</returns>
        ProductDetailsListViewModel GetUnassociatedAddonGroupProduct(int addonGroupId,int localeId, SortCollection sortCollection = null, ExpandCollection expands = null, FilterCollection filters = null, int? pageIndex = null, int? pageSize = null);

        /// <summary>
        /// Deletes addon group and product relation.
        /// </summary>
        /// <param name="addonGroupProductIds">Addon group product IDs</param>
        /// <param name="errorMessage">Error message set according to exception.</param>
        /// <returns>True if addon group product association is removed.</returns>
        bool DeleteAddonGroupProducts(string addonGroupProductIds, out string errorMessage);
    }
}
