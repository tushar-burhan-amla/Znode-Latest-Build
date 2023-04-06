using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Attributes;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class RMARequestController : BaseController
    {
        #region Private Variables
        private readonly IRMARequestCache _cache;
        private readonly IRMARequestService _service;
        #endregion

        #region Constructor
        public RMARequestController(IRMARequestService service)
        {
            _service = service;
            _cache = new RMARequestCache(_service);
        }
        #endregion

        #region Public Action Methods
        /// <summary>
        /// Gets a list of RMA requests.
        /// </summary>
        /// <returns>List of RMA request.</returns> 
        [ResponseType(typeof(RMARequestListResponse))]
        [PageIndex]
        [PageSize]
        [HttpGet]
        public HttpResponseMessage GetRMARequestList()
        {
            HttpResponseMessage response;

            try
            {
                var data = _cache.GetRMARequests(RouteUri, RouteTemplate);
                response = !Equals(data, null) ? CreateOKResponse<RMARequestListResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RMARequestListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets RMA request by RMA request id.
        /// </summary>
        /// <param name="rmaRequestId">RMA Request ID.</param>
        /// <returns>HTTP Response for RMA request.</returns>
        [ResponseType(typeof(RMARequestResponse))]
        [HttpGet]
        public HttpResponseMessage GetRMARequest(int rmaRequestId)
        {
            HttpResponseMessage response;

            try
            {
                var data = _cache.GetRMARequest(rmaRequestId, RouteUri, RouteTemplate);
                response = !Equals(data, null) ? CreateOKResponse<RMARequestResponse>(data) : CreateNotFoundResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RMARequestResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RMARequestResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Updates an existing RMA Request.
        /// </summary>
        /// <param name="rmaRequestId">The ID of the RMA Request.</param>
        /// <param name="model">The model of the RMA Request.</param>
        /// <returns>Updates an existing RMA Request.</returns>
        [ResponseType(typeof(RMARequestResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateRMARequest(int rmaRequestId, [FromBody] RMARequestModel model)
        {
            HttpResponseMessage response;

            try
            {
                var rmaRequest = _service.UpdateRMARequest(rmaRequestId, model);
                response = !Equals(rmaRequest, null) ? CreateOKResponse(new RMARequestResponse { RMARequest = rmaRequest }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RMARequestResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RMARequestResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Gets RMA Request Gift card details by rma request id.
        /// </summary> 
        /// <param name="rmaRequestId">RMA Request ID for which issued gift cards will be fetched.</param>
        /// <returns>Returns RMA Request Gift card details</returns>
        [ResponseType(typeof(IssuedGiftCardListResponse))]
        [HttpGet]
        public HttpResponseMessage GetRMAGiftCardDetails(int rmaRequestId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetIssuedGiftCards(rmaRequestId, RouteUri, RouteTemplate);
                response = HelperUtility.IsNotNull(data) ? CreateOKResponse<IssuedGiftCardListResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new IssuedGiftCardListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Creates a new RMA Request.
        /// </summary>
        /// <param name="model">The model of the RMA Request.</param>
        /// <returns>RMA Request model.</returns>
        [ResponseType(typeof(RMARequestResponse))]
        [HttpPost]
        public HttpResponseMessage Create([FromBody] RMARequestModel model)
        {
            HttpResponseMessage response;

            try
            {
                var rmaRequest = _service.CreateRMARequest(model);
                if (!Equals(rmaRequest, null))
                {
                    var uri = Request.RequestUri;
                    var location = uri.Scheme + "://" + uri.Host + uri.AbsolutePath + "/" + rmaRequest.RmaRequestId;

                    response = CreateCreatedResponse(new RMARequestResponse { RMARequest = rmaRequest });
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
                response = CreateInternalServerErrorResponse(new RMARequestResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RMARequestResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// To get order rma display flag by order details id.
        /// </summary>
        /// <param name="omsOrderDetailsId">Order details Id to Get RMA display Flag.</param>        
        /// <returns>Returns true if order RMA  is 1 else returns false.</returns>
        [ResponseType(typeof(RMARequestResponse))]
        [HttpGet]
        public HttpResponseMessage GetOrderRMAFlag(int omsOrderDetailsId)
        {
            HttpResponseMessage response;

            try
            {
                string resp = _cache.GetOrderRMAFlag(omsOrderDetailsId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(resp) ? CreateOKResponse<TrueFalseResponse>(resp) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Send mail to customer.
        /// </summary>
        /// <param name="rmaRequestId">RMA Request id according to which mail is sent or not.</param>
        /// <returns>HttpResponseMessage containing bool value according to email sent status.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage SendRMAStatusMail(int rmaRequestId)
        {
            HttpResponseMessage response;
            try
            {
                bool isStatusMailSent = _service.SendRMAStatusMail(rmaRequestId);

                BooleanModel boolModel = new BooleanModel();
                boolModel.IsSuccess = isStatusMailSent;

                TrueFalseResponse resp = new TrueFalseResponse();
                resp.booleanModel = boolModel;

                response = CreateOKResponse(resp);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        /// <summary>
        /// Send gift card mail to customer.
        /// </summary>
        /// <param name="model">Gift card model from where email data will be retrieved.</param>
        /// <returns>HTTP Response message containing bool value if the mail is sent or not.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage SendGiftCardMail([FromBody] GiftCardModel model)
        {
            HttpResponseMessage response;
            try
            {
                BooleanModel boolModel = new BooleanModel();
                boolModel.IsSuccess = _service.SendGiftCardMail(model);

                TrueFalseResponse resp = new TrueFalseResponse();
                resp.booleanModel = boolModel;

                response = CreateOKResponse(resp);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get service request details for report.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ReportServiceRequestListResponse))]
        [HttpGet]
        public HttpResponseMessage GetServiceRequestReport()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetServiceRequestReport(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportServiceRequestListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportServiceRequestListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}