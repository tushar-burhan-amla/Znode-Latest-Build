using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class QuickOrderController : BaseController
    {
        #region Private Variables
        private readonly IQuickOrderCache _cache;        

        #endregion

        #region Constructor
        public QuickOrderController(IQuickOrderService service)
        {            
            _cache = new QuickOrderCache(service);
        }
        #endregion

        #region Public Methods
        /// <summary>
		/// This method return quick order products based on parameter value
		/// </summary>
		/// <returns>Return list.</returns>
		[ResponseType(typeof(QuickOrderProductListResponse))]
        [HttpPost,ValidateModel]
        public virtual HttpResponseMessage GetQuickOrderProductList([FromBody]QuickOrderParameterModel parameters)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetQuickOrderProductList(parameters, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<QuickOrderProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new QuickOrderProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new QuickOrderProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }


        /// <summary>
		/// This method return random quick order product basic details
		/// </summary>
		/// <returns>Return list.</returns>
		[ResponseType(typeof(QuickOrderProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDummyQuickOrderProductList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetDummyQuickOrderProductList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<QuickOrderProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new QuickOrderProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new QuickOrderProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }
        #endregion
    }
}
