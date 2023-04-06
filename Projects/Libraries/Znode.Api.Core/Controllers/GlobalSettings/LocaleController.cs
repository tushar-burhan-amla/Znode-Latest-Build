using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Controllers
{
    public class LocaleController : BaseController
    {
        #region Private Variables
        private readonly ILocaleService _service;
        private readonly ILocaleCache _cache;
        #endregion

        #region Default Constructor
        public LocaleController(ILocaleService service)
        {
            _service = service;
            _cache = new LocaleCache(_service);
        }
        #endregion

        #region Public method
        /// <summary>
        /// Get Locale list.
        /// </summary>
        /// <returns>Returns list of all Locales.</returns>
        [ResponseType(typeof(LocaleListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetLocaleList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetLocaleList(RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<LocaleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Error);
                var data = new LocaleListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Get default Locale.
        /// </summary>
        /// <returns>Returns a Locale.</returns>
        [ResponseType(typeof(LocaleResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetLocale()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetLocale(RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<LocaleResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Error);
                var data = new LocaleResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Update Locale details.
        /// </summary>
        /// <param name="defaultGlobalConfigListModel">DefaultGlobalConfigListModel to be updated</param>
        /// <returns>HttpResponse for Default Global Config List Model.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody]DefaultGlobalConfigListModel defaultGlobalConfigListModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isSuccess = _service.UpdateLocale(defaultGlobalConfigListModel);
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
                var data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Error);
            }

            return response;
        }

        #endregion
    }
}
