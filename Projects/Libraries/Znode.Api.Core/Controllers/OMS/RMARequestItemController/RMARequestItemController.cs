using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class RMARequestItemController : BaseController
    {
        #region Private Variables
        private readonly IRMARequestItemCache _cache;
        private readonly IRMARequestItemService _service;
        #endregion

        #region Constructor
        public RMARequestItemController(IRMARequestItemService service)
        {
            _service = service;
            _cache = new RMARequestItemCache(_service);
        }
        #endregion

        /// <summary>
        /// Creates a new RMA request item.
        /// </summary>
        /// <param name="model">The model of the RMA Request Item.</param>
        /// <returns>Creates a new RMA request item.</returns>
        [ResponseType(typeof(RMARequestItemResponse))]
        [HttpPost]
        public HttpResponseMessage Create([FromBody] RMARequestItemModel model)
        {
            HttpResponseMessage response;
            try
            {
                RMARequestItemModel rmaRequestitemModel = _service.CreateRMAItemRequest(model);
                if (!Equals(rmaRequestitemModel, null))
                {
                    var uri = Request.RequestUri;
                    var location = uri.Scheme + "://" + uri.Host + uri.AbsolutePath + "/" + rmaRequestitemModel.RmaRequestItemId;

                    response = CreateCreatedResponse(new RMARequestItemResponse { RMARequestItem = rmaRequestitemModel });
                    response.Headers.Add("Location", location);
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RMARequestItemResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RMARequestItemResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets a list of RMA request items.
        /// </summary>
        /// <returns>List of RMA request item.</returns>
        [ResponseType(typeof(RMARequestItemListResponse))]
        [HttpGet]
        public HttpResponseMessage GetRMARequestItemList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetRMARequestItems(RouteUri, RouteTemplate);
                response = HelperUtility.IsNotNull(data) ? CreateOKResponse<RMARequestItemListResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RMARequestItemListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        /// <summary>
        /// Gets a list of RMA item according to order line items.
        /// </summary>
        /// <returns>RMA request item according to order line items.</returns> 
        [ResponseType(typeof(RMARequestItemListResponse))]
        [HttpGet]
        public HttpResponseMessage GetRMARequestItemsForGiftCard(string orderLineItems)
        {
            HttpResponseMessage response;

            try
            {
                var data = _cache.GetRMARequestItemsForGiftCard(RouteUri, RouteTemplate, orderLineItems);
                response = HelperUtility.IsNotNull(data) ? CreateOKResponse<RMARequestItemListResponse>(data) : CreateNotFoundResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RMARequestItemListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RMARequestItemListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
    }
}