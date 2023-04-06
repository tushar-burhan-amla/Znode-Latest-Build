using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IGeneralSettingAgent
    {
        /// <summary>
        /// Method To get Lists of General Settings
        /// </summary>
        /// <returns></returns>
        GlobalSettingViewModel List();

        /// <summary>
        /// Update The Existing General Setting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Update(GlobalSettingViewModel model);

        /// <summary>
        /// Get filtered list of PublishState-ApplicationType mapping.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PublishStateMappingListViewModel GetPublishStateMappingList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get all available publish state mappings.
        /// </summary>
        /// <returns></returns>
        IEnumerable<PublishStateMappingViewModel> GetAvailablePublishStateMappings();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<PublishStateMappingViewModel> GetAvailablePublishStatesforPreview();

        /// <summary>
        /// Get round off list.
        /// </summary>
        /// <returns>Returns round off list.</returns>
        List<SelectListItem> GetRoundOffList();

        /// <summary>
        /// Get environment list
        /// </summary>
        /// <returns>Returns environments list</returns>
        List<SelectListItem> GetEnvironmentsList();

        /// <summary>
        /// Method to get ApplicationPool, ApiCache or FullPageCache
        /// </summary>
        /// <returns>Returns Model containing cache data</returns>
        CacheListViewModel CacheData();

        /// <summary>
        /// Mehtod to update cache status
        /// </summary>
        /// <param name="cacheListViewModel">model to update</param>
        /// <returns>Returns true if record is updated successfully.</returns>
        bool UpdateCacheStatus(CacheListViewModel cacheListViewModel, ref string errorMessage);

        /// <summary>
        /// Method to update cache status.
        /// </summary>
        /// <param name="cacheViewModel"></param>
        /// <returns></returns>
        CacheViewModel RefreshCache(CacheViewModel cacheViewModel);

        /// <summary>
        /// Enable /Disable publish state to application type mapping.
        /// </summary>
        /// <param name="publishStateMappingId"></param>
        /// <param name="isEnabled">Supply true to enable.</param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool EnableDisablePublishStateMapping(int publishStateMappingId, bool isEnabled, out string message);


        /// <summary>
        /// Get global configuration settings for application.
        /// </summary>
        /// <returns>Configuration setting </returns>
        ConfigurationSettingViewModel GetConfigurationSettings();

        /// <summary>
        /// Update global configuration settings for application.
        /// </summary>
        /// <param name="model">Configuration settings </param>
        /// <returns>True or False status</returns>
        bool UpdateConfigurationSettings(ConfigurationSettingViewModel model);

        /// <summary>
        /// Get the Power BI setting details
        /// </summary>
        /// <returns>returns Power Bi details</returns>
        PowerBISettingsViewModel GetPowerBISettings();

        /// <summary>
        /// Update the Power BI setting details
        /// </summary>
        /// <param name="powerBISettingsViewModel"></param>
        /// <returns>returns true or false</returns>
        bool UpdatePowerBISettings(PowerBISettingsViewModel powerBISettingsViewModel);

        /// <summary>
        /// Get stock notice settings model
        /// </summary>
        /// <returns>stock notice model</returns>
        StockNoticeSettingsViewModel GetStockNoticeSettings();

        /// <summary>
        /// Update the stock notice settings model 
        /// </summary>
        /// <param name="stockNoticeSettingsViewModel">stockNoticeSettingsViewModel</param>
        /// <returns>Status</returns>
        bool UpdateStockNoticeSettings(StockNoticeSettingsViewModel stockNoticeSettingsViewModel);
    }
}