using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IAddonGroupService
    {

        /// <summary>
        /// Creates an add-on Group.
        /// </summary>
        /// <param name="addonGroup">Add-on Group to be created.</param>
        /// <returns>Add-on Group created.</returns>
        AddonGroupModel CreateAddonGroup(AddonGroupModel addonGroup);

        /// <summary>
        /// Gets Add-on Group according to add-on group ID.
        /// </summary>
        /// <param name="filters">Filters for Add-on Group to get the record.</param>
        /// <param name="expands">Expands for add-on Group.</param>
        /// <returns>Add-on Group  model.</returns>
        AddonGroupModel GetAddonGroup(FilterCollection filters, NameValueCollection expands);

        /// <summary>
        /// Updates an add-on group.
        /// </summary>
        /// <param name="addonGroup">Add-on group to be updated.</param>
        /// <returns>True if Add-on Group is updated; False if Add-on Group fails to update.</returns>
        bool UpdateAddonGroup(AddonGroupModel addonGroup);

        /// <summary>
        /// Get list of add-on group.
        /// </summary>
        /// <param name="expands">Expands for add-on group list.</param>
        /// <param name="filters">Filters for add-on group list.</param>
        /// <param name="sorts">Sorts for add-on group list.</param>
        /// <param name="page">Pagination for add-on group list.</param>
        /// <returns>List of add-on groups.</returns>
        AddonGroupListModel GetAddonGroupList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Deletes add-on group ID
        /// </summary>
        /// <param name="addonGroupIds">Add-on group ID to be deleted.</param>
        /// <returns>True if add-on group is deleted;False if add-on group fails to delete.</returns>
        bool DeleteAddonGroup(ParameterModel addonGroupIds);

        /// <summary>
        /// Creates association of addon groups and its associated products.
        /// </summary>
        /// <param name="addonGroupProducts">Addon group product model containing Addon groups and its associated products.</param>
        /// <returns>Returns true if all the addon group products are associated successfully.</returns>
        bool AssociateAddonGroupProduct(AddonGroupProductListModel addonGroupProducts);

        /// <summary>
        /// Gets list of unassociated products to addon group.
        /// </summary>
        /// <param name="addonGroupId">Main addon group ID.</param>
        /// <param name="expands">Expands required for unassociated products to addon group.</param>
        /// <param name="filters">Filters required for unassociated products to addon group.</param>
        /// <param name="sorts">Sorts required for unassociated products to addon group.</param>
        /// <param name="page">Page parameters required for unassociated products to addon group.</param>
        /// <returns>List of unassociated products to addon group.</returns>
        ProductDetailsListModel GetUnassociatedAddonGroupProducts(int addonGroupId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets list of associated product to addon group.
        /// </summary>
        /// <param name="addonGroupId">Main addon group ID.</param>
        /// <param name="expands">Expands required for unassociated products to addon group.</param>
        /// <param name="filters">Filters required for unassociated products to addon group.</param>
        /// <param name="sorts">Sorts required for unassociated products to addon group.</param>
        /// <param name="page">Page parameters required for unassociated products to addon group.</param>
        /// <returns>List of associated products to addon group.</returns>
        ProductDetailsListModel GetAssociatedProducts(int addonGroupId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete addon groups and products association.
        /// </summary>
        /// <param name="addonGroupProductIds">Addon group product associated IDs.</param>
        /// <returns>Returns true if selected values are deleted successfully.</returns>
        bool DeleteAddonGroupProductAssociation(ParameterModel addonGroupProductIds);
    }
}
