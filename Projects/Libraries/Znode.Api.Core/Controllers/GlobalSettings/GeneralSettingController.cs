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
    public class GeneralSettingController : BaseController
    {
        #region Private Variables
        private readonly IGeneralSettingService _service;
        private readonly IGeneralSettingCache _cache;
        #endregion

        #region Public Constructor
        public GeneralSettingController(IGeneralSettingService service)
        {
            _service = service;
            _cache = new GeneralSettingCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///Get List of General Settings
        /// </summary>
        /// <returns>List of General Settings</returns>
        [ResponseType(typeof(GeneralSettingResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.List(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GeneralSettingResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                GeneralSettingResponse data = new GeneralSettingResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update General Settings.
        /// </summary>
        /// <param name="model">model of general settings values</param>
        /// <returns>Updated General Settings.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody]GeneralSettingModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool isSuccess = _service.Update(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isSuccess });
            }
            catch (ZnodeException ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #region Cache Management

        /// <summary>
        /// Gets Cache Management Data
        /// </summary>
        /// <returns>Cache Management Data</returns>
        [ResponseType(typeof(GeneralSettingResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCacheData()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCacheManagementData(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GeneralSettingResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                GeneralSettingResponse data = new GeneralSettingResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                GeneralSettingResponse data = new GeneralSettingResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Updates/Creates provide Cache data.
        /// </summary>
        /// <param name="model">Model to be updated/created</param>
        /// <returns>updated/created Cache Model</returns>
        [ResponseType(typeof(GeneralSettingResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateUpdateCache([FromBody]CacheListModel model)
        {
            HttpResponseMessage response;
            try
            {
                if (HelperUtility.IsNotNull(model))
                {
                    response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.CreateUpdateCache(model) });
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// //Updates/Creates provide Cache data.
        /// </summary>
        /// <param name="model">Model to be updated/created</param>
        /// <returns>updated/created Cache Model</returns>
        [ResponseType(typeof(GeneralSettingResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage RefreshCache([FromBody]CacheModel model)
        {
            HttpResponseMessage response;
            try
            {
                CacheModel cacheModel = _service.RefreshCacheData(model);
                if (HelperUtility.IsNotNull(model))
                {
                    response = CreateCreatedResponse(new GeneralSettingResponse { Cache = cacheModel });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(cacheModel.ApplicationCacheId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new GeneralSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new GeneralSettingResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }
        #endregion

        #endregion

        #region Get Configuration Settings

        /// <summary>
        /// Get global configuration settings for application.
        /// </summary>
        /// <returns> Global configuration settings for application.</returns>
        [ResponseType(typeof(GeneralSettingResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetConfigurationSettings()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetConfigurationSettings(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GeneralSettingResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                GeneralSettingResponse data = new GeneralSettingResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                GeneralSettingResponse data = new GeneralSettingResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Configuration Settings
        /// </summary>
        /// <param name="model">Configuration Setting Model for coupon and Promotion</param>
        /// <returns>true or false status</returns>        
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateConfigurationSettings(ConfigurationSettingModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool updated = _service.UpdateConfigurationSettings(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = updated });
            }
            catch (ZnodeException ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { IsSuccess = false, HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { IsSuccess = false, HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion

        #region Power BI

        /// <summary>
        /// Gets Power BI Setting details
        /// </summary>
        /// <returns>returns Power BI setting data</returns>
        [ResponseType(typeof(GeneralSettingResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPowerBISettings()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPowerBISettings(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GeneralSettingResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                GeneralSettingResponse data = new GeneralSettingResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                GeneralSettingResponse data = new GeneralSettingResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update the Power BI settings
        /// </summary>
        /// <returns>returns response</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdatePowerBISettings(PowerBISettingsModel powerBISettingsModel)
        {
            HttpResponseMessage response;

            try
            {
                bool updated = _service.UpdatePowerBISettings(powerBISettingsModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = updated });
            }
            catch (ZnodeException ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { IsSuccess = false, HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { IsSuccess = false, HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion

        #region stock notification

        /// <summary>
        /// Gets stock notice setting details
        /// </summary>
        /// <returns>stock notice model</returns>
        [ResponseType(typeof(GeneralSettingResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetStockNoticeSettings()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetStockNoticeSettings(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GeneralSettingResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                GeneralSettingResponse data = new GeneralSettingResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                GeneralSettingResponse data = new GeneralSettingResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update stock notice settings
        /// </summary>
        /// <returns>response</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateStockNoticeSettings(StockNoticeSettingsModel stockNoticeSettingsModel)
        {
            HttpResponseMessage response;

            try
            {
                bool updated = _service.UpdateStockNoticeSettings(stockNoticeSettingsModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = updated });
            }
            catch (ZnodeException ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { IsSuccess = false, HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { IsSuccess = false, HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

    }
}
