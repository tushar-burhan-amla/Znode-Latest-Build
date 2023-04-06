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
    public class WebStoreCategoryController : BaseController
    {
        #region Private Variables     
        private readonly IWebStoreCategoryCache _cache;
        #endregion

        #region Default Constructor
        public WebStoreCategoryController(ICategoryService service)
        {
            _cache = new WebStoreCategoryCache(service);
        }
        #endregion

        /// <summary>
        /// Gets list of published categories, sub categories and products.
        /// </summary>
        /// <returns>Returns list of published categories, sub categories and products.</returns>   
        [ResponseType(typeof(WebStoreCategoryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCategoryDetails()
        {
            HttpResponseMessage response;
           
            try
            {
                //Get data from cache
                string data = _cache.GetCategoryDetails(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreCategoryListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreCategoryListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
    }
}