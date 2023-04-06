using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class DefaultGlobalConfigService : BaseService, IDefaultGlobalConfigService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeGlobalSetting> _defaultGlobalConfigRepository;
        #endregion

        #region  Public constructor
        public DefaultGlobalConfigService()
        {
            //Initialize instance of repository.
            _defaultGlobalConfigRepository = new ZnodeRepository<ZnodeGlobalSetting>();
        }
        #endregion

        #region Public Methods
        //Get a list of DefaultGlobalConfigModel
        public virtual DefaultGlobalConfigListModel GetDefaultGlobalConfigList()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            try
            {
                List<ZnodeGlobalSetting> defaultGlobalConfigList = new List<ZnodeGlobalSetting>(_defaultGlobalConfigRepository.Table);
                ZnodeLogging.LogMessage("defaultGlobalConfigList list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, defaultGlobalConfigList?.Count());
                ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(defaultGlobalConfigList) ? new DefaultGlobalConfigListModel() { DefaultGlobalConfigs = defaultGlobalConfigList.ToModel<DefaultGlobalConfigModel>().ToList() } : null;

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }
        //Get a Dictionary of Logging Configuration Settings
        public virtual Dictionary<string, string> GetLoggingGlobalConfigList()
        {
            try
            {
                List<string> globalSettingLoggingEnums = Enum.GetNames(typeof(LoggingSettingEnum)).Cast<string>().ToList();
                List<ZnodeGlobalSetting> defaultGlobalConfigList = new List<ZnodeGlobalSetting>(_defaultGlobalConfigRepository.Table.Where(m => globalSettingLoggingEnums.Contains(m.FeatureName)));

                Dictionary<string, string> loggingConfigSettings = defaultGlobalConfigList.ToDictionary(n => n.FeatureName, v => v.FeatureValues);

                return loggingConfigSettings?.Count > 0 ? loggingConfigSettings : new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, ex.Message);
            }
        }

        #endregion
    }
}
