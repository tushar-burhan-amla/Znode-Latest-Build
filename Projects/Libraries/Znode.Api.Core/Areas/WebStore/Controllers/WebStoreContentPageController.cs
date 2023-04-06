using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class WebStoreContentPageController :BaseController
    {
        #region Private Variables
        private readonly IContentPageCache _cache;
        #endregion

        #region Constructor
        public WebStoreContentPageController(IContentPageService service)
        {
            _cache = new ContentPageCache(service);
        }
        #endregion

      
        /// <summary>
        /// Get list of content pages.
        /// </summary>
        /// <returns>Returns list of content pages.</returns>
        [ResponseType(typeof(WebStoreContentPageListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetContentPagesList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetContentPagesList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex , ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                WebStoreContentPageListResponse data = new WebStoreContentPageListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                WebStoreContentPageListResponse data = new WebStoreContentPageListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
    }
}
