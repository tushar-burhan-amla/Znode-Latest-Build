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
    public class StoreLocatorController : BaseController
    {
        #region Private Variables
        private readonly IStoreLocatorService _service;
        private readonly IStoreLocatorCache _cache;
        #endregion

        #region Public Constructor
        public StoreLocatorController(IStoreLocatorService service)
        {
            _service = service;
            _cache = new StoreLocatorCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get a Store By id.
        /// </summary>
        /// <param name="storeId">The ID of the Store.</param>
        /// <returns>Return store depend upon given ID.</returns>
        [ResponseType(typeof(StoreLocatorResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int storeId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetStoreLocator(storeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<StoreLocatorResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                StoreLocatorResponse data = new StoreLocatorResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Get a Store By store location code.
        /// </summary>
        /// <param name="storeLocationCode">The code of the Store location.</param>
        /// <returns>Return store depend upon given ID.</returns>
        [ResponseType(typeof(StoreLocatorResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetStoreLocatorByCode(string storeLocationCode)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetStoreLocator(storeLocationCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<StoreLocatorResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                StoreLocatorResponse data = new StoreLocatorResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Get list of store which are near to given location.
        /// </summary>
        /// <returns>Return list of store which are near to given location.</returns>
        [ResponseType(typeof(StoreLocatorListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetStoreLocatorList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<StoreLocatorListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                StoreLocatorListResponse data = new StoreLocatorListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Creates a new store locator.
        /// </summary>
        /// <param name="model">The model with data of store.</param>
        /// <returns></returns>
        [ResponseType(typeof(StoreLocatorResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] StoreLocatorDataModel model)
        {

            HttpResponseMessage response;
            try
            {
                StoreLocatorDataModel data = _service.Create(model);
                if (IsNotNull(data))
                {
                    response = CreateCreatedResponse(new StoreLocatorResponse { storeLocatorModel = data });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(data.PortalAddressId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                StoreLocatorResponse data = new StoreLocatorResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                StoreLocatorResponse data = new StoreLocatorResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Update an existing store.
        /// </summary>
        /// <param name="model">The model with data of store</param>
        /// <returns></returns>
        [ResponseType(typeof(StoreLocatorResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] StoreLocatorDataModel model)
        {

            HttpResponseMessage response;
            try
            {
                bool store = _service.UpdateStoreLocator(model);
                response = store ? CreateCreatedResponse(new StoreLocatorResponse { storeLocatorModel = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.PortalAddressId)));

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                StoreLocatorResponse data = new StoreLocatorResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete an existing store.
        /// </summary>
        /// <param name="storeIds"></param>
        /// <param name="isDeleteByCode">if true then store location data will delete by store location code</param>
        /// <returns>Return true if deleted successfully else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Delete(ParameterModel storeIds,bool isDeleteByCode)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteStoreLocator(storeIds,isDeleteByCode);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion
    }
}
