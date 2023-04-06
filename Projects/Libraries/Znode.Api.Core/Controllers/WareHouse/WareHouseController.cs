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
    public class WarehouseController : BaseController
    {
        #region Private Variables

        private readonly IWarehouseCache _cache;
        private readonly IWarehouseService _service;

        #endregion

        #region Constructor
        public WarehouseController(IWarehouseService service)
        {
            _service = service;
            _cache = new WarehouseCache(_service);
        }
        #endregion

        #region Public Methods

        #region Warehouse
        /// <summary>
        /// Gets list of Warehouse.
        /// </summary>
        /// <returns>Returns list of Warehouse.</returns>
        [HttpGet]
        [ResponseType(typeof(WarehouseListResponse))]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetWarehouseList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<WarehouseListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WarehouseListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Create warehouse.
        /// </summary>
        /// <param name="warehouseModel">warehouseModel model.</param>
        /// <returns>Returns created model.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(WarehouseResponse))]
        public virtual HttpResponseMessage Create([FromBody] WarehouseModel warehouseModel)
        {
            HttpResponseMessage response;

            try
            {
                WarehouseModel warehouse = _service.CreateWarehouse(warehouseModel);

                if (HelperUtility.IsNotNull(warehouse))
                {
                    response = CreateCreatedResponse(new WarehouseResponse { Warehouse = warehouse });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(warehouse.WarehouseId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WarehouseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WarehouseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get warehouse by warehouse id.
        /// </summary>
        /// <param name="warehouseId">Warehouse id to get warehouse details.</param>
        /// <returns>Returns warehouse model.</returns>
        [HttpGet]
        [ResponseType(typeof(WarehouseResponse))]
        public virtual HttpResponseMessage GetWarehouse(int warehouseId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetWarehouse(warehouseId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<WarehouseResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WarehouseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WarehouseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update warehouse.
        /// </summary>
        /// <param name="warehouseModel">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [HttpPut]
        [ResponseType(typeof(WarehouseResponse))]
        public virtual HttpResponseMessage Update([FromBody] WarehouseModel warehouseModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update warehouse.
                bool warehouse = _service.UpdateWarehouse(warehouseModel);
                response = warehouse ? CreateCreatedResponse(new WarehouseResponse { Warehouse = warehouseModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(warehouseModel.WarehouseId)));
            }

            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WarehouseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WarehouseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete warehouse.
        /// </summary>
        /// <param name="warehouseId">Warehouse Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel warehouseId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteWarehouse(warehouseId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
            }

            return response;
        }
        #endregion

        #region Associate inventory
        /// <summary>
        /// Get list of associated inventory for warehouse.
        /// </summary>
        /// <returns>Returns associated inventory list.</returns>
        [HttpGet]
        [ResponseType(typeof(InventoryWarehouseMapperListResponse))]
        public virtual HttpResponseMessage GetAssociatedInventoryList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedInventoryList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<InventoryWarehouseMapperListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new InventoryWarehouseMapperListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
        #endregion
    }
}
