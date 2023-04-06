using System;
using System.Diagnostics;
using System.Net.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models.Responses.V2;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers.V2
{
    public class WebstoreCategoryV2Controller : BaseController
    {
        #region Private Variables
        private readonly IWebStoreCategoryCacheV2 _cache;
        #endregion

        #region Constructor
        public WebstoreCategoryV2Controller(ICategoryServiceV2 service)
        {
            _cache = new WebStoreCategoryCacheV2(service);
        }
        #endregion

        /// <summary>
        /// Get the products assigned to a category.
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetCategoryProducts()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCategoryProducts(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CategoryProductListResponseV2>(data) 
                                                       : CreateOKResponse(new TrueFalseResponse { ErrorMessage = "No products found for the given category.", IsSuccess = true });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new CategoryProductListResponseV2 { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CategoryProductListResponseV2 { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }
    }
}