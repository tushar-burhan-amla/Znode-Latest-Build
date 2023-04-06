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
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Controllers
{
    public class InventoryController : BaseController
    {
        #region Private Variables

        private readonly IInventoryCache _cache;
        private readonly IInventoryService _service;

        #endregion

        #region Constructor
        public InventoryController(IInventoryService service)
        {
            _service = service;
            _cache = new InventoryCache(_service);
        }
        #endregion

        #region Public Methods

        #region SKU Inventory
        /// <summary>
        /// Method to create SKU Inventory.
        /// </summary>
        /// <param name="inventorySKU">InventorySKUModel model.</param>
        /// <returns>Returns created SKU Inventory.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(InventorySKUResponse))]
        public virtual HttpResponseMessage AddSKUInventory([FromBody] InventorySKUModel inventorySKU)
        {
            HttpResponseMessage response;
            try
            {
                inventorySKU = _service.AddSKUInventory(inventorySKU);
                if (IsNotNull(inventorySKU))
                {
                    response = CreateCreatedResponse(new InventorySKUResponse { InventorySKU = inventorySKU });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(inventorySKU.InventoryId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new InventorySKUResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new InventorySKUResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// To get SKU Inventory by Inventory id.
        /// </summary>
        /// <param name="inventoryId">Inventory Id</param>
        /// <returns>InventorySKUModel model</returns>   
        [HttpGet]
        [ResponseType(typeof(InventorySKUResponse))]
        public virtual HttpResponseMessage GetSKUInventory(int inventoryId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSKUInventory(inventoryId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<InventorySKUResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new InventorySKUResponse { HasError = true, ErrorMessage = ex.Message});
            }
            return response;
        }

        /// <summary>
        /// Get List of SKU Inventory.
        /// </summary>
        /// <returns>SKU Inventory List.</returns>
        [HttpGet]
        [ResponseType(typeof(InventorySKUListResponse))]
        public virtual HttpResponseMessage GetSKUInventoryList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSKUInventoryList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<InventorySKUListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new InventorySKUListResponse { HasError = true, ErrorMessage = ex.Message});
            }
            return response;
        }

        /// <summary>
        /// Update SKU Inventory.
        /// </summary>
        /// <param name="inventorySKUModel">InventorySKUModel.</param>
        /// <returns>Updated InventorySKU model.</returns>
        [HttpPut]
        [ResponseType(typeof(InventorySKUResponse))]
        public virtual HttpResponseMessage UpdateSKUInventory([FromBody] InventorySKUModel inventorySKUModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateSKUInventory(inventorySKUModel);
                response = data ? CreateCreatedResponse(new InventorySKUResponse { InventorySKU = inventorySKUModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new InventorySKUResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new InventorySKUResponse { HasError = true, ErrorMessage = ex.Message});
            }
            return response;
        }

        /// <summary>
        /// Delete SKU Inventory.
        /// </summary>
        /// <param name="inventoryId">InventoryId to delete SKU inventory.</param>
        /// <returns>true if deleted.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage DeleteSKUInventory(ParameterModel inventoryId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteSKUInventory(inventoryId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion


        #region Digital Asset

        /// <summary>
        /// Get downloadable product key list.
        /// </summary>
        /// <returns>Keys list</returns>
        [ResponseType(typeof(DownloadableProductKeyListResponse))]
        [HttpGet]
        public HttpResponseMessage GetDownloadableProductKeyList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDownloadableProductKeyList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DownloadableProductKeyListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(),TraceLevel.Error);
                DownloadableProductKeyListResponse data = new DownloadableProductKeyListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Method to add downloadable product keys.
        /// </summary>
        /// <param name="downloadableProductKeyModel">DownloadableProductKeyModel model.</param>
        /// <returns>Returns added downloadable product keys.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage AddDownloadableProductKeys([FromBody] DownloadableProductKeyModel downloadableProductKeyModel)
        {
            HttpResponseMessage response;
            try
            {
                DownloadableProductKeyModel productKeyModel = _service.AddDownloadableProductKeys(downloadableProductKeyModel);
                if (!Equals(productKeyModel, null))
                {
                    response = CreateCreatedResponse(new DownloadableProductKeyResponse { DownloadableProductKey = productKeyModel });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(downloadableProductKeyModel.PimDownloadableProductKeyId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(),TraceLevel.Warning);
                DownloadableProductKeyResponse data = new DownloadableProductKeyResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.DuplicateProductKey };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(),TraceLevel.Error);
                DownloadableProductKeyResponse data = new DownloadableProductKeyResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.DuplicateProductKey };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Update Downloadable Product Key.
        /// </summary>
        /// <param name="downloadableProductKeyModel">DownloadableProductKeyModel to update product key</param>
        /// <returns>Retrun status as per update operation.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateDownloadableProductKey(DownloadableProductKeyModel downloadableProductKeyModel)
        {
            HttpResponseMessage response;
            try
            {
                bool updated = _service.UpdateDownloadableProductKey(downloadableProductKeyModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = updated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete Delete Downloadable Product Keys.
        /// </summary>
        /// <param name="PimDownloadableProductKeyId">PimDownloadableProductKeyId to delete DeleteDownloadableProductKeys</param>
        /// <returns>Retrun status as per delete operation.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage DeleteDownloadableProductKeys(ParameterModel pimDownloadableProductKeyId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteDownloadableProductKeys(pimDownloadableProductKeyId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(),TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion

        #endregion
    }
}
