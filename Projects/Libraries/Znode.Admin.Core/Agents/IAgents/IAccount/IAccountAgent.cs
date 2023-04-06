using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IAccountAgent
    {
        /// <summary>
        /// Get Account List.
        /// </summary>
        /// <param name="filters">Filter Collection Parameter</param>
        /// <param name="sortCollection">Sort Collection Parameter</param>
        /// <param name="pageIndex">Start Index of Page</param>
        /// <param name="recordPerPage">Records Per Page.</param>
        /// <returns>Return Account List in AccountListViewModel model.</returns>
        AccountListViewModel GetAccountList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get Sub Account List.
        /// </summary>
        /// <param name="parentAccountId">Id of the Parent Account</param>
        /// <param name="filters">Filter Collection Parameter</param>
        /// <param name="sortCollection">Sort Collection Parameter</param>
        /// <param name="pageIndex">Start Index of Page</param>
        /// <param name="recordPerPage">Records Per Page.</param>
        /// <returns>Return Account List in AccountListViewModel model.</returns>
        AccountListViewModel GetSubAccountList(int parentAccountId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);


        /// <summary>
        /// Get parent account list.
        /// </summary>
        /// <param name="portalId">If portal id is greater ,get parent account list based on portal id.</param>
        /// <returns></returns>
        List<SelectListItem> GetParentAccountList(int portalId = 0);

        /// <summary>
        /// Gets tab view list model.
        /// </summary>
        /// <param name="accountId">Account Id.</param>
        /// <returns></returns>
        AccountDataViewModel GetAccountById(int accountId);

        /// <summary>
        /// Create New Account.
        /// </summary>
        /// <param name="model">Model of Type AccountDataViewModel</param>
        /// <returns>Return the Account Details in AccountDataViewModel Format.</returns>
        AccountDataViewModel Create(AccountDataViewModel model);

        /// <summary>
        /// Update the Account Details.
        /// </summary>
        /// <param name="model">Model of Type AccountDataViewModel</param>
        /// <returns>Return AccountDataViewModel</returns>
        AccountDataViewModel UpdateAccount(AccountDataViewModel model);

        /// <summary>
        /// Delete the Account
        /// </summary>
        /// <param name="accountIds">Ids for the account.</param>
        /// <param name="message">.Message regarding the status</param>
        /// <returns>Return true or false.</returns>
        bool DeleteAccount(string accountIds, out string message);

        /// <summary>
        /// Get customer account list.
        /// </summary>
        /// <param name="currentUserName">User name of logged in user.</param>
        /// <param name="filters">Filters for Account.</param>
        /// <param name="sorts">Sorts for Account.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns AccountListModel.</returns>
        CustomerListViewModel GetCustomerAccountList(string currentUserName, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Set the AccountDataModel.
        /// </summary>
        /// <param name="parentAccountId">Parent account id.</param>
        /// <param name="model">Model to set the data.</param>
        void SetAccountDataModel(int parentAccountId, AccountDataViewModel model);

        /// <summary>
        /// Check whether account name exits or not.
        /// </summary>
        /// <param name="accountName">userName to check.</param>
        /// <param name="parentAccountId">accountId.</param>
        /// <param name="portalId">portalId.</param>
        /// <returns></returns>
        bool CheckAccountNameExist(string accountName, int parentAccountId, int portalId);

        #region Account Note
        /// <summary>
        /// Add Account Note.
        /// </summary>
        /// <param name="noteViewModel">NoteViewModel to create Account Note.</param>
        /// <returns>Added Account Note.</returns>
        NoteViewModel CreateAccountNote(NoteViewModel noteViewModel);

        /// <summary>
        /// Get Account Note list from database.
        /// </summary>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>Returns NoteListViewModel</returns>
        NoteListViewModel GetAccountNotes(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get Account Note by NoteId.
        /// </summary>
        /// <param name="noteId">Get Account Note on the basis of noteId.</param>
        /// <returns>Returns NoteViewModel.</returns>
        NoteViewModel GetAccountNote(int noteId);

        /// <summary>
        /// Update Account Note.
        /// </summary>
        /// <param name="noteViewModel">NoteViewModel.</param>
        /// <returns>Returns NoteViewModel.</returns>
        NoteViewModel UpdateAccountNote(NoteViewModel noteViewModel);

        /// <summary>
        /// Delete Account Note on the basis of noteIds.
        /// </summary>
        /// <param name="noteIds">Note Ids to delete account note.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteAccountNote(string noteIds, out string message);
        #endregion

        #region Account Department
        /// <summary>
        /// Add Account Department.
        /// </summary>
        /// <param name="departmentViewModel">AccountDepartmentViewModel to create Account Department.</param>
        /// <returns>Added Account Department view model.</returns>
        AccountDepartmentViewModel CreateAccountDepartment(AccountDepartmentViewModel departmentViewModel);

        /// <summary>
        /// Get Account Department list from database.
        /// </summary>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>Returns AccountDepartmentListViewModel</returns>
        AccountDepartmentListViewModel GetAccountDepartments(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get Account Department by DepartmentId.
        /// </summary>
        /// <param name="departmentId">Get Account Department on the basis of departmentId.</param>
        /// <returns>Returns AccountDepartmentViewModel.</returns>
        AccountDepartmentViewModel GetAccountDepartment(int departmentId, int accountId);

        /// <summary>
        /// Update Account Department.
        /// </summary>
        /// <param name="departmentViewModel">AccountDepartmentViewModel.</param>
        /// <returns>Returns AccountDepartmentViewModel.</returns>
        bool UpdateAccountDepartment(AccountDepartmentViewModel departmentViewModel);

        /// <summary>
        /// Delete Account departmentViewModel on the basis of departmentIds.
        /// </summary>
        /// <param name="departmentIds">departmentIds to delete account department.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteAccountDepartment(string departmentId, out string message);

        /// <summary>
        /// Sets filter for user id.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="accountId">accountId.</param>
        void SetFiltersForAccountId(FilterCollection filters, int? accountId);

        /// <summary>
        /// Set the customer account view model.
        /// </summary>
        /// <param name="model">Model to set the values.</param>
        /// <param name="accountId">Account Id to get the list.</param>
        void SetCustomerAccountViewModel(CustomerAccountViewModel model, int accountId);

        /// <summary>
        /// Get the details of accounts customer.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Returns details of customer.</returns>
        CustomerAccountViewModel GetAccountsCustomer(int userId);

        /// <summary>
        /// Create new account's customer.
        /// </summary>
        /// <param name="customerViewModel">Customer Account View Model</param>
        /// <returns>Customer Account View Model.</returns>
        CustomerAccountViewModel CreateCustomerAccount(CustomerAccountViewModel customerViewModel);

        /// <summary>
        /// Update the accounts customer.
        /// </summary>
        /// <param name="model">Model to update in database.</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool UpdateCustomerAccount(CustomerAccountViewModel model, out string errorMessage);

        /// <summary>
        /// Get UnAssociated Customer(s).
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <returns>Return all customer list</returns>
        CustomerListViewModel GetUnAssociatedCustomerList(int portalId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);


        /// <summary>
        /// Update user account mapping
        /// </summary>
        /// <param name="userModel">Model to update in database</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool UpdateUserAccountMapping(UserAccountViewModel userModel, out string errorMessage);
        #endregion

        #region Address
        /// <summary>
        /// Get the address list.
        /// </summary>
        /// <param name="accountId">Account id to get the list.</param>
        /// <param name="sortCollection">Sort.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="recordPerPage">Record per page.</param>
        /// <returns>Returns address list.</returns>
        AddressListViewModel GetAddressList(int accountId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Create account address.
        /// </summary>
        /// <param name="addressViewModel">Model to create.</param>
        /// <returns>Returns created model.</returns>
        AddressViewModel CreateAccountAddress(AddressViewModel addressViewModel);

        /// <summary>
        /// Get account address. 
        /// </summary>
        /// <param name="accountAddressId">Account address id to get.</param>
        /// <returns>Returns account address model.</returns>
        AddressViewModel GetAccountAddress(int accountAddressId);

        /// <summary>
        /// Update account address.
        /// </summary>
        /// <param name="addressViewModel">Model to update in db.</param>
        /// <returns>Updated model.</returns>
        AddressViewModel UpdateAccountAddress(AddressViewModel addressViewModel);

        /// <summary>
        /// Delete account address.
        /// </summary>
        /// <param name="accountAddressId">Account Address Id to delete</param>
        /// <param name="message"></param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteAccountAddress(string accountAddressId, out string message);
        #endregion

        #region Associate Price
        /// <summary>
        /// Get list for Price for Account.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Price list.</param>
        /// <param name="filters">Filters to be applied on Price list.</param>
        /// <param name="sorts">Sorting to be applied on Price list.</param>
        /// <param name="pageIndex">Start page index of Price list.</param>
        /// <param name="pageSize">Page size of Price list.</param>
        /// <param name="accountId">accountId to get price list associated to this account.</param>
        /// <returns>Returns Price list.</returns>
        PriceListViewModel GetPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int accountId);

        /// <summary>
        /// Get unassociated price list for Account.
        /// </summary>
        /// <param name="expands">Expands for Price List.</param>
        /// <param name="filters">Filters for Price List.</param>
        /// <param name="sorts">Sorts for Price List.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="recordPerPage">Size of page.</param>
        /// <returns>Returns unassociated price list.</returns>
        PriceListViewModel GetUnAssociatedPriceList(int accountId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Associate pricelist to account.
        /// </summary>
        /// <param name="accountId">Account Id to which pricelist to be associated.</param>
        /// <param name="priceListIds">priceListIds to be associated.</param>
        /// <returns>Returns true if pricelist associated successfully else return false.</returns>
        bool AssociatePriceList(int accountId, string priceListId);

        /// <summary>
        /// Remove associated price list from Account.
        /// </summary>
        /// <param name="priceListIds">price list ids associate to account.</param>
        /// <param name="accountId">Account Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool UnAssociatedPriceList(string priceListIds, int accountId);

        /// <summary>
        /// Get associated price list precedence value for Account.
        /// </summary>
        /// <param name="priceListId">priceListId.</param>
        /// <param name="accountId">accountId to get precedence for Account.</param>
        /// <param name="listName">price list name.</param>
        /// <returns>Returns PriceAccountViewModel.</returns>
        PriceAccountViewModel GetAssociatedPriceListPrecedence(int priceListId, int accountId, string listName);

        /// <summary>
        /// Update associated price list precedence value for Account.
        /// </summary>
        /// <param name="priceAccountViewModel">PriceAccountViewModel model.</param>
        /// <returns>Returns true/false status.</returns>
        bool UpdateAssociatedPriceListPrecedence(PriceAccountViewModel priceAccountViewModel);
        #endregion

        #region Account Order

        /// <summary>
        /// Get order details by order id.
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sortCollection">SortCollection</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="recordPerPage">record per page.</param>
        /// <param name="accountId">Account Id</param>
        /// <returns>OrdersListViewModel</returns>
        OrdersListViewModel GetAccountUserOrderList(int accountId, int userId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);
        #endregion

        #region Account Profile
        /// <summary>
        /// Get list for profiles for Account.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with profile.</param>
        /// <param name="filters">Filters to be applied on profile.</param>
        /// <param name="sorts">Sorting to be applied on profile.</param>
        /// <param name="pageIndex">Start page index of profile.</param>
        /// <param name="pageSize">Page size of profile.</param>
        /// <param name="accountId">accountId to get profile associated to this account.</param>
        /// <returns>Returns profile list.</returns>
        ProfileListViewModel GetAssociatedProfile(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int accountId);

        /// <summary>
        /// Get unassociated profile for Account.
        /// </summary>
        /// <param name="expands">Expands for profile.</param>
        /// <param name="filters">Filters for profile.</param>
        /// <param name="sorts">Sorts for profile.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="recordPerPage">Size of page.</param>
        /// <returns>Returns unassociated profiles.</returns>
        ProfileListViewModel GetUnAssociatedProfile(int accountId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Associate profiles to account.
        /// </summary>
        /// <param name="accountId">Account Id to which profiles to be associated.</param>
        /// <param name="profileIds">profileIds to be associated.</param>
        /// <returns>Returns true if profiles associated successfully else return false.</returns>
        bool AssociateProfile(int accountId, string profileIds);

        /// <summary>
        /// Remove associated profiles from Account.
        /// </summary>
        /// <param name="profileIds">profile ids associate to account.</param>
        /// <param name="accountId">Account Id.</param>
        /// <param name="message">Set error message.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool UnAssociateProfile(string profileIds, int accountId, out string message);

        /// <summary>
        /// Set default profile for account.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="accountProfileId">accountProfileId</param>
        /// <param name="profileId">profileId</param>
        /// <param name="errorMessage">set error message</param>
        /// <returns></returns>
        bool SetDefaultProfile(int accountId, int accountProfileId, int profileId, out string errorMessage);
        #endregion

        /// <summary>
        /// Get Approver Level List.
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns>Return approval level list.</returns>
        UserApproverListViewModel GetApproverLevelList(int userId);

        /// <summary>
        /// Create and update approver level.
        /// </summary>
        /// <param name="model">Model of Type User Approver View Model</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Return true or false</returns>
        bool CreateApproverLevel(UserApproverViewModel model, out string errorMessage);

        /// <summary>
        /// Delete the approver level by id 
        /// </summary>
        /// <param name="approverLevelId">Ids for the approver.</param>
        /// <returns>Return true or false.</returns>
        bool DeleteApproverLevelById(string approverLevelId);

        /// <summary>
        /// Get levels list.
        /// </summary>
        /// <returns></returns>
        UserApproverViewModel GetLevelList();

        /// <summary>
        /// Get users list by search Key. 
        /// </summary>
        /// <param name="searchTerm">Search value to get in list.</param>
        /// <param name="portalId">accountId</param>
        /// <param name="accountId">accountId</param>
        /// <param name="userId">userId</param>
        /// <returns>Returns users name list.</returns>
        List<UserApproverViewModel> GetApproverUsersByName(string searchTerm, int? portalId, int? accountId, int? userId, string approvalUserIds);

        /// <summary>
        /// Save permission code according to that user
        /// </summary>
        /// <param name="model">save permission setting</param>
        PermissionCodeViewModel SavePermissionSetting(PermissionCodeViewModel model);

        /// <summary>
        /// Get parent account list.
        /// </summary>
        /// <param name="filters">filters</param>
        /// <param name="sortCollection">sortCollection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="recordPerPage">recordPerPage</param>
        /// <returns>Return parent account list</returns>
        ParentAccountListViewModel GetParentAccountList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Set the portal id filter in filter list
        /// </summary>
        /// <param name="filters">filters</param>
        /// <param name="portalId">portalId</param>
        void SetPortalIdFilter(FilterCollection filters, int portalId);
    }
}
