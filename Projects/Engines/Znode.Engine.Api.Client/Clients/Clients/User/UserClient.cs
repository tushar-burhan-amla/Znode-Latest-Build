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
    public class UserClient : BaseClient, IUserClient
    {
        public virtual UserModel Login(UserModel model, ExpandCollection expands)
        {
            string endpoint = UsersEndpoint.Login();
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            UserResponse response = PostResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Unauthorized };
            CheckStatusAndThrow<ZnodeUnauthorizedException>(status, expectedStatusCodes);
            return response?.User;
        }

        public virtual UserModel ChangePassword(UserModel model)
        {
            string endpoint = UsersEndpoint.ChangePassword();
            ApiStatus status = new ApiStatus();

            UserResponse response = PostResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.User;
        }

        public virtual bool BulkResetPassword(ParameterModel userId)
        {
            string endpoint = UsersEndpoint.BulkResetPassword();
            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(userId), status);

            //Check status
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public virtual UserModel ForgotPassword(UserModel model)
        {
            string endpoint = UsersEndpoint.ForgotPassword();
            ApiStatus status = new ApiStatus();

            UserResponse response = PostResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.User;
        }

        public virtual int? VerifyResetPasswordLinkStatus(UserModel model)
        {
            string endpoint = UsersEndpoint.VerifyResetPasswordLinkStatus();
            ApiStatus status = new ApiStatus();

            UserResponse response = PostResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.ErrorCode;
        }

        public virtual UserModel GetAccountByUser(string username)
        {
            string endpoint = UsersEndpoint.GetByUsername();
            ApiStatus status = new ApiStatus();

            UserResponse response = PostResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(new UserModel() { UserName = username }), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.User;
        }

        public virtual UserModel CreateUser(UserModel userModel)
        {
            //Get Endpoint
            string endpoint = UsersEndpoint.CreateUser();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            UserResponse response = PostResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(userModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.User;
        }

        public virtual UserModel GetUserAccountData(int userId, int portalId = 0) => GetUserAccountData(userId, null, portalId);

        public virtual UserModel GetUserAccountData(int userId, ExpandCollection expands, int portalId = 0)
        {
            //Get Endpoint
            string endpoint = UsersEndpoint.GetUserAccountData(userId, portalId);
            endpoint += BuildEndpointQueryString(expands);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            UserResponse response = GetResourceFromEndpoint<UserResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.User;
        }

        //Update User account data.
        public virtual UserModel UpdateUserAccountData(UserModel model, bool webstoreUser)
        {
            //Get Endpoint
            string endpoint = UsersEndpoint.UpdateUserAccountData(webstoreUser);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            UserResponse response = PutResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.User;
        }

        //Get user account list.
        public virtual UserListModel GetUserAccountList(int loggedUserAccountId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = UsersEndpoint.GetUserAccountList(loggedUserAccountId);
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            UserListResponse response = GetResourceFromEndpoint<UserListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes list.
            UserListModel list = new UserListModel { Users = response?.Users };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Get List of Sales Rep Users
        public virtual UserListModel GetSalesRepListForAssociation(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = UsersEndpoint.GetSalesRepListForAssociation();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            UserListResponse response = GetResourceFromEndpoint<UserListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //SalesRep list.
            UserListModel list = new UserListModel { Users = response?.Users };
            list.MapPagingDataFromResponse(response);

            return list;
        }


        public virtual bool DeleteUser(ParameterModel userId)
        {
            //Get Endpoint.
            string endpoint = UsersEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(userId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public virtual bool EnableDisableAccount(ParameterModel userId, bool lockUser)
        {
            //Get Endpoint.
            string endpoint = UsersEndpoint.EnableDisableAccount(lockUser);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(userId), status);

            //Check status
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public virtual UserModel CreateCustomerAccount(UserModel accountModel)
        {
            //Get Endpoint
            string endpoint = UsersEndpoint.CreateCustomerAccount();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            UserResponse response = PostResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(accountModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.User;
        }

        public virtual UserModel UpdateCustomerAccount(UserModel model)
        {
            //Get Endpoint
            string endpoint = UsersEndpoint.UpdateCustomerAccount();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            UserResponse response = PutResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.User;
        }

        //Get customer account list.
        public virtual UserListModel GetCustomerAccountList(string currentUserName, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, string columnList = "")
        {
            int loggedUserAccountId = UserId > 0 ? UserId : GetAccountByUser(currentUserName).UserId;
            //Get Endpoint.
            string endpoint = UsersEndpoint.GetCustomerAccountList(loggedUserAccountId);

            return GetCustomerList(endpoint, filters, sorts, pageIndex, pageSize, columnList);
        }

        //Get customer account list.
        public virtual UserListModel GetCustomerListForAdmin(string currentUserName, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, string columnList = "", int loggedUserAccountId = 0, string roleName = null)
        {
            if (loggedUserAccountId <= 0)
                loggedUserAccountId = UserId > 0 ? UserId: GetAccountByUser(currentUserName).UserId;

            //Get Endpoint.
            string endpoint = UsersEndpoint.GetCustomerAccountListForAdmin(loggedUserAccountId, currentUserName, roleName);

            return GetCustomerList(endpoint, filters, sorts, pageIndex, pageSize, columnList);
        }

        public virtual UserListModel GetCustomerList(string endpoint, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, string columnList = "")
        {
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            if (!string.IsNullOrEmpty(columnList))
                endpoint = BuildCustomEndpointQueryString(endpoint, "columnList", columnList);

            //Get response.
            ApiStatus status = new ApiStatus();
            UserListResponse response = GetResourceFromEndpoint<UserListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //customer list.
            UserListModel list = new UserListModel { Users = response?.Users };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Get customer account list.
        public virtual UserListModel GetCustomersForAdmin(AdminUserModel userAdminModel, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            if (userAdminModel?.LoggedUserAccountId <= 0)
                userAdminModel.LoggedUserAccountId = UserId > 0 ? UserId : GetAccountByUser(userAdminModel.UserName).UserId;

            //Get Endpoint.
            string endpoint = UsersEndpoint.GetCustomerListForAdmin();

            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            UserListResponse response = PostResourceToEndpoint<UserListResponse>(endpoint, JsonConvert.SerializeObject(userAdminModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //customer list.
            UserListModel list = new UserListModel { Users = response?.Users };
            list.MapPagingDataFromResponse(response);
            return list;
        }
        //Get UnAssociated Customer(s).
        public virtual UserListModel GetUnAssociatedCustomerList(int portalId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = UsersEndpoint.GetUnAssociatedCustomerList(portalId);
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            UserListResponse response = GetResourceFromEndpoint<UserListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Customer list.
            UserListModel list = new UserListModel { Users = response?.Users };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Update user account mapping
        public virtual bool UpdateUserAccountMapping(UserAccountModel userModel)
        {
            //Get Endpoint.
            string endpoint = UsersEndpoint.UpdateUserAccountMapping();

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(userModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.IsSuccess == true;
        }



        public virtual UserListModel GetAccountList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = UsersEndpoint.GetAccountList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            UserListResponse response = GetResourceFromEndpoint<UserListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Attributes list.
            UserListModel list = new UserListModel { Users = response?.Users };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Gets the assigned portals to user.
        public virtual UserPortalModel GetPortalIds(string aspNetUserId)
        {
            //Get Endpoint
            string endpoint = UsersEndpoint.GetPortalIds(aspNetUserId);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            UserResponse response = GetResourceFromEndpoint<UserResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.UserPortal;
        }

        //Save portal ids againt the user.
        public virtual UserPortalModel SavePortalsIds(UserPortalModel model)
        {
            //Get Endpoint
            string endpoint = UsersEndpoint.SavePortalsIds();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            UserResponse response = PostResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.UserPortal;
        }

        //Sign up for news letter.
        public virtual bool SignUpForNewsLetter(NewsLetterSignUpModel model)
        {
            string endpoint = UsersEndpoint.SignUpForNewsLetter();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public virtual bool IsDefaultAdminPasswordReset()
        {
            //Get Endpoint.
            string endpoint = UsersEndpoint.IsDefaultAdminPasswordReset();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Convert shopper to admin.
        public virtual UserModel ConvertShopperToAdmin(UserModel model)
        {
            string endpoint = UsersEndpoint.ConvertShopperToAdmin();
            ApiStatus status = new ApiStatus();

            UserResponse response = PostResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.User;
        }

        public virtual UserModel UpdateCustomer(UserModel model)
        {
            //Get Endpoint
            string endpoint = UsersEndpoint.UpdateCustomer();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            UserResponse response = PutResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.User;
        }

        //Check and validate the username is an existing Shopper or not.
        public virtual bool CheckIsUserNameAnExistingShopper(string username)
        {
            string endpoint = UsersEndpoint.CheckIsUserNameAnExistingShopper();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(username), status);

            //check the status of response.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.InternalServerError };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response.IsSuccess;
        }

        //Get List of Sales Rep Users
        public virtual SalesRepUserListModel GetSalesRepListForAccount(FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? recordPerPage)
        {
            //Get Endpoint.
            string endpoint = UsersEndpoint.GetSalesRepListForAccount();
            endpoint += BuildEndpointQueryString(null, filters, sortCollection, pageIndex, recordPerPage);

            //Get response.
            ApiStatus status = new ApiStatus();
            SalesRepUserListResponse response = GetResourceFromEndpoint<SalesRepUserListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //SalesRep list.
            SalesRepUserListModel list = new SalesRepUserListModel { SalesRepUsers = response?.SalesRepUsers };
            list.MapPagingDataFromResponse(response);

            return list;
        }


        #region Social login
        // Login to the 3rd party like facebook, google etc.
        public virtual UserModel SocialLogin(SocialLoginModel model)
        {
            string endpoint = UsersEndpoint.SocialLogin();

            ApiStatus status = new ApiStatus();
            UserResponse response = PostResourceToEndpoint<UserResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeUnauthorizedException>(status, new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Unauthorized });
            return response?.User;
        }

        //Update Billing Account Number and Quote status
        public virtual EntityAttributeModel UpdateUserAndQuoteDetails(EntityAttributeModel entityAttributeModel)
        {
            string endpoint = UsersEndpoint.UpdateUserAndQuoteDetails();

            ApiStatus status = new ApiStatus();
            EntityAttributeResponse response = PostResourceToEndpoint<EntityAttributeResponse>(endpoint, JsonConvert.SerializeObject(entityAttributeModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.EntityAttribute;
        }
        #endregion

        #region Impersonation
        public virtual ImpersonationAPIModel ValidateCSRToken(string token)
        {
            ImpersonationAPIModel impersonationAPIModel = new ImpersonationAPIModel();
            impersonationAPIModel.Token = token;
            string endpoint = UsersEndpoint.ValidateCSRToken();
            ApiStatus status = new ApiStatus();

            ImpersonationResponce response = PostResourceToEndpoint<ImpersonationResponce>(endpoint, JsonConvert.SerializeObject(impersonationAPIModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.ImpersonationModel;
        }
        #endregion

        //Get customer account by customer account id.
        public virtual UserModel GetCustomerAccountDetails(int userId)
        {
            //Get Endpoint.
            string endpoint = UsersEndpoint.GetCustomerAccountDetails(userId);

            //Get response.
            ApiStatus status = new ApiStatus();
            UserResponse response = GetResourceFromEndpoint<UserResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.User;
        }

        //Get User Detail By Id
        public virtual UserModel GetUserDetailById(int userId, int portalId)
        {
            //Get Endpoint
            string endpoint = UsersEndpoint.GetUserDetailById(userId, portalId);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            UserResponse response = GetResourceFromEndpoint<UserResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.User;
        }

        //Get Account Order List By Id
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

        //  Pay Invoice Payment
        public virtual OrderModel PayInvoice(PayInvoiceModel payInvoiceModel)
        {
            //Get Endpoint.
            string endpoint = UsersEndpoint.PayInvoice();

            //Get response
            ApiStatus status = new ApiStatus();
            OrderResponse response = PostResourceToEndpoint<OrderResponse>(endpoint, JsonConvert.SerializeObject(payInvoiceModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Order;
        }

        // To update the username of registered user.
        public virtual bool UpdateUsernameForRegisteredUser(UserDetailsModel userDetailsModel)
        {
            // Get endpoints
            string endpoint = UsersEndpoint.UpdateUsernameForRegisteredUser();

            // Get response
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(userDetailsModel), status);

            CheckStatusAndThrow<ZnodeUnauthorizedException>(status, new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NotModified });
            return (response?.IsSuccess).GetValueOrDefault();
        }
    }
}
