using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
namespace Znode.Api.Core.Helper
{
    public class APIAuthorizeAttribute : AuthorizeAttribute
    {
        public int ErrorCode { get; set; } = ErrorCodes.UnAuthorized;
        public string ErrorMessage { get; set; } = "You are unauthorized to access this resource";
        public HttpStatusCode Httpcode { get; set; } = HttpStatusCode.Unauthorized;


        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (Authorize(actionContext))
                return;

            HandleUnauthorizedRequest(actionContext);
        }
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            BaseResponse data = new BaseResponse();
            data.ErrorCode = ErrorCode;
            data.ErrorMessage = ErrorMessage;
            actionContext.Response = actionContext.Request.CreateResponse(Httpcode, data);
        }
       
        protected virtual bool Authorize(HttpActionContext actionContext)
        {
            try
            {
                bool validFlag = true;
                //If Authorization not required then return true
                if (!Convert.ToBoolean(ZnodeApiSettings.ValidateAuthHeader))
                    return validFlag;

                //SkipAuthorization get sets to true when the action has the [AllowAnonymous] attribute, If true then skip authentication.
                if (SkipAuthorization(actionContext))
                    return validFlag;

                if (ZnodeApiSettings.EnableTokenBasedAuthorization)
                {
                    var headers = actionContext.Request.Headers;
                    string encodedString = headers.Contains("Token") ? headers.GetValues("Token").First() : string.Empty;

                    if (!string.IsNullOrEmpty(encodedString))
                    {
                        string key = EncryptionLibrary.DecryptText(encodedString);

                        string[] parts = key.Split(new char[] { '|' });
                        if (parts.Length > 0)
                        {
                            string domainName = parts[0];
                            string webApiKey = parts[1];
                            if (string.IsNullOrEmpty(webApiKey))
                            {
                                SetErrorCodes(ErrorCodes.WebAPIKeyNotFound, "You are unauthorized to access this resource", HttpStatusCode.Unauthorized, actionContext);
                            }
                            // Validate the Token expiration time.
                            if (!ZnodeTokenHelper.ValidateToken(parts[3], webApiKey, domainName))
                            {
                                validFlag = false;
                                SetErrorCodes(ErrorCodes.UnAuthorized, "Token is expired", HttpStatusCode.Unauthorized, actionContext);
                            }
                        }
                    }
                    else
                    {
                        SetErrorCodes(ErrorCodes.UnAuthorized, "Token is invalid", HttpStatusCode.Unauthorized, actionContext);
                    }
                }
                else if (ZnodeApiSettings.EnableBasicAuthorization)
                {
                    string authValue = actionContext.Request.Headers.GetValues("Authorization").First();
                    string[] authHeader = ZnodeTokenHelper.GetAuthHeader(authValue);

                    if (!Equals(authHeader, null))
                    {
                        validFlag = ZnodeTokenHelper.CheckAuthHeader(authHeader[0], authHeader[1]);
                    }
                }
                return validFlag;
            }
            catch (Exception ex)
            {
                return false;
            }
        } 

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                       || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        private void SetErrorCodes(int error_code, string error_message, HttpStatusCode httpstatus_code, HttpActionContext actionContext)
        {
            ErrorCode = error_code;
            ErrorMessage = error_message;
            Httpcode = httpstatus_code;
            HandleUnauthorizedRequest(actionContext);
        }
    }
}
