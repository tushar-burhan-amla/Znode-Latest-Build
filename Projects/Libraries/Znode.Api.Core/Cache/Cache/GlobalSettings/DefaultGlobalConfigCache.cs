using System.Collections.Generic;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class DefaultGlobalConfigCache : BaseCache, IDefaultGlobalConfigCache
    {
        #region Private Variables
        private readonly IDefaultGlobalConfigService _service;
        #endregion

        #region Public Constructor
        public DefaultGlobalConfigCache(IDefaultGlobalConfigService defaultGlobalConfigService)
        {
            _service = defaultGlobalConfigService;
        }
        #endregion

        #region Public Methods
        //Get DefaultGlobalConfig List
        public virtual DefaultGlobalConfigListResponse GetDefaultGlobalConfigList()
        {
            //Get data from cache.
            DefaultGlobalConfigListModel data = Equals(HttpRuntime.Cache[CachedKeys.DefaultGlobalConfigCache], null)
                ? DefaultGlobalConfigSettingHelper.GetDefaultGlobalConfigSettings()
                : (DefaultGlobalConfigListModel)HttpRuntime.Cache.Get(CachedKeys.DefaultGlobalConfigCache);

            DefaultGlobalConfigListResponse response = new DefaultGlobalConfigListResponse { DefaultGlobalConfigs = data.DefaultGlobalConfigs };
            return response;
        }
        //Get Logging Configuration Setting Dictionary
        public virtual DefaultGlobalConfigListResponse GetDefaultLoggingConfigSettings()
        {
            //Get data from cache.
            Dictionary<string, string> data = Equals(HttpRuntime.Cache[CachedKeys.DefaultLoggingConfigCache], null)
                ? DefaultGlobalConfigSettingHelper.GetDefaultLoggingConfigSettings()
                : (Dictionary<string, string>)HttpRuntime.Cache.Get(CachedKeys.DefaultLoggingConfigCache);

            DefaultGlobalConfigListResponse response = new DefaultGlobalConfigListResponse { LoggingConfigurationSettings = data };
            return response;
        }

        #endregion
    }
}
