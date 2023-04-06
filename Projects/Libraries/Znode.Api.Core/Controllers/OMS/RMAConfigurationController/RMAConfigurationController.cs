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
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class RMAConfigurationController : BaseController
    {
        #region Private Variables
        private readonly IRMAConfigurationCache _cache;
        private readonly IRMAConfigurationService _service;
        #endregion

        #region Public constructor
        public RMAConfigurationController(IRMAConfigurationService service)
        {
            _service = service;
            _cache = new RMAConfigurationCache(_service);
        }
        #endregion

        #region Public Method
        #region RMA Configuration
        /// <summary>
        /// Get RMA Configuration by RMA config id.
        /// </summary>
        /// <returns>RMA Configuration details.</returns>
        [ResponseType(typeof(RMAConfigurationResponse))]
        [HttpGet]
        public HttpResponseMessage GetRMAConfiguration()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetRMAConfiguration(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RMAConfigurationResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RMAConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Create/Update RMA Configuration.
        /// </summary>
        /// <param name="model">The model of the RMAConfiguration.</param>
        /// <returns>Model of RMAConfiguration having created or updated data.</returns>
        [ResponseType(typeof(RMAConfigurationResponse))]
        [HttpPost]
        public HttpResponseMessage CreateRMAConfiguration([FromBody] RMAConfigurationModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create attribute.
                RMAConfigurationModel rmaConfiguration = _service.CreateRMAConfiguration(model);
                if (HelperUtility.IsNotNull(rmaConfiguration))
                {
                    response = CreateCreatedResponse(new RMAConfigurationResponse { RMAConfiguration = rmaConfiguration });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(rmaConfiguration.RmaConfigurationId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RMAConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RMAConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region Request Status
        /// <summary>
        /// Get the request status by rmaRequestStatusId.
        /// </summary>
        /// <param name="rmaRequestStatusId">The Id of the RequestStatus.</param>
        /// <returns>Returns HttpResponseMessage type response.</returns>
        [ResponseType(typeof(RequestStatusResponse))]
        [HttpGet]
        public HttpResponseMessage GetRequestStatus(int rmaRequestStatusId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetRequestStatus(rmaRequestStatusId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RequestStatusResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RequestStatusResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get list of request status.
        /// </summary>
        /// <returns>List of Request Status.</returns>  
        [ResponseType(typeof(RequestStatusListResponse))]
        [HttpGet]
        public HttpResponseMessage GetRequestStatusList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetRequestStatusList(RouteUri, RouteTemplate);
                response = data != null ? CreateOKResponse<RequestStatusListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RequestStatusListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update request status.
        /// </summary>
        /// <param name="requestStatus">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(RequestStatusResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateRequestStatus([FromBody] RequestStatusModel requestStatus)
        {
            HttpResponseMessage response;
            try
            {
                //Update reason for request.
                response = _service.UpdateRequestStatus(requestStatus) ? CreateCreatedResponse(new RequestStatusResponse { RequestStatus = requestStatus, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(requestStatus.RmaRequestStatusId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RequestStatusResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RequestStatusResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Deletes an existing request status.
        /// </summary>
        /// <param name="rmaRequestStatusId">ParameterModel containing ids of the RequestStatus.</param>
        /// <returns>Returns HttpResponseMessage type response.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage DeleteRequestStatus(ParameterModel rmaRequestStatusId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteRequestStatus(rmaRequestStatusId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
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
        #endregion

        #region Reason For Request
        /// <summary>
        /// Get the reason for return by rmaReasonForReturnId.
        /// </summary>
        /// <param name="rmaReasonForReturnId">The Id of the Reason For Return.</param>
        /// <returns>Returns HttpResponseMessage type response.</returns>
        [ResponseType(typeof(RequestStatusResponse))]
        [HttpGet]
        public HttpResponseMessage GetReasonForReturn(int rmaReasonForReturnId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetReasonForReturn(rmaReasonForReturnId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RequestStatusResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RequestStatusResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get list of reason for request.
        /// </summary>
        /// <returns>List of Reason For Request.</returns> 
        [ResponseType(typeof(RequestStatusListResponse))]
        [HttpGet]
        public HttpResponseMessage GetReasonForReturnList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetReasonForReturnList(RouteUri, RouteTemplate);
                response = data != null ? CreateOKResponse<RequestStatusListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RequestStatusListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create reason for request.
        /// </summary>
        /// <param name="reasonForRequest">Request Status model.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(RequestStatusResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage CreateReasonForReturn([FromBody] RequestStatusModel reasonForRequest)
        {
            HttpResponseMessage response;

            try
            {
                reasonForRequest = _service.CreateReasonForReturn(reasonForRequest);

                if (HelperUtility.IsNotNull(reasonForRequest))
                {
                    response = CreateCreatedResponse(new RequestStatusResponse { RequestStatus = reasonForRequest });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(reasonForRequest.RmaReasonForReturnId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RequestStatusResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RequestStatusResponse { HasError = true, ErrorMessage = ex.Message});
            }
            return response;
        }

        /// <summary>
        /// Update reason for request.
        /// </summary>
        /// <param name="reasonForRequest">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(RequestStatusResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateReasonForReturn([FromBody] RequestStatusModel reasonForRequest)
        {
            HttpResponseMessage response;
            try
            {
                //Update reason for request.
                response = _service.UpdateReasonForReturn(reasonForRequest) ? CreateCreatedResponse(new RequestStatusResponse { RequestStatus = reasonForRequest, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(reasonForRequest.RmaReasonForReturnId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RequestStatusResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RequestStatusResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Deletes an existing reason for request.
        /// </summary>
        /// <param name="rmaReasonForRequestId">ParameterModel containing ids of the Reason For Request.</param>
        /// <returns>Returns HttpResponseMessage type response.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage DeleteReasonForReturn(ParameterModel rmaReasonForRequestId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteReasonForReturn(rmaReasonForRequestId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
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
        #endregion
        #endregion
    }
}
