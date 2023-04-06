using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.Libraries.Data.Helpers;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class GeneralSettingAgent : BaseAgent, IGeneralSettingAgent
    {
        #region Private Variables
        private readonly IGeneralSettingClient _generalSettingClient;
        #endregion

        #region public Constructor
        public GeneralSettingAgent(IGeneralSettingClient generalSettingClient)
        {
            _generalSettingClient = GetClient<IGeneralSettingClient>(generalSettingClient);
        }
        #endregion

        #region public virtual Methods
        //Method To get Lists of General Settings
        public virtual GlobalSettingViewModel List()
            => GeneralSettingMap.ToViewModel(_generalSettingClient.list());

        //Update The Existing General Setting
        public virtual bool Update(GlobalSettingViewModel model)
                => _generalSettingClient.Update(model.ToModel<GeneralSettingModel>());

        // Get filtered list of PublishState-ApplicationType mapping.
        public virtual PublishStateMappingListViewModel GetPublishStateMappingList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            if (HelperUtility.IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.DisplayOrder, DynamicGridConstants.ASCKey);
            }

            //Checking For LocaleId already Exists in Filters Or Not
            PublishStateMappingListModel publishStateMappingListModel = _generalSettingClient.GetPublishStateMappingList(filters, sorts, pageIndex, pageSize);
            PublishStateMappingListViewModel listViewModel = new PublishStateMappingListViewModel { PublishStateMappingList = publishStateMappingListModel?.PublishStateMappingList?.ToViewModel<PublishStateMappingViewModel>().ToList() };
            SetListPagingData(listViewModel, publishStateMappingListModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return publishStateMappingListModel?.PublishStateMappingList?.Count > 0 ? listViewModel : new PublishStateMappingListViewModel() { PublishStateMappingList = new List<PublishStateMappingViewModel>() };
        }

        public virtual IEnumerable<PublishStateMappingViewModel> GetAvailablePublishStateMappings()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();

            SetIsEnabledFilter(filters, true);

            PublishStateMappingListModel publishStateMappingListModel = _generalSettingClient.GetPublishStateMappingList(filters, null, null, null);

            IEnumerable<PublishStateMappingViewModel> resultset = publishStateMappingListModel?.PublishStateMappingList?.ToViewModel<PublishStateMappingViewModel>().ToList();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return resultset;
        }

        public virtual IEnumerable<PublishStateMappingViewModel> GetAvailablePublishStatesforPreview()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);

            PublishStateMappingListModel publishStateMappingListModel = _generalSettingClient.GetPublishStateMappingList(null, null, null, null);

            IEnumerable<PublishStateMappingViewModel> resultset = publishStateMappingListModel?.PublishStateMappingList?.ToViewModel<PublishStateMappingViewModel>().ToList();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return resultset;
        }

        //Enable/Disable publish state to application type mapping.
        public virtual bool EnableDisablePublishStateMapping(int publishStateMappingId, bool isEnabled, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);

            errorMessage = string.Empty;

            if (publishStateMappingId > 0)
            {
                try
                {
                    return _generalSettingClient.EnableDisablePublishStateMapping(publishStateMappingId, isEnabled);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.NotPermitted:
                            errorMessage = Admin_Resources.DefaultPublishStateDisableErrorMessage;
                            break;
                        default:
                            errorMessage = Admin_Resources.ErrorMessageEnableDisablePublishState;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorMessageEnableDisablePublishState;
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);

            return false;
        }

        #region Cache Management
        //Method to get cache management data
        public virtual CacheListViewModel CacheData()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            CacheListModel listModel = _generalSettingClient.GetCacheData();
            if (IsNull(listModel))
            {
                return new CacheListViewModel() { CacheList = new List<CacheViewModel>() };
            }
            CacheListViewModel listViewModel = new CacheListViewModel() { CacheList = listModel?.CacheData?.ToViewModel<CacheViewModel>().ToList()};

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return listModel?.CacheData?.Count > 0 ? listViewModel : new CacheListViewModel() { CacheList = new List<CacheViewModel>() };
        }

        //Method to update cache status
        public virtual CacheViewModel RefreshCache(CacheViewModel cacheViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                CacheViewModel updatedModel = _generalSettingClient.RefreshCacheData(cacheViewModel.ToModel<CacheModel>())?.ToViewModel<CacheViewModel>();
                updatedModel.SuccessMessage = Admin_Resources.CacheClearedMessage;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(updatedModel) ? updatedModel : new CacheViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
                if (ex.ErrorCode == ErrorCodes.InvalidData)
                    return (CacheViewModel)GetViewModelWithErrorMessage(cacheViewModel, ex.Message);

                return (CacheViewModel)GetViewModelWithErrorMessage(cacheViewModel, Admin_Resources.CacheNotClearedMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                return (CacheViewModel)GetViewModelWithErrorMessage(cacheViewModel, Admin_Resources.CacheNotClearedMessage);
            }
        }

        //Method to update cache status
        public virtual bool UpdateCacheStatus(CacheListViewModel cacheViewModel, ref string errorMessage)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                return _generalSettingClient.CreateUpdateCacheData(new CacheListModel() { CacheData = cacheViewModel?.CacheList?.ToModel<CacheModel>()?.ToList()});
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
                if (ex.ErrorCode == ErrorCodes.InvalidData)
                    errorMessage = ex.Message;

                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion
        #endregion

        //Get the list of customer review states.
        public virtual List<SelectListItem> GetRoundOffList()
            => new List<SelectListItem>(){
                new SelectListItem(){Text="0", Value="0"},
                new SelectListItem(){Text="1", Value="1"},
                new SelectListItem(){Text="2", Value="2"},
                new SelectListItem(){Text="3", Value="3"},
                new SelectListItem(){Text="4", Value="4"},
                new SelectListItem(){Text="5", Value="5"},
                new SelectListItem(){Text="6", Value="6"}
            };

        //Get the list of customer review states.
        public virtual List<SelectListItem> GetEnvironmentsList()
            => new List<SelectListItem>(){
                new SelectListItem(){Text="DEV", Value="DEV"},
                new SelectListItem(){Text="QA", Value="QA"},
                new SelectListItem(){Text="UAT", Value="UAT"},
                new SelectListItem(){Text="PROD", Value="PROD"}
            };

           

        #region Configuration Settings

        // Get global configuration settings for application.
        public ConfigurationSettingViewModel GetConfigurationSettings()
            => _generalSettingClient.GetConfigurationSettings().ToViewModel<ConfigurationSettingViewModel>();

        // Update global configuration settings for application.
        public virtual bool UpdateConfigurationSettings(ConfigurationSettingViewModel model)
           => _generalSettingClient.UpdateConfigurationSettings(model.ToModel<ConfigurationSettingModel>());

        #endregion

        #region Power BI

        //Gets power bi setting details
        public virtual PowerBISettingsViewModel GetPowerBISettings()
            => _generalSettingClient.GetPowerBISettings()?.ToViewModel<PowerBISettingsViewModel>();

        //Updates power bi setting details
        public virtual bool UpdatePowerBISettings(PowerBISettingsViewModel powerBISettingsViewModel)
           => _generalSettingClient.UpdatePowerBISettings(powerBISettingsViewModel?.ToModel<PowerBISettingsModel>());

        #endregion

        #region Stock Notice Settings

        // Gets stock notice settings 
        public virtual StockNoticeSettingsViewModel GetStockNoticeSettings()
            => _generalSettingClient.GetStockNoticeSettings()?.ToViewModel<StockNoticeSettingsViewModel>();

        // Updates stock notice setting 
        public virtual bool UpdateStockNoticeSettings(StockNoticeSettingsViewModel stockNoticeSettingsViewModel)
           => _generalSettingClient.UpdateStockNoticeSettings(stockNoticeSettingsViewModel?.ToModel<StockNoticeSettingsModel>());

        #endregion


        #region Private methods

        private void SetIsEnabledFilter(FilterCollection filters, bool isEnabled)
        {
            if (HelperUtility.IsNotNull(filters))
            {
                filters.Add(new FilterTuple(FilterKeys.IsEnabled.ToString(), FilterOperators.Equals, (isEnabled ? 1 : 0).ToString()));
            }
        }

        #endregion
    }
}