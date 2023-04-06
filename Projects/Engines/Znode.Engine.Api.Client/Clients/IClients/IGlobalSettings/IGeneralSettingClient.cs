using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IGeneralSettingClient : IBaseClient
    {
        /// <summary>
        /// Method to get list of all GeneralSettings.
        /// </summary>
        /// <returns>model contains list of general settings</returns>
        GeneralSettingModel list();

        /// <summary>
        /// Method to ppdate existing GeneralSettings.
        /// </summary>
        /// <param name="generalSettingModel"></param>
        /// <returns>true/false</returns>
        bool Update(GeneralSettingModel generalSettingModel);

        /// <summary>
        /// Get filtered list for PublishState - ApplicationType mapping.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PublishStateMappingListModel GetPublishStateMappingList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Method to get Cache Management data 
        /// </summary>
        /// <returns>model containing cache management data</returns>
        CacheListModel GetCacheData();

        /// <summary>
        /// Method to Create/Update cache data.
        /// </summary>
        /// <param name="cacheListModel">model to be updated</param>
        /// <returns>return true if updated or created successfully else return false.</returns>
        bool CreateUpdateCacheData(CacheListModel cacheListModel);

        /// <summary>
        /// Method to refresh cache data.
        /// </summary>
        /// <param name="cacheModel">model to be updated</param>
        /// <returns>refresh cachemodel</returns>
        CacheModel RefreshCacheData(CacheModel cacheModel);

        /// <summary>
        /// Method to Enable or Disable publish state to application type mapping.
        /// </summary>
        /// <param name="publishStateMappingId"></param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        bool EnableDisablePublishStateMapping(int publishStateMappingId, bool isEnabled);

        /// <summary>
        /// Get global configuration settings for application.
        /// </summary>
        /// <returns>Configuration settings details</returns>
        ConfigurationSettingModel GetConfigurationSettings();

        /// <summary>
        /// Update global configuration settings for application.
        /// </summary>
        /// <param name="configurationSettingModel">global configuration settings</param>
        /// <returns>true or false status</returns>
        bool UpdateConfigurationSettings(ConfigurationSettingModel configurationSettingModel);

        /// <summary>
        /// Get the Power BI setting details
        /// </summary>
        /// <returns></returns>
        PowerBISettingsModel GetPowerBISettings();

        /// <summary>
        /// Update the Power BI setting data
        /// </summary>
        /// <param name="powerBiSettingModel"></param>
        /// <returns></returns>
        bool UpdatePowerBISettings(PowerBISettingsModel powerBiSettingModel);

        /// <summary>
        /// Get the stock notice settings details.
        /// </summary>
        /// <returns>StockNoticeSettingsModel</returns>
        StockNoticeSettingsModel GetStockNoticeSettings();

        /// <summary>
        /// Update the stock notice settings data.
        /// </summary>
        /// <param name="stockNoticeSettingsModel">stockNoticeSettingsModel</param>
        /// <returns>StockNoticeModel</returns>
        bool UpdateStockNoticeSettings(StockNoticeSettingsModel stockNoticeSettingsModel);
    }
}
