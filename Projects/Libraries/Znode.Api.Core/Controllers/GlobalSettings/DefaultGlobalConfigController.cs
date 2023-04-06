using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class DefaultGlobalConfigController : BaseController
    {
        #region Private Variables
        private readonly IDefaultGlobalConfigCache _cache;
        #endregion

        #region Default Constructor
        public DefaultGlobalConfigController(IDefaultGlobalConfigService service)
        {
            _cache = new DefaultGlobalConfigCache(service);
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Gets list of all DefaultGlobalConfig.
        /// </summary>
        /// <returns>DefaultGlobalConfig List</returns>
        [ResponseType(typeof(DefaultGlobalConfigListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                DefaultGlobalConfigListResponse data = _cache.GetDefaultGlobalConfigList();
                response = !Equals(data, null) ? CreateOKResponse(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                DefaultGlobalConfigListResponse data = new DefaultGlobalConfigListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Error);
            }
            return response;
        }
        /// <summary>
        /// Gets list of all Default Logging Setting
        /// </summary>
        /// <returns>DefaultGlobalConfig List</returns>
        [ResponseType(typeof(DefaultGlobalConfigListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDefaultLoggingConfig()
        {
            HttpResponseMessage response;

            try
            {
                DefaultGlobalConfigListResponse data = _cache.GetDefaultLoggingConfigSettings();
                response = !Equals(data, null) ? CreateOKResponse(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                DefaultGlobalConfigListResponse data = new DefaultGlobalConfigListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}

