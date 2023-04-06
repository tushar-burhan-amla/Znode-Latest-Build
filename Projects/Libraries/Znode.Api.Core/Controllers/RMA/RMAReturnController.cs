using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Attributes;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Controllers
{
    public class RMAReturnController : BaseController
    {
        #region Private Variables
        private readonly IRMAReturnCache _cache;
        private readonly IRMAReturnService _service;
        #endregion

        #region Constructor
        public RMAReturnController(IRMAReturnService service)
        {
            _service = service;
            _cache = new RMAReturnCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get order details by order number for return.
        /// </summary>
        /// <returns>Get order details.</returns>
        [ResponseType(typeof(OrderResponse))]
        [HttpGet]
        public HttpResponseMessage GetOrderDetailsForReturn(int userId, string orderNumber, bool isFromAdmin = false)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetOrderDetailsForReturn(userId, orderNumber, isFromAdmin);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<OrderResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get the list of all Returns.
        /// </summary>
        /// <returns>Returns list of all returns.</returns>
        [ResponseType(typeof(RMAReturnListResponse))]
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetReturnList(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<RMAReturnListResponse>(data);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RMAReturnListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RMAReturnListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Check if order eligible is for return
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="portalId">portalId</param>
        /// <param name="orderNumber">orderNumber</param>
        /// <returns>Returns true if the order is eligible for return</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage IsOrderEligibleForReturn(int userId, int portalId, string orderNumber)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.IsOrderEligibleForReturn(userId, portalId, orderNumber) });
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
        /// Get order return details by return number
        /// </summary>
        /// <param name="returnNumber">Return Number</param>
        /// <returns>Return details</returns>
        /// 
        [ResponseType(typeof(RMAReturnResponse))]
        [HttpGet]
        public HttpResponseMessage GetReturnDetails(string returnNumber)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetReturnDetails(returnNumber);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RMAReturnResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RMAReturnResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RMAReturnResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Insert or update order return details.
        /// </summary>
        /// <param name="returnModel">returnModel</param>
        /// <returns>Returns RMAReturnModel.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(RMAReturnResponse))]
        public virtual HttpResponseMessage SaveOrderReturn([FromBody] RMAReturnModel returnModel)
        {
            HttpResponseMessage response;
            try
            {
                returnModel.IsSubmitReturn = false;
                //Insert or update order return details.
                RMAReturnModel rmaReturnModel = _service.SaveOrderReturn(returnModel);
                if (IsNotNull(rmaReturnModel))
                {
                    response = CreateCreatedResponse(new RMAReturnResponse { Return = rmaReturnModel });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(rmaReturnModel.RmaReturnDetailsId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RMAReturnResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RMAReturnResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete order return on the basis of return number.
        /// </summary>
        /// <param name="returnNumber">Return Number</param>
        /// <param name="userId">userId</param>
        /// <returns>Returns true if deleted successfully else returns false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteOrderReturnByReturnNumber(string returnNumber, int userId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteOrderReturnByReturnNumber(returnNumber, userId);
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
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message});
            }
            return response;
        }

        /// <summary>
        /// Submit order return.
        /// </summary>
        /// <param name="returnModel">returnModel</param>
        /// <returns>Returns RMAReturnModel.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(RMAReturnResponse))]
        public virtual HttpResponseMessage SubmitOrderReturn([FromBody] RMAReturnModel returnModel)
        {
            HttpResponseMessage response;
            try
            {
                returnModel.IsSubmitReturn = true;
                //submit return details.
                RMAReturnModel rmaReturnModel = _service.SaveOrderReturn(returnModel);
                if (IsNotNull(rmaReturnModel))
                {
                    response = CreateCreatedResponse(new RMAReturnResponse { Return = rmaReturnModel });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(rmaReturnModel.RmaReturnDetailsId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
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
        ///  Perform calculations for an order return line item.
        /// </summary>
        /// <param name="returnCalculateModel">returnCalculateModel</param>
        /// <returns>Returns RMAReturnCalculateModel.</returns>
        [ResponseType(typeof(RMAReturnCalculateResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage CalculateOrderReturn([FromBody] RMAReturnCalculateModel returnCalculateModel)
        {
            HttpResponseMessage response;
            try
            {
                RMAReturnCalculateModel calculateModel = _service.CalculateOrderReturn(returnCalculateModel);
                response = IsNotNull(calculateModel) ? CreateOKResponse(new RMAReturnCalculateResponse { ReturnCalculateModel = calculateModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new RMAReturnCalculateResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RMAReturnCalculateResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get List of Return States
        /// </summary>
        /// <returns>List of ReturnStates</returns>
        [ResponseType(typeof(RMAReturnStateListResponse))]
        [PageIndex, PageSize]
        [HttpGet]
        public virtual HttpResponseMessage GetReturnStatusList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetReturnStatusList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RMAReturnStateListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new RMAReturnStateListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get order return details for admin by return number
        /// </summary>
        /// <param name="returnNumber">Return Number</param>
        /// <returns>Return details</returns>
        /// 
        [ResponseType(typeof(RMAReturnResponse))]
        [HttpGet]
        public HttpResponseMessage GetReturnDetailsForAdmin(string returnNumber)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetReturnDetailsForAdmin(returnNumber);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RMAReturnResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    response = CreateUnauthorizedResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
                else
                {
                    response = CreateInternalServerErrorResponse(new RMAReturnResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RMAReturnResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create return history.
        /// </summary>
        /// <param name="returnHistoryModel">List of RMAReturnHistoryModel</param>
        /// <returns>Returns true if return history created successfully</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage CreateReturnHistory([FromBody] List<RMAReturnHistoryModel> returnHistoryModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isReturnHistoryCreated = _service.CreateReturnHistory(returnHistoryModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isReturnHistoryCreated });
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
        /// Save notes for return.
        /// </summary>
        /// <param name="rmaReturnNotesModel">RMAReturnNotesModel</param>
        /// <returns>Returns true if return notes saved successfully</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage SaveReturnNotes(RMAReturnNotesModel rmaReturnNotesModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isReturnNotesSaved = _service.SaveReturnNotes(rmaReturnNotesModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isReturnNotesSaved });
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
        /// Validate orderlineitem to create return
        /// </summary>
        /// <param name="returnModel">returnModel</param>
        /// <returns>Returns RMAReturnModel.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(RMAReturnResponse))]
        public virtual HttpResponseMessage IsValidReturnItems([FromBody] RMAReturnModel returnModel)
        {
            HttpResponseMessage response;
            try
            {
                //submit return details.
                RMAReturnModel rmaReturnModel = _service.IsValidReturnItems(returnModel);
                if (IsNotNull(rmaReturnModel))
                {
                    response = CreateCreatedResponse(new RMAReturnResponse { Return = rmaReturnModel });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(rmaReturnModel.RmaReturnDetailsId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
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
    }
}
