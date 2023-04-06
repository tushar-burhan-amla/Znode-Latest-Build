using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class PromotionTypeController : BaseController
    {
        #region Private Variables
        private readonly IPromotionTypeCache _promotionTypeCache;
        private readonly IPromotionTypeService _promotionTypeService;
        #endregion

        #region Constructor
        public PromotionTypeController(IPromotionTypeService promotionTypeService)
        {
            _promotionTypeService = promotionTypeService;
            _promotionTypeCache = new PromotionTypeCache(_promotionTypeService);
        }
        #endregion

        /// <summary>
        /// Ge the list of all Promotion Types.
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(PromotionTypeListResponse))]
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _promotionTypeCache.GetPromotionTypeList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PromotionTypeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PromotionTypeListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get a PromotionType.
        /// </summary>
        /// <param name="promotionTypeId">ID of Promotion type to get the details.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(PromotionTypeResponse))]
        [HttpGet]
        public HttpResponseMessage Get(int promotionTypeId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _promotionTypeCache.GetPromotionType(promotionTypeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PromotionTypeResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PromotionTypeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create new Promotion Type.
        /// </summary>
        /// <param name="promotionTypeModel">PromotionType Model to create new Promotion type.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(PromotionTypeResponse))]
        [HttpPost]
        public HttpResponseMessage Create([FromBody] PromotionTypeModel promotionTypeModel)
        {
            HttpResponseMessage response;
            try
            {
                PromotionTypeModel promotionType = _promotionTypeService.CreatePromotionType(promotionTypeModel);

                if (!Equals(promotionType, null))
                {
                    response = CreateCreatedResponse(new PromotionTypeResponse { PromotionType = promotionType });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(promotionType.PromotionTypeId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PromotionTypeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Promotion type.
        /// </summary>
        /// <param name="promotionTypeModel">PromotionType Model to update Promotion type.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(PromotionTypeResponse))]
        [HttpPut]
        public HttpResponseMessage Update([FromBody] PromotionTypeModel promotionTypeModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _promotionTypeService.UpdatePromotionType(promotionTypeModel);
                response = isUpdated ? CreateOKResponse(new PromotionTypeResponse { PromotionType = promotionTypeModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PromotionTypeResponse { HasError = true, ErrorCode = ex.ErrorCode, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PromotionTypeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Promotion type
        /// </summary>
        /// <param name="entityIds">ID of Promotion type to delete.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage Delete([FromBody] ParameterModel entityIds)
        {
            HttpResponseMessage response;
            try
            {
                bool isDeleted = _promotionTypeService.DeletePromotionType(entityIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isDeleted });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get all Promotion Types which are not present in database.
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(PromotionTypeListResponse))]
        [HttpGet]
        public HttpResponseMessage GetAllPromotionTypesNotInDatabase()
        {
            HttpResponseMessage response;
            try
            {
                string data = _promotionTypeCache.GetAllPromotionTypesNotInDatabase(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PromotionTypeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PromotionTypeListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Bulk enable or disable promotion types.
        /// </summary>
        /// <param name="entityIds">Ids of promotion types.</param>
        /// <param name="isEnable">boolean value true/false</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage BulkEnableDisablePromotionTypes([FromBody] ParameterModel entityIds, bool isEnable)
        {
            HttpResponseMessage response;
            try
            {
                bool isEnabled = _promotionTypeService.EnableDisablePromotionType(entityIds, isEnable);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isEnabled });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
    }
}
