using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IAccessPermissionClient : IBaseClient
    {
        /// <summary>
        /// Gets account permission list.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sorts.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns account permission list.</returns>
        AccessPermissionListModel AccountPermissionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Delete account permission.
        /// </summary>
        /// <param name="attributeGroupIds">ids to be deleted.</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteAccountPermission(ParameterModel attributeGroupIds);

        /// <summary>
        /// Create account permission.
        /// </summary>
        /// <param name="model">model to create.</param>
        /// <returns>Returns created model.</returns>
        AccessPermissionModel CreateAccountPermission(AccessPermissionModel model);

        /// <summary>
        /// Get account permission.
        /// </summary>
        /// <param name="expands">Expands</param>
        /// <param name="filters">Filters</param>
        /// <returns>Get account permission model.</returns>
        AccessPermissionModel GetAccountPermission(ExpandCollection expands, FilterCollection filters);

        /// <summary>
        /// Update account permission.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns true if updated successfully.</returns>
        AccessPermissionModel UpdateAccountPermission(AccessPermissionModel model);

        /// <summary>
        /// Gets the access permission list.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Returns the access permission list.</returns>
        AccessPermissionListModel AccessPermissionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
    }
}
