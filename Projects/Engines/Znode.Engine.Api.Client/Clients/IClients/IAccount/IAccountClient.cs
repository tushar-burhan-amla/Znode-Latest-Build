using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IAccountClient : IBaseClient
    {
        /// <summary>
        /// Get CompanyAccount List
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        AccountListModel GetAccountList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Account Details by Id.
        /// </summary>
        /// <param name="accountId">Id for the Account</param>
        /// <returns>Return Account Details in AccountModel Format.</returns>
        AccountModel GetAccount(int accountId);
        /// <summary>
        /// Create Company Account
        /// </summary>
        /// <param name="attributeModel">model with compoany and address details</param>
        /// <returns></returns>
        AccountDataModel Create(AccountDataModel companyModel);

        /// <summary>
        /// Gets Account details by name
        /// </summary>
        /// <param name="accountName">name of the account to be fetched</param>
        /// <param name="portalId">portalId of the account to be fetched</param>
        /// <param name="expands">expands for the account to be fetched</param>
        /// <returns>returns the fetched account model</returns>
        AccountModel GetAccountByName(string accountName, ExpandCollection expands, int portalId = 0);

        /// <summary>
        /// Update Company Account 
        /// </summary>
        /// <param name="companyModel"></param>
        /// <returns></returns>
        AccountDataModel UpdateAccount(AccountDataModel companyModel);

        /// <summary>
        /// Delete Account.
        /// </summary>
        /// <param name="accountId">Acccount Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteAccount(ParameterModel accountId);

        #region Account Notes
        /// <summary>
        /// Get List of Account Notes.
        /// </summary>
        /// <param name="expands">collection of expands.</param>
        /// <param name="filters">collection of filter.</param>
        /// <param name="sorts">sort collection.</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">size of page.</param>
        /// <returns>List of Account Notes.</returns>
        NoteListModel GetAccountNotes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Account Note on the basis of Note id.
        /// </summary>
        /// <param name="noteId">Note Id to get Account Note.</param>
        /// <returns>Returns Note Model.</returns>
        NoteModel GetAccountNote(int noteId);

        /// <summary>
        /// Create Account Note.
        /// </summary>
        /// <param name="noteModel">Note Model.</param>
        /// <returns>Returns created Note Model.</returns>
        NoteModel CreateAccountNote(NoteModel noteModel);

        /// <summary>
        /// Update Account Note.
        /// </summary>
        /// <param name="noteModel">Note model to update.</param>
        /// <returns>Returns updated Note model.</returns>
        NoteModel UpdateAccountNote(NoteModel noteModel);

        /// <summary>
        /// Delete note.
        /// </summary>
        /// <param name="noteId">Note Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteAccountNote(ParameterModel noteId);
        #endregion

        #region Department
        /// <summary>
        /// Get List of Account Departments.
        /// </summary>
        /// <param name="expands">collection of expands.</param>
        /// <param name="filters">collection of filter.</param>
        /// <param name="sorts">sort collection.</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">size of page.</param>
        /// <returns>List of Account Departments.</returns>
        AccountDepartmentListModel GetAccountDepartments(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Department on the basis of Department id.
        /// </summary>
        /// <param name="departmentId">Department Id to get Department.</param>
        /// <returns>Returns User's Department Model.</returns>
        AccountDepartmentModel GetAccountDepartment(int departmentId);

        /// <summary>
        /// Create Account Department.
        /// </summary>
        /// <param name="departmentModel">User Department Model.</param>
        /// <returns>Returns created Account Department Model.</returns>
        AccountDepartmentModel CreateAccountDepartment(AccountDepartmentModel departmentModel);

        /// <summary>
        /// Update Account Department.
        /// </summary>
        /// <param name="departmentModel">Department model to update.</param>
        /// <returns>Returns updated Account Department model.</returns>
        AccountDepartmentModel UpdateAccountDepartment(AccountDepartmentModel departmentModel);

        /// <summary>
        /// Delete Account Department.
        /// </summary>
        /// <param name="departmentIds">Account Department Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteAccountDepartment(ParameterModel departmentIds);
        #endregion

        #region Address
        /// <summary>
        /// Get the address list.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">size of page.</param>
        /// <returns>Returns address list.</returns>
        AddressListModel GetAddressList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get account address. 
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <returns>Returns address.</returns>
        AddressModel GetAccountAddress(ExpandCollection expands, FilterCollection filters);

        /// <summary>
        /// Create account address.
        /// </summary>
        /// <param name="addressModel">Model to create.</param>
        /// <returns>Returns created model.</returns>
        AddressModel CreateAccountAddress(AddressModel addressModel);

        /// <summary>
        /// Update account address.
        /// </summary>
        /// <param name="addressModel">Model to update in db.</param>
        /// <returns>Updated model.</returns>
        AddressModel UpdateAccountAddress(AddressModel addressModel);

        /// <summary>
        /// Delete account address.
        /// </summary>
        /// <param name="accountAddressId">Account Address Id to delete</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteAccountAddress(ParameterModel accountAddressId);
        #endregion

        #region Associate Price
        /// <summary>
        /// Associate Price to Account.
        /// </summary>
        /// <param name="priceAccountModel">PriceAccountModel</param>
        /// <returns>Returns true if price associated successfully else return false.</returns>
        bool AssociatePriceList(PriceAccountModel priceAccountModel);

        /// <summary>
        /// UnAssociate associated price list from Account.
        /// </summary>
        /// <param name="priceAccountModel">Model containing price list ids and Account Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool UnAssociatedPriceList(PriceAccountModel priceAccountModel);

        /// <summary>
        /// Get associated price list precedence value for Account.
        /// </summary>
        /// <param name="priceAccountModel">priceAccountModel contains price list id and account id to get precedence.</param>
        /// <returns>Returns PriceAccountModel.</returns>
        PriceAccountModel GetAssociatedPriceListPrecedence(PriceAccountModel priceAccountModel);

        /// <summary>
        /// Update associated price list precedence value Account.
        /// </summary>
        /// <param name="priceAccountModel">PriceAccountModel.</param>
        /// <returns>Returns updated PriceAccountModel.</returns>
        PriceAccountModel UpdateAssociatedPriceListPrecedence(PriceAccountModel priceAccountModel);
        #endregion

        #region Account Order
        /// <summary>
        /// Get user order list of account.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <param name="accountId">Account Id</param>
        /// <returns>Returns user order list of account.</returns>
        OrdersListModel GetAccountUserOrderList(int accountId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
        #endregion

        #region Account Profile

        /// <summary>
        /// Get list for associated/unassociated profile.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with profile.</param>
        /// <param name="filters">Filters to be applied on profile.</param>
        /// <param name="sorts">Sorting to be applied on profile.</param>
        /// <param name="pageIndex">Start page index of profile.</param>
        /// <param name="pageSize">Page size of profile.</param>
        /// <returns>Returns profiles associated/unassociated to account.</returns>
        ProfileListModel GetAssociatedUnAssociatedProfile(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate Profile to Account.
        /// </summary>
        /// <param name="profileModel">ProfileModel</param>
        /// <returns>Returns true if profile associated successfully else return false.</returns>
        bool AssociateProfile(ProfileModel profileModel);

        /// <summary>
        /// UnAssociate associated profiles from Account.
        /// </summary>
        /// <param name="model">Model containing profile ids and Account Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool UnAssociateProfile(AccountProfileModel profileModel);


        #endregion

        #region Approval Routing

        /// <summary>
        /// Get Approver Level List
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with approver level list.</param>
        /// <param name="filters">Filters to be applied on with approver level list.</param>
        /// <param name="sorts">Sorting to be applied on with approver level list.</param>
        /// <param name="pageIndex">Start page index of with approver level list.</param>
        /// <param name="pageSize">Page size of with approver level list.</param>
        /// <returns>Return approver level list.</returns>
        UserApproverListModel GetApproverLevelList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create and update approver level.
        /// </summary>
        /// <param name="model">Approver Level Model</param>
        /// <returns>Return true or false</returns>
        bool CreateApproverLevel(UserApproverModel approverLevelModel);

        /// <summary>
        /// This function save the permission setting.
        /// </summary>
        /// <param name="model">Permission code model</param>
        PermissionCodeModel SavePermissionSetting(PermissionCodeModel model);

        /// <summary>
        /// Gets the list of levels.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with level list.</param>
        /// <param name="filters">Filters to be applied on level list.</param>
        /// <param name="sorts">Sorting to be applied on level list.</param>
        /// <param name="pageIndex">Start page index of level list.</param>
        /// <param name="pageSize">Page size of level list.</param>
        /// <returns>levels list model.</returns>
        ApproverLevelListModel GetLevelsList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Delete approverLevel by Id.
        /// </summary>
        /// <param name="approverLevelId">ApproverLevel Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteApproverLevel(ParameterModel approverLevelId);

        #endregion

        /// <summary>
        /// Get parent account List
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>Return parent account List</returns>
        ParentAccountListModel GetParentAccountList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
    }
}
