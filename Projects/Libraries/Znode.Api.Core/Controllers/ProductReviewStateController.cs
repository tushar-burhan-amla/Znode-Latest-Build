using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using System.Web.Http.Description;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class ProductReviewStateController : BaseController
    {
        #region Private Variables
        private readonly IProductReviewStateCache _cache;
        #endregion

        #region Constructor
        public ProductReviewStateController(IProductReviewStateService service)
        {
            _cache = new ProductReviewStateCache(service);
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Get List of ProductReviewState
        /// </summary>
        /// <returns>List of ProductReviewState</returns>
        [ResponseType(typeof(ProductReviewStateListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage ProductReviewStateList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetProductReviewStates(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductReviewStateListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ProductReviewStateListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
    }
}
