using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Multifront.PaymentApplication.Api.Filters;
using Znode.Multifront.PaymentApplication.Data.Service;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [PrivateKeyAuthorization]
    public class AuthTokenController : BaseController
    {
        [HttpGet]
        [ResponseType(typeof(StringResponse))]
        public virtual HttpResponseMessage GenerateToken(string userOrSessionId, bool fromAdminApp)
        {
            HttpResponseMessage response;
            try
            {
                AuthTokenService authService = new AuthTokenService();
                return Request.CreateResponse(HttpStatusCode.OK, new StringResponse { Response = authService.CreateToken(EncryptionHelper.DecodeBase64(userOrSessionId), fromAdminApp) });
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.ExpectationFailed, new StringResponse { Response = "Token generation failed." });
            }
            return response;
        }

        [HttpDelete]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage DeleteExpiredAuthToken()
        {
            HttpResponseMessage response;
            try
            {
                AuthTokenService authService = new AuthTokenService();
                bool status = authService.DeleteExpiredAuthToken();
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
    }
}
