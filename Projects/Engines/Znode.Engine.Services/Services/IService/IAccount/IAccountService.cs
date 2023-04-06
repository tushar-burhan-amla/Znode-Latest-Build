using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IAccountService
    {
        AccountListModel GetAccountList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        AccountModel GetAccount(int accountId);

        /// <summary>
        /// Gets the account details by its name
        /// </summary>
        /// <param name="accountName">provided name of the account</param>
        /// <param name="portalId">portalId of the account</param>
        /// <param name="expands">expands for the account to be fetched</param>
        /// <returns>returns the account details based on the name provided</returns>
        AccountModel GetAccountByName(string accountName, NameValueCollection expands, int portalId = 0);

        AccountDataModel CreateAccount(AccountDataModel model);

        bool Update(AccountDataModel model);
        bool Delete(ParameterModel accountIds);

        /// <summary>
        /// Gets the account details by account Code
        /// </summary>
        /// <param name="accountCode">account code</param>
        /// <returns>return the account details based on account code</returns>
        AccountModel GetAccountByCode(string accountCode);

        /// <summary>
        /// Delete account by account Code.
        /// </summary>
        /// <param name="accountCodes">account Codes</param>
        /// <returns>return status</returns>
        bool DeleteAccountByCode(ParameterModel accountCodes);

        #region Account Notes
        /// <summary>
        /// Create Account Note.
        /// </summary>
        /// <param name="noteModel">Account Note Model.</param>
        /// <returns>Returns created Note.</returns>
        NoteModel CreateAccountNote(NoteModel noteModel);

        /// <summary>
        /// Get Account Note on the basis of Note id.
        /// </summary>
        /// <param name="noteId">noteId.</param>
        /// <returns>Returns Note Model.</returns>
        NoteModel GetAccountNote(int noteId);

        /// <summary>
        /// Update Account Note.
        /// </summary>
        /// <param name="noteModel">Note model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateAccountNote(NoteModel noteModel);

        /// <summary>
        /// Get list for Account Note.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Account Note list.</param>
        /// <param name="filters">Filters to be applied on Account Note list.</param>
        /// <param name="sorts">Sorting to be applied on Account Note list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of Account Note.</returns>
        NoteListModel GetAccountNotes(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete Account Note.
        /// </summary>
        /// <param name="noteId">Note Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteAccountNote(ParameterModel noteId);
        #endregion

        #region Account Department
        /// <summary>
        /// Create Account Department.
        /// </summary>
        /// <param name="accountDepartmentModel">Account Department Model.</param>
        /// <returns>Returns created Department.</returns>
        AccountDepartmentModel CreateAccountDepartment(AccountDepartmentModel accountDepartmentModel);

        /// <summary>
        /// Get Account Department on the basis of Department id.
        /// </summary>
        /// <param name="departmentId">departmentId.</param>
        /// <returns>Returns Department Model.</returns>
        AccountDepartmentModel GetAccountDepartment(int departmentId);

        /// <summary>
        /// Update Account Department.
        /// </summary>
        /// <param name="accountDepartmentModel">Department model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateAccountDepartment(AccountDepartmentModel accountDepartmentModel);

        /// <summary>
        /// Get list for Account Department.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Account Department list.</param>
        /// <param name="filters">Filters to be applied on Account Department list.</param>
        /// <param name="sorts">Sorting to be applied on Account Department list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of Account Department.</returns>
        AccountDepartmentListModel GetAccountDepartments(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete Account Department.
        /// </summary>
        /// <param name="departmentId">Department Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteAccountDepartment(ParameterModel departmentId);
        #endregion

        #region Address
        /// <summary>
        /// Get the address list.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="page">Page collection.</param>
        /// <returns>Returns address list.</returns>
        AddressListModel GetAddressList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete account address.
        /// </summary>
        /// <param name="accountAddressId">Account Address Id to delete</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteAccountAddress(ParameterModel accountAddressId);        

        /// <summary>
        /// Update account address.
        /// </summary>
        /// <param name="addressModel">Model to update in db.</param>
        /// <returns>Updated model.</returns>
        bool UpdateAccountAddress(AddressModel addressModel);

        /// <summary>
        /// Create account address.
        /// </summary>
        /// <param name="addressModel">Model to create.</param>
        /// <returns>Returns created model.</returns>
        AddressModel CreateAccountAddress(AddressModel addressModel);

        /// <summary>
        /// Get account address. 
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="expands">Expands.</param>
        /// <returns>Returns account address model.</returns>
        AddressModel GetAccountAddress(FilterCollection filters, NameValueCollection expands);
        #endregion

        #region WebStore Account
        /// <summary>
        /// Create account address from web store.
        /// </summary>
        /// <param name="addressModel">Model to create.</param>
        /// <returns>Returns created model.</returns>
        AddressModel CreateWebStoreAccountAddress(AddressModel addressModel);

        /// <summary>
        /// Get list of User Address for Address Book.
        /// </summary>
        /// <param name="expands">expand for product list.</param>
        /// <param name="filters">filter for product list.</param>
        /// <param name="sorts">sorts for products</param>
        /// <param name="page">paging parameter for product list</param>
        /// <returns>Returns AddressListModel.</returns>
        AddressListModel GetUserAddressList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Address information on the basis of Address Id.
        /// </summary>
        /// <param name="addressId">Address Id whose information has to be fetched.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Returns Address Model containing address information.</returns>
        AddressModel GetAddress(int addressId);

        /// <summary>
        /// Update account address from web store.
        /// </summary>
        /// <param name="addressModel">Model to update.</param>
        /// <returns>Returns updated model.</returns>
        bool UpdateWebStoreAccountAddress(AddressModel addressModel);

        /// <summary>
        /// Delete address on the basis of addressId.
        /// </summary>
        /// <param name="addressId">AddressId.</param>
        /// <param name="userId">UserId.</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteAddress(int? addressId, int? userId);
        #endregion

        #region Associate Price
        /// <summary>
        /// Associate price list to account.
        /// </summary>
        /// <param name="priceAccountModel">priceAccountModel contains data to associate.</param>
        /// <returns>returns true if associated else false.</returns>
        bool AssociatePriceList(PriceAccountModel priceAccountModel);

        /// <summary>
        /// UnAssociate associated price list from account.
        /// </summary>
        /// <param name="priceAccountModel">priceAccountModel contains price list ids to unassociate.</param>
        /// <returns>returns true if unassociated else false.</returns>
        bool UnAssociatePriceList(PriceAccountModel priceAccountModel);

        /// <summary>
        /// Get Associated Price List with Precedence data for Account.
        /// </summary>
        /// <param name="priceAccountModel">PriceAccountModel</param>
        /// <returns>PriceAccountModel</returns>
        PriceAccountModel GetAssociatedPriceListPrecedence(PriceAccountModel priceAccountModel);

        /// <summary>
        /// Update the precedence value for associated price list for Account.
        /// </summary>
        /// <param name="priceAccountModel">PriceAccountModel</param>
        /// <returns>PriceAccountModel</returns>
        bool UpdateAssociatedPriceListPrecedence(PriceAccountModel priceAccountModel);
        #endregion

        #region Account Order
        /// <summary>
        /// Get user order list of account.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>OrdersList Model.</returns>
        OrdersListModel GetAccountUserOrderList(int accountId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
        #endregion

        #region Account Profile

        /// <summary>
        /// Get list of associated/unassociated profiles for account.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with profile.</param>
        /// <param name="filters">Filters to be applied on profile.</param>
        /// <param name="sorts">Sorting to be applied on profile.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of associated/unassociated profile.</returns>
        ProfileListModel GetAssociatedUnAssociatedProfile(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate profiles to account.
        /// </summary>
        /// <param name="profileModel">profileModel contains data to associate.</param>
        /// <returns>returns true if associated else false.</returns>
        bool AssociateProfile(ProfileModel profileModel);

        /// <summary>
        /// UnAssociate associated profiles from account.
        /// </summary>
        /// <param name="profileModel">profileModel contains profile ids to unassociate.</param>
        /// <returns>returns true if unassociated else false.</returns>
        bool UnAssociateProfile(AccountProfileModel profileModel);
        #endregion

        #region Approval Routing
        /// <summary>
        /// Get Level List.
        /// </summary>
        /// <returns>Returns list of approver levels.</returns>
        ApproverLevelListModel Getlevelslist();

        /// <summary>
        /// Create User Approver.
        /// </summary>
        /// <param name="userApproverModel">userApproverModel.</param>
        /// <returns>Return true if approver level created successfully else return false.</returns>
        bool CreateApproverLevel(UserApproverModel userApproverModel);

        /// <summary>
        /// Save permission setting
        /// </summary>
        /// <param name="permissionCodeModel">PermissionCodeModel.</param>
        /// <returns>Returns true if permission settings saved successfully else return false.</returns>
        bool SavePermissionSetting(PermissionCodeModel permissionCodeModel);

        /// <summary>
        /// Delete User Approver.
        /// </summary>
        /// <param name="userApproverId">User Approver Id.</param>
        /// <returns>Returns true if approver level deleted successfully else return false.</returns>
        bool DeleteApproverLevel(ParameterModel userApproverId);
        #endregion

        /// <summary>
        /// Get parent account list
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="page">page</param>
        /// <returns>Return parent account List</returns>
        ParentAccountListModel GetParentAccountList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
    }
}
