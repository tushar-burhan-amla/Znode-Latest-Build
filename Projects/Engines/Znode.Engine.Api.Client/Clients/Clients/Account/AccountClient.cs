using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class AccountClient : BaseClient, IAccountClient
    {
        #region Account
        //Get Company Account List
        public virtual AccountListModel GetAccountList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AccountsEndpoint.GetAccountList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AccountListResponse response = GetResourceFromEndpoint<AccountListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AccountListModel list = new AccountListModel { Accounts = response?.Accounts };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Gets the account on the basis of provided name
        public virtual AccountModel GetAccountByName(string accountName, ExpandCollection expands, int portalId = 0)
        {
            //Get Endpoint
            string endpoint = AccountsEndpoint.GetAccountByUserName(accountName, portalId);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);

            //Get response
            ApiStatus status = new ApiStatus();
            AccountResponse response = GetResourceFromEndpoint<AccountResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Account;
        }

        //Get Company Account Details by Id.
        public virtual AccountModel GetAccount(int accountId)
        {
            //Get Endpoint.
            string endpoint = AccountsEndpoint.GetAccount(accountId);

            //Get response.
            ApiStatus status = new ApiStatus();
            AccountResponse response = GetResourceFromEndpoint<AccountResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.Account;
        }

        //Create Company Account
        public virtual AccountDataModel Create(AccountDataModel companyModel)
        {
            string endpoint = AccountsEndpoint.Create();

            ApiStatus status = new ApiStatus();
            AccountDataResponse response = PostResourceToEndpoint<AccountDataResponse>(endpoint, JsonConvert.SerializeObject(companyModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CompanyDetails;
        }

        //Update Company Account
        public virtual AccountDataModel UpdateAccount(AccountDataModel companyModel)
        {
            string endpoint = AccountsEndpoint.Update();

            ApiStatus status = new ApiStatus();
            AccountDataResponse response = PutResourceToEndpoint<AccountDataResponse>(endpoint, JsonConvert.SerializeObject(companyModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.CompanyDetails;
        }

        //Delete the Account by AccountId.
        public virtual bool DeleteAccount(ParameterModel accountId)
        {
            string endpoint = AccountsEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(accountId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        #endregion

        #region Account Notes
        //Get account note list.
        public virtual NoteListModel GetAccountNotes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AccountsEndpoint.GetAccountNotes();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            NoteListResponse response = GetResourceFromEndpoint<NoteListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            NoteListModel list = new NoteListModel();
            if (HelperUtility.IsNotNull(response))
            {
                list.Notes = response.Notes;
                list.CustomerName = response.CustomerName;
            }
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get account note on the basis of note id.
        public virtual NoteModel GetAccountNote(int noteId)
        {
            string endpoint = AccountsEndpoint.GetAccountNote(noteId);

            ApiStatus status = new ApiStatus();
            NoteResponse response = GetResourceFromEndpoint<NoteResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Note;
        }

        //Create account note.
        public virtual NoteModel CreateAccountNote(NoteModel noteModel)
        {
            string endpoint = AccountsEndpoint.CreateAccountNote();

            ApiStatus status = new ApiStatus();
            NoteResponse response = PostResourceToEndpoint<NoteResponse>(endpoint, JsonConvert.SerializeObject(noteModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Note;
        }

        //Update account note.
        public virtual NoteModel UpdateAccountNote(NoteModel noteModel)
        {
            string endpoint = AccountsEndpoint.UpdateAccountNote();

            ApiStatus status = new ApiStatus();
            NoteResponse response = PutResourceToEndpoint<NoteResponse>(endpoint, JsonConvert.SerializeObject(noteModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Note;
        }

        //Delete account note.
        public virtual bool DeleteAccountNote(ParameterModel noteId)
        {
            string endpoint = AccountsEndpoint.DeleteAccountNote();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(noteId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        #endregion

        #region Department
        //Get account department list.
        public virtual AccountDepartmentListModel GetAccountDepartments(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AccountsEndpoint.GetAccountDepartments();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AccountDepartmentListResponse response = GetResourceFromEndpoint<AccountDepartmentListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AccountDepartmentListModel list = new AccountDepartmentListModel { Departments = response?.Departments };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get account department on the basis of department id.
        public virtual AccountDepartmentModel GetAccountDepartment(int departmentId)
        {
            string endpoint = AccountsEndpoint.GetAccountDepartment(departmentId);

            ApiStatus status = new ApiStatus();
            AccountDepartmentResponse response = GetResourceFromEndpoint<AccountDepartmentResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Department;
        }

        //Create account department.
        public virtual AccountDepartmentModel CreateAccountDepartment(AccountDepartmentModel departmentModel)
        {
            string endpoint = AccountsEndpoint.CreateAccountDepartment();

            ApiStatus status = new ApiStatus();
            AccountDepartmentResponse response = PostResourceToEndpoint<AccountDepartmentResponse>(endpoint, JsonConvert.SerializeObject(departmentModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Department;
        }

        //Update account department.
        public virtual AccountDepartmentModel UpdateAccountDepartment(AccountDepartmentModel departmentModel)
        {
            string endpoint = AccountsEndpoint.UpdateAccountDepartment();

            ApiStatus status = new ApiStatus();
            AccountDepartmentResponse response = PutResourceToEndpoint<AccountDepartmentResponse>(endpoint, JsonConvert.SerializeObject(departmentModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Department;
        }

        //Delete account department.
        public virtual bool DeleteAccountDepartment(ParameterModel departmentIds)
        {
            string endpoint = AccountsEndpoint.DeleteAccountDepartment();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(departmentIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        #endregion

        #region Address
        //Get address list.
        public virtual AddressListModel GetAddressList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AccountsEndpoint.GetAddressList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AccountListResponse response = GetResourceFromEndpoint<AccountListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AddressListModel list = new AddressListModel { AddressList = response?.AddressList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get account address.
        public virtual AddressModel GetAccountAddress(ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = AccountsEndpoint.GetAccountAddress();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            AccountResponse response = GetResourceFromEndpoint<AccountResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AccountAddress;
        }

        //Create account address.
        public virtual AddressModel CreateAccountAddress(AddressModel addressModel)
        {
            string endpoint = AccountsEndpoint.CreateAccountAddress();

            ApiStatus status = new ApiStatus();
            AccountResponse response = PostResourceToEndpoint<AccountResponse>(endpoint, JsonConvert.SerializeObject(addressModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AccountAddress;
        }

        //Update account address.
        public virtual AddressModel UpdateAccountAddress(AddressModel addressModel)
        {
            string endpoint = AccountsEndpoint.UpdateAccountAddress();

            ApiStatus status = new ApiStatus();
            AccountResponse response = PutResourceToEndpoint<AccountResponse>(endpoint, JsonConvert.SerializeObject(addressModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AccountAddress;
        }

        //Delete account address.
        public virtual bool DeleteAccountAddress(ParameterModel accountAddressId)
        {
            string endpoint = AccountsEndpoint.DeleteAccountAddress();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(accountAddressId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        #endregion

        #region Associate Price
        //Associate Price to Account.
        public virtual bool AssociatePriceList(PriceAccountModel priceAccountModel)
        {
            string endpoint = AccountsEndpoint.AssociatePriceList();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceAccountModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //UnAssociate associated price list from Account.
        public virtual bool UnAssociatedPriceList(PriceAccountModel priceAccountModel)
        {
            string endpoint = AccountsEndpoint.UnAssociatePriceList();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceAccountModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get associated price list precedence value for Account.
        public virtual PriceAccountModel GetAssociatedPriceListPrecedence(PriceAccountModel priceAccountModel)
        {
            string endpoint = AccountsEndpoint.GetAssociatedPriceListPrecedence();

            ApiStatus status = new ApiStatus();
            PriceAccountResponse response = PostResourceToEndpoint<PriceAccountResponse>(endpoint, JsonConvert.SerializeObject(priceAccountModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PriceAccount;
        }

        //Update associated price list precedence value Account.
        public virtual PriceAccountModel UpdateAssociatedPriceListPrecedence(PriceAccountModel priceAccountModel)
        {
            string endpoint = AccountsEndpoint.UpdateAssociatedPriceListPrecedence();

            ApiStatus status = new ApiStatus();
            PriceAccountResponse response = PutResourceToEndpoint<PriceAccountResponse>(endpoint, JsonConvert.SerializeObject(priceAccountModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PriceAccount;
        }
        #endregion

        #region Account Order
        //Get user order list of account.
        public virtual OrdersListModel GetAccountUserOrderList(int accountId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = AccountsEndpoint.GetAccountUserOrderList(accountId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            OrderListResponse response = GetResourceFromEndpoint<OrderListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            OrdersListModel list = new OrdersListModel();
            if (HelperUtility.IsNotNull(response?.OrderList))
            {
                list.Orders = response.OrderList.Orders;
                list.HasParentAccounts = response.OrderList.HasParentAccounts;
            }

            list.MapPagingDataFromResponse(response);

            return list;
        }
        #endregion

        #region Account Profile

        // Gets associated profile for account.
        public virtual ProfileListModel GetAssociatedUnAssociatedProfile(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = AccountsEndpoint.GetAssociatedUnAssociatedProfile();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            ProfileListResponse response = GetResourceFromEndpoint<ProfileListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProfileListModel list = new ProfileListModel();
            if (HelperUtility.IsNotNull(response))
            {
                list.Profiles = response.Profiles;
                list.HasParentAccounts = response.HasParentAccounts;
            }
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Associate profile to Account.
        public virtual bool AssociateProfile(ProfileModel profileModel)
        {
            string endpoint = AccountsEndpoint.AssociateProfile();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(profileModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //UnAssociate associated profile from Account.
        public virtual bool UnAssociateProfile(AccountProfileModel profileModel)
        {
            string endpoint = AccountsEndpoint.UnAssociateProfile();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(profileModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
        #endregion

        #region Approval Routing

        //Get Approver level list
        public UserApproverListModel GetApproverLevelList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AccountsEndpoint.GetApproverLevelList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            UserApproverListResponse response = GetResourceFromEndpoint<UserApproverListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            UserApproverListModel userApproverList = null;
            if(HelperUtility.IsNotNull(response))
            {
                userApproverList = new UserApproverListModel { UserApprovers = response.UserApproverList?.UserApprovers, AccountPermissionAccessId = response.UserApproverList?.AccountPermissionAccessId, AccountUserPermissionId = response.UserApproverList.AccountUserPermissionId, AccountId = response.UserApproverList.AccountId, PortalId = response.UserApproverList.PortalId };
                userApproverList.MapPagingDataFromResponse(response);
            }
            return userApproverList;
        }

        //Create and update approval level
        public virtual bool CreateApproverLevel(UserApproverModel approverLevelModel)
        {
            string endpoint = AccountsEndpoint.CreateApproverLevel();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(approverLevelModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.IsSuccess ?? false;
        }

        //Delete approval level by id
        public bool DeleteApproverLevel(ParameterModel approverLevelId)
        {
            string endpoint = AccountsEndpoint.DeleteApproverLevelList();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(approverLevelId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // Get levels list.
        public virtual ApproverLevelListModel GetLevelsList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = AccountsEndpoint.GetLevelsList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            ApproverLevelListResponse response = GetResourceFromEndpoint<ApproverLevelListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ApproverLevelListModel list = new ApproverLevelListModel { ApproverLevelList = response?.ApproverLevelList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Save permission setting
        public PermissionCodeModel SavePermissionSetting(PermissionCodeModel model)
        {
            //Get Endpoint
            string endpoint = AccountsEndpoint.SavePermissionSetting();
            ApiStatus status = new ApiStatus();
            PermissionCodeResponse response = PostResourceToEndpoint<PermissionCodeResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PermissionCode;
        }

        #endregion

        //Get parent account List
        public virtual ParentAccountListModel GetParentAccountList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AccountsEndpoint.GetParentAccountList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ParentAccountListResponse response = GetResourceFromEndpoint<ParentAccountListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ParentAccountListModel parentAccountListModel = new ParentAccountListModel { ParentAccount = response?.ParentAccount };
            parentAccountListModel.MapPagingDataFromResponse(response);

            return parentAccountListModel;
        }
    }
}
