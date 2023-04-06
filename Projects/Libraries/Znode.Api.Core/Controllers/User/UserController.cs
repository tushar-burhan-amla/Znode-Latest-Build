using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using System.Web.Http.Description;
using System.Diagnostics;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Controllers
{
    public class UserController : BaseController
    {
        #region Private readonly Variables
        private readonly IUserService _service;
        private readonly IUserCache _cache;
        #endregion

        #region Public Constructor
        public UserController(IUserService service)
        {
            _service = service;
            _cache = new UserCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Login to application.
        /// </summary>
        /// <param name="model">User Model.</param>
        /// <returns></returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPost]
        public HttpResponseMessage Login([FromBody] UserModel model)
        {
            HttpResponseMessage response;
            try
            {
                int? errorCode;
                UserModel user = _cache.Login(model?.PortalId > 0 ? model.PortalId.GetValueOrDefault() : PortalId, model, out errorCode);
                response = HelperUtility.IsNotNull(user) ? CreateOKResponse(new UserResponse { User = user, ErrorCode = Convert.ToInt32(errorCode), HasError = errorCode != null }) : null;
            }
            catch (ZnodeUnauthorizedException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateUnauthorizedResponse(new UserResponse { HasError = true, ErrorCode = ex.ErrorCode });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateUnauthorizedResponse(new UserResponse { HasError = true, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateUnauthorizedResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Change the password.
        /// </summary>
        /// <param name="model">User Model.</param>
        /// <returns></returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPost]
        public HttpResponseMessage ChangePassword([FromBody] UserModel model)
        {
            HttpResponseMessage response;
            try
            {
                UserModel user = _service.ChangePassword(model?.PortalId > 0 ? model.PortalId.GetValueOrDefault() : PortalId, model);
                response = HelperUtility.IsNotNull(user) ? CreateOKResponse(new UserResponse { User = user }) : null;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Bulk reset password.
        /// </summary>
        /// <param name="userId">User id to reset the pasword.</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage BulkResetPassword([FromBody] ParameterModel userId)
        {
            HttpResponseMessage response;
            try
            {
                bool resp = _service.BulkResetPassword(userId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = resp });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Forgot password.
        /// </summary>
        /// <param name="model">User Model.</param>
        /// <returns></returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPost]
        public HttpResponseMessage ForgotPassword([FromBody] UserModel model)
        {
            HttpResponseMessage response;
            try
            {
                UserModel user = _service.ForgotPassword(model?.PortalId > 0 ? model.PortalId.GetValueOrDefault() : PortalId, model);
                response = HelperUtility.IsNotNull(user) ? CreateOKResponse(new UserResponse { User = user }) : null;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Verifies the reset password link and sets a valid status code in response.
        /// </summary>
        /// <param name="model">User model</param>
        /// <returns>HttpResponseMessage containing status code.</returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPost]
        public HttpResponseMessage VerifyResetPasswordLinkStatus([FromBody] UserModel model)
        {
            HttpResponseMessage response;
            try
            {
                int? verifyResetPasswordStatusCode = _service.VerifyResetPasswordLinkStatus(model?.PortalId > 0 ? model.PortalId.GetValueOrDefault() : PortalId, model);
                response = CreateOKResponse(new UserResponse { ErrorCode = verifyResetPasswordStatusCode });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Enable/Disable user.
        /// </summary>
        /// <param name="userId">User id to enable/ disable user.</param>
        /// <param name="lockUser">lock user.</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage EnableDisableAccount([FromBody] ParameterModel userId, bool lockUser)
        {
            HttpResponseMessage response;
            try
            {
                bool resp = _service.EnableDisableUser(userId, lockUser);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = resp });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get the user by user name.
        /// </summary>
        /// <param name="model">Model contains the user name and portal id.</param>
        /// <returns></returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPost]
        public HttpResponseMessage GetByUsername([FromBody] UserModel model)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUserByUsername(model.UserName, model.PortalId > 0 ? model.PortalId.GetValueOrDefault() : PortalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Creates a new user account.
        /// </summary>
        /// <param name="model">The model of the account.</param>
        /// <returns>Returns newly created user.</returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPost]
        public HttpResponseMessage CreateAdminUserAccount([FromBody] UserModel model)
        {
            HttpResponseMessage response;
            try
            {
                UserModel accountData = _service.CreateAdminUser(model);
                if (HelperUtility.IsNotNull(accountData))
                {
                    response = CreateCreatedResponse(new UserResponse { User = accountData });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(accountData.UserId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get account data by account id.
        /// </summary>
        /// <param name="accountId">Account Id.</param>
        /// <param name="portalId">portalId</param>
        /// <returns>Returns account data.</returns>
        [ResponseType(typeof(UserResponse))]
        [HttpGet]
        public HttpResponseMessage GetUserAccountData(int accountId, int portalId = 0)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUser(accountId, RouteUri, RouteTemplate, portalId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        /// <summary>
        /// Updates an existing user admin account.
        /// </summary>
        /// <param name="model">Account Model.</param>
        /// <param name="webStoreUser">Web store user status</param>
        /// <returns>Returns updated user account.</returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateUserAccountData([FromBody] UserModel model, bool webStoreUser)
        {
            HttpResponseMessage response;
            try
            {
                bool user = _service.UpdateUserData(model, webStoreUser);
                response = user ? CreateOKResponse(new UserResponse { User = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of user accounts.
        /// </summary>
        /// <param name="loggedUserAccountId">loggedUserAccountId</param>
        /// <returns>Returns user account list.</returns>
        [ResponseType(typeof(UserListResponse))]
        [HttpGet]
        public HttpResponseMessage GetUserAccountList(int loggedUserAccountId)
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetUserList(loggedUserAccountId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete account by accountId.
        /// </summary>
        /// <param name="accountIds">Account Ids to be deleted.</param>
        /// <returns>Returns true if record deleted successfully, else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage DeleteUserAccount([FromBody] ParameterModel accountIds)
        {
            HttpResponseMessage response;
            try
            {
                //Delete account.
                bool deleted = _service.DeleteUser(accountIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #region Customer

        /// <summary>
        /// Create the Customer Account.
        /// </summary>
        /// <param name="model">Model of type AccountModel</param>
        /// <returns>Returns newly created customer account.</returns>
        [ResponseType(typeof(UserResponse))]
        public HttpResponseMessage CreateCustomerAccount([FromBody] UserModel model)
        {
            HttpResponseMessage response;
            try
            {
                UserModel accountData = _service.CreateCustomer(model?.PortalId > 0 ? model.PortalId.GetValueOrDefault() : PortalId, model);
                if (HelperUtility.IsNotNull(accountData))
                {
                    response = CreateCreatedResponse(new UserResponse { User = accountData });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(accountData.UserId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Updates an existing customer account.
        /// </summary>
        /// <param name="model">User Model.</param>
        /// <returns>Returns updated customer account.</returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateCustomerAccount([FromBody] UserModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool account = _service.UpdateCustomer(model);
                response = account ? CreateOKResponse(new UserResponse { User = model }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.UserId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Updates an existing customer account.
        /// </summary>
        /// <param name="model">User Model.</param>
        /// <returns>Returns updated customer account.</returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateCustomer([FromBody] UserModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool account = _service.UpdateCustomer(model, true);
                response = account ? CreateOKResponse(new UserResponse { User = model }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.UserId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of user accounts.
        /// </summary>
        /// <param name="loggedUserAccountId">loggedUserAccountId</param>
        /// <param name="columnList">List of column to display on grid</param>
        /// <returns>Returns user account list.</returns>
        [ResponseType(typeof(UserListResponse))]
        [HttpGet]
        public HttpResponseMessage GetCustomerAccountList(int loggedUserAccountId, string columnList = "")
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetUserList(loggedUserAccountId, RouteUri, RouteTemplate, columnList);

                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method mark as obsolete because when currentUserName contains '+' API is not getting triggered " +
        " Please use overload of this method which is HttpPost method")]
        [ResponseType(typeof(UserListResponse))]
        [HttpGet]
        public HttpResponseMessage GetCustomerAccountListForAdmin(int loggedUserAccountId, string currentUserName, string roleName, string columnList = "")
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetUserListForAdmin(loggedUserAccountId, RouteUri, RouteTemplate, currentUserName, roleName, columnList);
            
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of customer.
        /// </summary>
        /// <param name="model">model of type UserAdminModel which has parameters required like username,userID </param>
        /// <returns>Returns customer list.</returns>
        [ResponseType(typeof(UserListResponse))]
        [HttpPost]
        public HttpResponseMessage GetCustomerListForAdmin([FromBody] AdminUserModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Gets list of customer.
                string data = _cache.GetUserListForAdmin(model.LoggedUserAccountId, RouteUri, RouteTemplate, model.UserName, model.RoleName, model.ColumnList);

                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get UnAssociated Customer(s).
        /// </summary>
        /// <param name="portalId">Portal Id</param>        
        /// <returns>Returns user account list.</returns>
        [ResponseType(typeof(UserListResponse))]
        [HttpGet]
        public HttpResponseMessage GetUnAssociatedCustomerList(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                //Get user list.
                string data = _cache.GetUnAssociatedCustomerList(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Updates an existing user account mapping.
        /// </summary>
        /// <param name="userModel">User Model.</param>        
        /// <returns>Returns true if updated successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateUserAccountMapping([FromBody] UserAccountModel userModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isAssociated = _service.UpdateUserAccountMapping(userModel);
                response = isAssociated ? CreateOKResponse(new TrueFalseResponse { IsSuccess = isAssociated }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }



        /// <summary>
        ///  Gets the list of Sales Rep
        /// </summary>
        /// <returns>Returns list of Sales Rep List</returns>
        [ResponseType(typeof(UserListResponse))]
        [HttpGet]
        public HttpResponseMessage GetSalesRepListForAssociation()
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetSalesRepForAssociation(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets the assigned portals to user.
        /// </summary>
        /// <param name="aspNetUserId">Asp net user Id.</param>
        /// <returns>Returns assigned portals to user.</returns>
        [ResponseType(typeof(UserResponse))]
        [HttpGet]
        public HttpResponseMessage GetPortalIds(string aspNetUserId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPortalIds(aspNetUserId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Save portal ids againt the user.
        /// </summary>
        /// <param name="model">Save portal ids againt the user.</param>
        /// <returns>Returns true if saved successfully.</returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPost]
        public HttpResponseMessage SavePortalsIds([FromBody] UserPortalModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.SavePortalsIds(model) ? CreateOKResponse(new UserResponse { UserPortal = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion

        /// <summary>
        /// Sign up for news letter.
        /// </summary>
        /// <param name="model">NewsLetterSignUpModel containing email address.</param>
        /// <returns>Returns news letter subscription.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage SignUpForNewsLetter([FromBody] NewsLetterSignUpModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.SignUpForNewsLetter(model) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Check default admin password is reset.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage IsDefaultAdminPasswordReset()
        {
            HttpResponseMessage response;
            try
            {
                bool resp = _service.IsDefaultAdminPasswordReset();
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = resp });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Convert shopper to admin.
        /// </summary>
        /// <param name="model">User Model.</param>
        /// <returns></returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPost]
        public HttpResponseMessage ConvertShopperToAdmin([FromBody] UserModel model)
        {
            HttpResponseMessage response;
            try
            {
                UserModel user = _service.ConvertShopperToAdmin(model);
                response = HelperUtility.IsNotNull(user) ? CreateOKResponse(new UserResponse { User = user }) : null;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Check and validate the username is an existing Shopper or not.
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>Returns true/false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage CheckIsUserNameAnExistingShopper([FromBody]string username)
        {
            HttpResponseMessage response;
            try
            {
                bool resp = _service.CheckIsUserNameAnExistingShopper(username);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = resp });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateOKResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        ///  Gets the list of Sales Rep
        /// </summary>
        /// <returns>Returns list of Sales Rep List</returns>
        [ResponseType(typeof(UserListResponse))]
        [HttpGet]
        public HttpResponseMessage GetSalesRepListForAccount()
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetSalesRepForAccount(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #region Social Login
        /// <summary>
        /// Login to the 3rd party like facebook, google etc.
        /// </summary>
        /// <param name="model">SocialLoginModel to login 3rd party.</param>
        /// <returns>Returns UserModel</returns>
        [ResponseType(typeof(UserResponse))]
        [HttpPost]
        public HttpResponseMessage SocialLogin([FromBody] SocialLoginModel model)
        {
            HttpResponseMessage response;
            try
            {
                UserModel user = _service.SocialLogin(model);
                response = HelperUtility.IsNotNull(user) ? CreateOKResponse(new UserResponse { User = user }) : null;
            }
            catch (ZnodeUnauthorizedException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateUnauthorizedResponse(new UserResponse { HasError = true, ErrorCode = ex.ErrorCode });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorCode = ex.ErrorCode, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get login providers.
        /// </summary>
        /// <returns>Returns social login providers.</returns>
        [ResponseType(typeof(SocialProviderResponse))]
        [HttpGet]
        public HttpResponseMessage GetLoginProviders()
        {
            HttpResponseMessage response;
            try
            {
                List<SocialDomainModel> data = _service.GetLoginProviders();
                response = HelperUtility.IsNotNull(data) ? CreateOKResponse(new SocialProviderResponse { SocialDomainList = data }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new SocialProviderResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region Add Billing Number
        /// <summary>
        /// Update User And Quote Details
        /// </summary>
        /// <param name="model"> entityattribute model</param>
        /// <returns>Return entityattributemodel .</returns>
        [ResponseType(typeof(EntityAttributeResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UpdateUserAndQuoteDetails([FromBody] EntityAttributeModel model)
        {
            HttpResponseMessage response;

            try
            {
                IGlobalAttributeGroupEntityService globalAttributeGroupEntityService = GetService<IGlobalAttributeGroupEntityService>();
                var entityAttribute = globalAttributeGroupEntityService.SaveEntityAttributeDetails(model);
                if (!Equals(entityAttribute, null))
                {
                    if (entityAttribute.IsSuccess)
                    {
                        response = CreateCreatedResponse(new EntityAttributeResponse { EntityAttribute = entityAttribute });
                        response.Headers.Add("Location", GetUriLocation(Convert.ToString(entityAttribute.EntityValueId)));
                    }
                    else
                    {
                        response = CreateInternalServerErrorResponse();
                        EntityAttributeResponse data = new EntityAttributeResponse { HasError = true, ErrorMessage = "Billing Account Number Added Successfully but Quote status not updated." };
                        response = CreateInternalServerErrorResponse(data);
                    }
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                EntityAttributeResponse data = new EntityAttributeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EntityAttributeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }
        #endregion


        #region Impersonation

        /// <summary>
        /// Validate impersonation CSR Token
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="portalId">portalId</param>
        /// <returns>Returns account data.</returns>
        [ResponseType(typeof(ImpersonationResponce))]
        [HttpPost]
        public HttpResponseMessage ValidateCSRToken([FromBody] ImpersonationAPIModel impersonationAPIModel)
        {
            HttpResponseMessage response;
            try
            {
                ImpersonationAPIModel impersonationModel = new ImpersonationAPIModel();
                ZnodeEncryption encryption = new ZnodeEncryption();
                string decryptedToken="";
                bool isValidToken = ZnodeTokenHelper.ValidateCSRToken(impersonationAPIModel.Token, out decryptedToken);// encryption.DecryptData(impersonationAPIModel.Token);
                impersonationModel.Result = false;
                if (isValidToken && !string.IsNullOrEmpty(decryptedToken))
                {
                    impersonationModel = new ImpersonationAPIModel();
                    string[] tokenParts =  decryptedToken.Split(new char[] { '|' });
                    int adminUserId = Convert.ToInt32(tokenParts[0].ToString());
                    int userId = Convert.ToInt32(tokenParts[1].ToString());
                    string userName = Convert.ToString(tokenParts[2].ToString());
                    if (adminUserId > 0 && userId > 0 && !string.IsNullOrEmpty(userName))
                    {
                        impersonationModel.Result = true;
                        impersonationModel.CRSUserId = adminUserId;
                        impersonationModel.WebstoreUserId = userId;
                        impersonationModel.Token = impersonationAPIModel.Token;
                        impersonationModel.IsImpersonation = true;
                        impersonationModel.UserName = userName;
                    }
                }
                response = HelperUtility.IsNotNull(impersonationModel) ? CreateOKResponse(new ImpersonationResponce { ImpersonationModel = impersonationModel }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ImpersonationResponce { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ImpersonationResponce { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #endregion

        /// <summary>
        /// Gets user account details.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>Returns user account details.</returns>
        [ResponseType(typeof(UserResponse))]
        [HttpGet]
        public HttpResponseMessage GetCustomerAccountdetails(int userId)
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetCustomerAccountdetails(userId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    response = CreateUnauthorizedResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
                else
                {
                    response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Clear user registration attempt details.
        /// </summary>
        /// <returns> Return status true of false </returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage RemoveAllUserRegistrationAttemptDetails()
        {
            HttpResponseMessage response;
            try
            {
                //Remove user registration attempt details.
                bool resp =  _cache.RemoveUserRegistrationAttemptDetail();
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = resp });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    response = CreateUnauthorizedResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
                else
                {
                    response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get user data by user id.
        /// </summary>
        /// <param name="userId">userId Id.</param>
        /// <param name="portalId">portal Id</param>
        /// <returns>Returns user data.</returns>
        [ResponseType(typeof(UserResponse))]
        [HttpGet] 
        public HttpResponseMessage GetUserDetailById(int userId, int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUserDetailById(userId, RouteUri, RouteTemplate, portalId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// To update the username of the registered user.
        /// </summary>
        /// <param name="userDetailsModel">UserDetailsModel</param>
        /// <returns>True if username updated successfully</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateUsernameForRegisteredUser([FromBody] UserDetailsModel userDetailsModel)
        {
            HttpResponseMessage response;
            try
            {
                if (ModelState.IsValid)
                {
                    BooleanModel usernameUpdated = _service.UpdateUsernameForRegisteredUser(userDetailsModel);
                    response = (usernameUpdated?.IsSuccess).GetValueOrDefault() ? CreateOKResponse(new TrueFalseResponse { IsSuccess = (usernameUpdated?.IsSuccess).GetValueOrDefault() }) :
                        CreateOKResponse(new TrueFalseResponse { IsSuccess = (usernameUpdated?.IsSuccess).GetValueOrDefault(), ErrorMessage = usernameUpdated?.ErrorMessage });
                }
                else
                    response = CreateOKResponse(new TrueFalseResponse { IsSuccess = false, ErrorMessage = Api_Resources.ErrorModelNull });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
      
        /// <summary>
        /// Pay Invoice.
        /// </summary>
        /// <param name="PayInvoiceModel"></param>
        /// <returns>OrderResponse</returns>
        [ResponseType(typeof(OrderResponse))]
        [HttpPost]
        public HttpResponseMessage PayInvoice([FromBody] PayInvoiceModel payInvoiceModel)
        {
            HttpResponseMessage response;
            try
            {
                OrderModel model = _service.PayInvoice(payInvoiceModel);
                response = HelperUtility.IsNotNull(model) ? CreateCreatedResponse(new OrderResponse { Order = model }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion
    }
}
