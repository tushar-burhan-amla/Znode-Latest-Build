namespace Znode.Engine.Api.Cache
{
    public interface IAccountCache
    {
        #region Account
        /// <summary>
        /// Get company account list.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetAccountList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get company account details.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <param name="accountId"></param>
        /// <returns>Returns company account data</returns>
        string GetAccount(int accountId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get account by account name
        /// </summary>
        /// <param name="accountName">name of the account to be fetched</param>
        /// <param name="portalId">portalId of the account to be fetched</param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>returns the account details based on the account name</returns>
        string GetAccountByName(string accountName, int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get account details.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <param name="accountCode"></param>
        /// <returns>Returns company account data</returns>
        string GetAccountByCode(string accountCode, string routeUri, string routeTemplate);
        #endregion

        #region Account Notes
        /// <summary>
        /// Get Account Note on the basis of Account Note id.
        /// </summary>
        /// <param name="noteId">Note id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Account Note.</returns>
        string GetAccountNote(int noteId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Account Note.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Account Note list.</returns>
        string GetAccountNotes(string routeUri, string routeTemplate);
        #endregion

        #region Account Department
        /// <summary>
        /// Get Account Department on the basis of Account Department id.
        /// </summary>
        /// <param name="departmentId">Department id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Account Department.</returns>
        string GetAccountDepartment(int departmentId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Account AccountDepartments.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns AccountDepartment list.</returns>
        string GetAccountDepartments(string routeUri, string routeTemplate);
        #endregion

        #region Address
        /// <summary>
        /// Gets the account address list.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns the account address list.</returns>
        string GetAddressList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the account address.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns the account address.</returns>
        string GetAccountAddress(string routeUri, string routeTemplate);
        #endregion

        #region Account Order
        /// <summary>
        /// Get the list of all orders of account.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <param name="accountId">Account Id</param>
        /// <returns>list of order in string format by serializing it.</returns>
        string GetAccountUserOrderList(int accountId, string routeUri, string routeTemplate);
        #endregion

        #region Account Profile

        /// <summary>
        /// Get associated/unassociated profiles for account.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns associated/unassociated profiles for account.</returns>
        string GetAssociatedUnAssociatedProfile(string routeUri, string routeTemplate);

        #endregion

        #region Approval Routing
        /// <summary>
        /// Get Level List
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string Getlevelslist(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of user approvers.
        /// </summary>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns list of user approvers.</returns>
        string GetUserApproverList(string routeUri, string routeTemplate);
        #endregion

        /// <summary>
        /// Get parent account list.
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>Return parent account list</returns>
        string GetParentAccountList(string routeUri, string routeTemplate);
    }
}
