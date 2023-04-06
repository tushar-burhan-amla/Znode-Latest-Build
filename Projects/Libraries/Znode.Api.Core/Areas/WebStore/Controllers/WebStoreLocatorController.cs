using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class WebStoreLocatorController : BaseController
    {
        #region Private Variables
        private readonly IWebStoreLocatorCache _cache;
        #endregion

        #region Default Constructor
        public WebStoreLocatorController(IStoreLocatorService service)
        {
            _cache = new WebStoreLocatorCache(service);
        }
        #endregion

        /// <summary>
        /// Get list of store locator.
        /// </summary>
        /// <returns>Returns list of store locator.</returns>   
        [ResponseType(typeof(WebStoreLocatorResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                //Get data from cache
                string data = _cache.GetWebStoreLocatorList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreLocatorResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
    }
}