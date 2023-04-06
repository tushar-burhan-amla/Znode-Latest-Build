using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IAccessPermissionAgent
    {
        /// <summary>
        /// Gets account permission list.
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="sortCollection">Sorts.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="recordPerPage">Record per page.</param>
        /// <returns>Returns account permission list.</returns>
        AccountPermissionListViewModel AccountPermissionList(int accountId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Gets the access permission list.
        /// </summary>
        /// <returns>Returns the access permission list.</returns>
        List<SelectListItem> AccessPermissionList();

        /// <summary>
        /// Gets the account permission.
        /// </summary>
        /// <param name="accountPermissionId">Account permission id.</param>
        /// <returns>Returns account permission.</returns>
        AccountPermissionViewModel GetAccountPermission(int accountPermissionId);

        /// <summary>
        /// Create account permission.
        /// </summary>
        /// <param name="model">Model to create.</param>
        /// <returns>Returns true if created successfully.</returns>
        bool CreateAccountPermission(AccountPermissionViewModel model);

        /// <summary>
        /// Update account permission.
        /// </summary>
        /// <param name="data">account permission details as json format.</param>
        /// <returns>Returns account permission view model.</returns>
        AccountPermissionViewModel UpdateAccountPermission(string data);

        /// <summary>
        /// Delete the account permissions.
        /// </summary>
        /// <param name="accountPermissionIds">Ids of account permission to delete.</param>
        /// <returns>Return true if deleted successfully.</returns>
        bool DeleteAccountPermission(string accountPermissionIds);

        /// <summary>
        /// Check Whether Permission is already exists.
        /// </summary>
        /// <param name="accountPermissionName">accountPermissionName to check.</param>
        /// <param name="accountId">account id</param>
        /// <param name="accountPermissionId">id for the account permission</param>
        /// <returns>return the status in true or false</returns>
        bool CheckAccountPermissionExist(string accountPermissionName, int accountId, int accountPermissionId);
    }
}
