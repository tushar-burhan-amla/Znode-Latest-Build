using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IAccessPermissionService
    {
        /// <summary>
        /// Gets account permission list.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sorts.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns account permission list.</returns>
        AccessPermissionListModel AccountPermissionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete account permission.
        /// </summary>
        /// <param name="ids">ids to be deleted.</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteAccountPermission(ParameterModel ids);

        /// <summary>
        /// Create account permission.
        /// </summary>
        /// <param name="accessPermissionModel">model to create.</param>
        /// <returns>Returns created model.</returns>
        AccessPermissionModel CreateAccountPermission(AccessPermissionModel accessPermissionModel);

        /// <summary>
        /// Get account permission.
        /// </summary>
        /// <param name="expands">Expands</param>
        /// <param name="filters">Filters</param>
        /// <returns>Get account permission model.</returns>
        AccessPermissionModel GetAccountPermission(NameValueCollection expands, FilterCollection filters);

        /// <summary>
        /// Update account permission.
        /// </summary>
        /// <param name="accessPermissionModel">model to update.</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool UpdateAccountPermission(AccessPermissionModel accessPermissionModel);

        /// <summary>
        /// Get the access permission list.
        /// </summary>
        /// <param name="expands">Expands for access permission list.</param>
        /// <param name="filters">Filters for access permission list.</param>
        /// <param name="sorts">Sorts for access permission list.</param>
        /// <param name="page">Page collection.</param>
        /// <returns>Returns the access permission list.</returns>
        AccessPermissionListModel AccessPermissionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
    }
}
