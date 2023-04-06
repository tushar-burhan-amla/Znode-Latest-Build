using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Attributes;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using System.Web.Http.Description;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class OrderStateController : BaseController
    {
        #region Private Variables
        private readonly IOrderStateCache _cache;

        #endregion

        #region Constructor
        public OrderStateController(IOrderStateService service)
        {
            _cache = new OrderStateCache(service);
        }
        #endregion

        #region Action Method

        [ResponseType(typeof(OrderStateListResponse))]
        [PageIndex, PageSize]
        [HttpGet]
        public virtual HttpResponseMessage OrderStateList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetOrderStates(RouteUri, RouteTemplate);
                // TODO: Add info logs
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<OrderStateListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new OrderStateListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
    }
}
