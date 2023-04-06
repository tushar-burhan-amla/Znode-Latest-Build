using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Mvc;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class DemoController : BaseController
    {
        readonly IUserCache _cache;
        public DemoController()
        {            
            _cache = new UserCache();
        }
        // [ExpandAccountType, ExpandAddresses, ExpandOrders, ExpandProfiles, ExpandUser, ExpandWishList, ExpandReferralCommissionType]
        [HttpGet]
        public HttpResponseMessage Get(int accountId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetUser(accountId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                var data = new UserResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }
    }
}