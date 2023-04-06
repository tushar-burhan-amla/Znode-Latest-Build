using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IGeneralSettingService
    {
        /// <summary>
        /// Method To get List Of All General Setting
        /// </summary>
        /// <returns>model of general setting list</returns>
        GeneralSettingModel List();

        /// <summary>
        /// Method To Update Existing General Setting
        /// </summary>
        /// <param name="model">general setting model with values to update</param>
        /// <returns>true/false</returns>
        bool Update(GeneralSettingModel model);

        /// <summary>
        /// Method to get Cache Management data
        /// </summary>
        /// <returns>model containing Cache Management Data</returns>
        CacheListModel GetCacheData();

        /// <summary>
        /// Updates provide Cache data if already exists, otherwise creates new entry.
        /// </summary>
        /// <param name="cacheListModel"></param>
        /// <returns>return true if updated or created successfully else return false.</returns>
        bool CreateUpdateCache(CacheListModel cacheListModel);

        /// <summary>
        /// Refresh cache data.
        /// </summary>
        /// <param name="cacheModel"></param>
        /// <returns>refresh cache cacheModel.</returns>
        CacheModel RefreshCacheData(CacheModel cacheModel);

        /// <summary>
        /// Get global configuration settings for application.
        /// </summary>
        /// <returns>Configuration settings details</returns>
        ConfigurationSettingModel GetConfigurationSettings();

        /// <summary>
        /// Update global configuration settings for application.
        /// </summary>
        /// <param name="model">Configuration setting details</param>
        /// <returns>True or False status</returns>
        bool UpdateConfigurationSettings(ConfigurationSettingModel model);

        /// <summary>
        /// Get the Power BI setting details
        /// </summary>
        /// <returns>returns Power BI setting</returns>
        PowerBISettingsModel GetPowerBISettings();

        /// <summary>
        /// Update the Power BI setting data
        /// </summary>
        /// <param name="powerBiDetailsModel"></param>
        /// <returns>returns true or false</returns>
        bool UpdatePowerBISettings(PowerBISettingsModel powerBISettingsModel);

        /// <summary>
        /// Get the stock notice setting.
        /// </summary>
        /// <returns>Stock Notice Model</returns>
        StockNoticeSettingsModel GetStockNoticeSettings();

        /// <summary>
        /// Update the stock notice setting data.
        /// </summary>
        /// <param name="stockNoticeSettingsModel">stockNoticeSettingsModel</param>
        /// <returns>Status</returns>
        bool UpdateStockNoticeSettings(StockNoticeSettingsModel stockNoticeSettingsModel);

    }
}
