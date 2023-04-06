using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Api.Core.Controllers
{
    public class TokenController : BaseController
    {       
        
        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(StringResponse))]
        public virtual HttpResponseMessage GenerateToken()
        {
            HttpResponseMessage response;
            try
            {
                response = GetTokenKey();
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, new StringResponse { Response = "Web API Key Not Found" });
            }
            return response;
        }

        //Get the token
        private HttpResponseMessage GetTokenKey()
        {
            string _authValue = HttpContext.Current.Request.Headers.AllKeys.Contains("Authorization") ? HttpContext.Current.Request.Headers["Authorization"] : string.Empty;

            string[] authHeader = ZnodeTokenHelper.GetAuthHeader(_authValue);

            // Strip off the "Basic " from "Authorization" header
            _authValue = _authValue.Remove(0, 6);

            if (!Equals(authHeader, null))
            {
                bool validFlag = ZnodeTokenHelper.CheckAuthHeader(authHeader[0], authHeader[1]);
                if (validFlag)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new StringResponse { Response = ZnodeTokenHelper.GenerateTokenKey(_authValue) });
                }
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new StringResponse { Response = "Web API Key Not Found" });
        }       
        
    }
}
