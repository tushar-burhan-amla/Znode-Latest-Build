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
    public class ShippingTypeController : BaseController
    {
        #region Private Variables
        private readonly IShippingTypeCache _shippingTypeCache;
        private readonly IShippingTypeService _shippingTypeService;
        #endregion

        #region Constructor
        public ShippingTypeController(IShippingTypeService shippingTypeService)
        {
            _shippingTypeService = shippingTypeService;
            _shippingTypeCache = new ShippingTypeCache(_shippingTypeService);
        }
        #endregion

        /// <summary>
        /// Get the list of all Shipping Types.
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(ShippingTypeListResponse))]
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _shippingTypeCache.GetShippingTypeList(RouteUri, RouteTemplate);
                // TODO: Add info logs
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingTypeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ShippingTypeListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get a ShippingType.
        /// </summary>
        /// <param name="shippingTypeId">ID of Shipping type to get the details.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(ShippingTypeResponse))]
        [HttpGet]
        public HttpResponseMessage Get(int shippingTypeId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _shippingTypeCache.GetShippingType(shippingTypeId, RouteUri, RouteTemplate);
                // TODO: Add info logs
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingTypeResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ShippingTypeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create new Shipping Type.
        /// </summary>
        /// <param name="shippingTypeModel">ShippingType Model to create new Shipping type.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(ShippingTypeResponse))]
        [HttpPost]
        public HttpResponseMessage Create([FromBody] ShippingTypeModel shippingTypeModel)
        {
            HttpResponseMessage response;
            try
            {
                ShippingTypeModel shippingType = _shippingTypeService.CreateShippingType(shippingTypeModel);

                if (!Equals(shippingType, null))
                {
                    response = CreateCreatedResponse(new ShippingTypeResponse { ShippingType = shippingType });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(shippingType.ShippingTypeId)));
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
                response = CreateInternalServerErrorResponse(new ShippingTypeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Shipping type.
        /// </summary>
        /// <param name="shippingTypeModel">ShippingType Model to update Shipping type.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(ShippingTypeResponse))]
        [HttpPut]
        public HttpResponseMessage Update([FromBody] ShippingTypeModel shippingTypeModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _shippingTypeService.UpdateShippingType (shippingTypeModel);
                response = isUpdated ? CreateOKResponse(new ShippingTypeResponse { ShippingType = shippingTypeModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ShippingTypeResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Shipping type
        /// </summary>
        /// <param name="entityIds">IDs of Shipping type to delete.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage Delete([FromBody] ParameterModel entityIds)
        {
            HttpResponseMessage response;
            try
            {
                bool isDeleted = _shippingTypeService.DeleteShippingType(entityIds);
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
        /// Get all Shipping Types which are not present in database.
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(ShippingTypeListResponse))]
        [HttpGet]
        public HttpResponseMessage GetAllShippingTypesNotInDatabase()
        {
            HttpResponseMessage response;
            try
            {
                string data = _shippingTypeCache.GetAllShippingTypesNotInDatabase(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ShippingTypeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ShippingTypeListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Bulk enable or disable shipping types.
        /// </summary>
        /// <param name="entityIds">Ids of shipping types.</param>
        /// <param name="isEnable">boolean value true/false</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage BulkEnableDisableShippingTypes([FromBody] ParameterModel entityIds, bool isEnable)
        {
            HttpResponseMessage response;
            try
            {
                bool isEnabled = _shippingTypeService.EnableDisableShippingType(entityIds, isEnable);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isEnabled });
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
