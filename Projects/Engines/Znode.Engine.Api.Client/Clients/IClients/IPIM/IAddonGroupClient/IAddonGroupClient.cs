using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IAddonGroupClient : IBaseClient
    {
        /// <summary>
        /// Creates an add-on group.
        /// </summary>
        /// <param name="addonGroupModel">Add-on Group Model to be created.</param>
        /// <returns>Created add-on group model.</returns>
        AddonGroupModel CreateAddonGroup(AddonGroupModel addonGroupModel);

        /// <summary>
        /// Gets add-on Group.
        /// </summary>
        /// <param name="filters">Filter for Add-on Group to get the record.</param>
        /// <param name="expands">Expands for add-on group.</param>
        /// <returns>Add-on Group Model.</returns>
        AddonGroupModel GetAddonGroup(FilterCollection filters, ExpandCollection expands);

        /// <summary>
        /// Updates an add-on group.
        /// </summary>
        /// <param name="addonGroupModel">Add-on group model to be updated.</param>
        /// <returns>Updated add-on group model.</returns>
        AddonGroupModel UpdateAddonGroup(AddonGroupModel addonGroupModel);

        /// <summary>
        /// Get list of add-on groups.
        /// </summary>
        /// <param name="expands">Expands for add-on groups.</param>
        /// <param name="filters">Filters for add-on groups.</param>
        /// <param name="sorts">Sorts for add-on groups.</param>
        /// <returns>List of add-on group model.</returns>
        AddonGroupListModel GetAddonGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Addon group list model.
        /// </summary>
        /// <param name="expands">Expands for add-on groups.</param>
        /// <param name="filters">Filters for add-on groups.</param>
        /// <param name="sorts">Sorts for add-on groups.</param>
        /// <param name="pageIndex">Start page index for add-on group list.</param>
        /// <param name="pageSize">Page size of add-on group list.</param>
        /// <returns>List of add-on group model.</returns>
        AddonGroupListModel GetAddonGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Deletes add-on group.
        /// </summary>
        /// <param name="addonGroupIds">Add-on Group IDs to be deleted.</param>
        /// <returns>True if addon group is deleted;False if add-on group fails to delete.</returns>
        bool DeleteAddonGroup(ParameterModel addonGroupIds);

        /// <summary>
        /// Associates products to add-on groups.
        /// </summary>
        /// <param name="addonGroupProducts">Model with addon group and associated products.</param>
        /// <returns>Return true if products are associated to an add-on group.</returns>
        bool AssociateAddonGroupProduct(AddonGroupProductListModel addonGroupProducts);

        /// <summary>
        /// Gets list of associated products to addon group.
        /// </summary>
        /// <param name="addonGroupId">Main addon group ID.</param>
        /// <param name="expands">Expands for list of associated products to addon group.</param>
        /// <param name="filters">Filters for list of associated products to addon group.</param>
        /// <param name="sorts">Sorts for list of associated products to addon group.</param>
        /// <param name="pageIndex">Page index for list of associated products to addon group.</param>
        /// <param name="pageSize">Page size for list of associated products to addon group.</param>
        /// <returns>List of associated products to addon group.</returns>
        ProductDetailsListModel GetAssociatedAddonGroupProductAssociation(int addonGroupId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets list of unassociated products to addon group.
        /// </summary>
        /// <param name="addonGroupId">Main addon group ID.</param>
        /// <param name="expands">Expands for list of associated products to addon group.</param>
        /// <param name="filters">Filters for list of associated products to addon group.</param>
        /// <param name="sorts">Sorts for list of associated products to addon group.</param>
        /// <param name="pageIndex">Page index for list of associated products to addon group.</param>
        /// <param name="pageSize">Page size for list of associated products to addon group.</param>
        /// <returns>List of associated products to addon group.</returns>
        ProductDetailsListModel GetUnassociatedAddonGroupProductAssociation(int addonGroupId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Deletes addon group product association.
        /// </summary>
        /// <param name="addonGroupProductIds">Add-on group product IDs</param>
        /// <returns>Returns true if all selected product association is deleted.</returns>
        bool DeleteAddonGroupProductAssociation(ParameterModel addonGroupProductIds);
    }
}
