using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class GeneralSettingCache : BaseCache, IGeneralSettingCache
    {
        #region Private Variable
        private readonly IGeneralSettingService _service;
        #endregion

        #region Public Constructor
        public GeneralSettingCache(IGeneralSettingService Service)
        {
            _service = Service;
        }
        #endregion

        #region Public Methods
        //Get a list of all countries.
        public virtual string List(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                GeneralSettingModel list = _service.List();
                if (!Equals(list, null))
                {
                    GeneralSettingResponse response = new GeneralSettingResponse { GeneralSetting = list };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        } 

        //Gets a list of Cache Management data
        public virtual string GetCacheManagementData(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CacheListModel cacheData = _service.GetCacheData();
                if (!Equals(cacheData, null))
                {
                    GeneralSettingResponse response = new GeneralSettingResponse { CacheData = cacheData };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get global configuration settings for application.
        public string GetConfigurationSettings(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ConfigurationSettingModel configurationSettingModel = _service.GetConfigurationSettings();
                if (!Equals(configurationSettingModel, null))
                {
                    GeneralSettingResponse response = new GeneralSettingResponse { ConfigurationSetting = configurationSettingModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets the Power BI setting details
        public virtual string GetPowerBISettings(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PowerBISettingsModel powerBISettings = _service.GetPowerBISettings();
                if (!Equals(powerBISettings, null))
                {
                    GeneralSettingResponse response = new GeneralSettingResponse { PowerBISettings = powerBISettings };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        // Gets stock notification setting details.
        public virtual string GetStockNoticeSettings(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                StockNoticeSettingsModel stockNoticeSettings = _service.GetStockNoticeSettings();
                if (!Equals(stockNoticeSettings, null))
                {
                    GeneralSettingResponse response = new GeneralSettingResponse { StockNoticeSettings = stockNoticeSettings };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}