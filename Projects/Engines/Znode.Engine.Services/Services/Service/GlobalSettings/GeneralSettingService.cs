using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Helper;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.Observer;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class GeneralSettingService : BaseService, IGeneralSettingService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeDisplayUnit> _displayUnitRepository;
        private readonly IZnodeRepository<ZnodeDateFormat> _dateFormatRepository;
        private readonly IZnodeRepository<ZnodeWeightUnit> _weightUnitRepository;
        private readonly IZnodeRepository<ZnodeTimeZone> _timeZoneRepository;
        private readonly IZnodeRepository<ZnodeGlobalSetting> _globalSettingRepository;
        private readonly IZnodeRepository<ZnodeApplicationCache> _applicationCacheRepository;
        private readonly IZnodeRepository<ZnodeTimeFormat> _timeFormatRepository;
        #endregion

        #region Public Constructor
        public GeneralSettingService()
        {
            _dateFormatRepository = new ZnodeRepository<ZnodeDateFormat>();
            _timeFormatRepository = new ZnodeRepository<ZnodeTimeFormat>();
            _displayUnitRepository = new ZnodeRepository<ZnodeDisplayUnit>();
            _weightUnitRepository = new ZnodeRepository<ZnodeWeightUnit>();
            _timeZoneRepository = new ZnodeRepository<ZnodeTimeZone>();
            _globalSettingRepository = new ZnodeRepository<ZnodeGlobalSetting>();
            _applicationCacheRepository = new ZnodeRepository<ZnodeApplicationCache>();
        }
        #endregion

        #region public Methods
        //Method To get List Of All General Setting
        public virtual GeneralSettingModel List()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            GeneralSettingModel list = new GeneralSettingModel();

            //Clear Cached Data
            CultureInfo.CurrentCulture.ClearCachedData();

            //Get list Of General Settings.
            List<ZnodeDateFormat> dateFormatsList = _dateFormatRepository.Table.Where(x => x.CultureName == CultureInfo.CurrentCulture.Name)?.ToList();
            List<ZnodeTimeFormat> timeFormatsList = _timeFormatRepository.Table.Where(x => x.CultureName == CultureInfo.CurrentCulture.Name)?.ToList();
            List<ZnodeDisplayUnit> DisplayUnitList = _displayUnitRepository.Table.ToList();
            List<ZnodeWeightUnit> weightUnitList = _weightUnitRepository.Table.ToList();
            List<ZnodeTimeZone> timeZoneList = _timeZoneRepository.Table.ToList();
            List<ZnodeGlobalSetting> priceGlobalSettingList = _globalSettingRepository.Table.Where(x => x.FeatureName == GlobalSettingEnum.PriceRoundOff.ToString()).ToList();
            List<ZnodeGlobalSetting> inventoryGlobalSettingList = _globalSettingRepository.Table.Where(x => x.FeatureName == GlobalSettingEnum.InventoryRoundOff.ToString()).ToList();
            List<ZnodeGlobalSetting> environmentSettingList = _globalSettingRepository.Table.Where(x => x.FeatureName == GlobalSettingEnum.CurrentEnvironment.ToString()).ToList();

            //Map of list from entity to model.
            list.DateFormatList = dateFormatsList.ToModel<DateFormatModel>().ToList();
            list.TimeFormatList = timeFormatsList.ToModel<TimeFormatModel>().ToList();
            list.DisplayUnitList = DisplayUnitList.ToModel<DisplayUnitModel>().ToList();
            list.WeightUnitList = weightUnitList.ToModel<WeightUnitModel>().ToList();
            list.TimeZoneList = timeZoneList.ToModel<TimeZoneModel>().ToList();
            list.PriceRoundOffList = priceGlobalSettingList.ToModel<DefaultGlobalConfigModel>().ToList();
            list.InventoryRoundOffList = inventoryGlobalSettingList.ToModel<DefaultGlobalConfigModel>().ToList();
            list.EnvironmentsList = environmentSettingList.ToModel<DefaultGlobalConfigModel>().ToList();
            list.ServerTimeZone = DefaultGlobalConfigSettingHelper.GetServerTimeZone();
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return list;
        }

        //Method To Update Existing General Setting
        public virtual bool Update(GeneralSettingModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            bool updated = UpdateDateFormat(model.DateFormatId) && UpdateTimeFormat(model.TimeFormatId) && UpdateTimeZone(model.TimeZoneId) && UpdateDisplayUnit(model.DisplayUnitId) && UpdatePriceRoundOff(model.PriceRoundOffFeatureValue) && UpdateInventoryRoundOff(model.InventoryRoundOffFeatureValue) && UpdateCurrentEnvironment(model.CurrentEnvironmentFeatureValue);
            if (updated)
            {
                IDefaultGlobalConfigService _service = ZnodeDependencyResolver.GetService<IDefaultGlobalConfigService>();
                DefaultGlobalConfigListModel globalSettingData = _service.GetDefaultGlobalConfigList();
                var clearCache = new ZnodeEventNotifier<DefaultGlobalConfigListModel>(globalSettingData);
                ZnodeCacheDependencyManager.Insert("DefaultGlobalConfigCache", globalSettingData, "ZnodeGlobalSetting");
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return updated;
        }


        #region Cache Management

        //gets the list of cache data
        public virtual CacheListModel GetCacheData()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);

            CacheListModel cacheListModel = new CacheListModel();
            List<ZnodeApplicationCache> applicationCacheList = _applicationCacheRepository.Table.ToList();
            ZnodeLogging.LogMessage("applicationCache list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, applicationCacheList?.Count());
            cacheListModel.CacheData = applicationCacheList.ToModel<CacheModel>().ToList();

            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);

            return cacheListModel;
        }

        //Updates provide Cache data if already exists, otherwise creates new entry.
        public virtual bool CreateUpdateCache(CacheListModel cacheListModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(cacheListModel?.CacheData?.Count <= 0))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            bool isSuccess = false;
            foreach (CacheModel cacheModel in cacheListModel?.CacheData)
            {
                //if data exists, then update the data.
                if (cacheModel.ApplicationCacheId > 0)
                {
                    isSuccess = _applicationCacheRepository.Update(cacheModel.ToEntity<ZnodeApplicationCache>());
                    ZnodeLogging.LogMessage(isSuccess ? Admin_Resources.SuccessCacheUpdate : Admin_Resources.ErrorCacheUpdate, string.Empty, TraceLevel.Info);
                }
                //else create a new data of provided cache.
                else
                {
                    isSuccess = _applicationCacheRepository.Insert(cacheModel.ToEntity<ZnodeApplicationCache>())?.ToModel<CacheModel>()?.ApplicationCacheId > 0;
                    ZnodeLogging.LogMessage(isSuccess ? Admin_Resources.SuccessCacheDataInsert : Admin_Resources.ErrorCacheDataInsert, string.Empty, TraceLevel.Info);
                }
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);

            return isSuccess;
        }

        //Refresh Cache data.
        public virtual CacheModel RefreshCacheData(CacheModel cacheModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            if ((cacheModel?.ApplicationCacheId).GetValueOrDefault() < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IDLengthCanNotLessOne);

            cacheModel.ApplicationType = _applicationCacheRepository.Table.FirstOrDefault(x => x.ApplicationCacheId == cacheModel.ApplicationCacheId)?.ApplicationType;

            //Clears and refreshes Cloudflare Cache
            if (cacheModel.ApplicationType == ApplicationCacheTypeEnum.CloudflareCache.ToString())
            {
                cacheModel.CloudflareErrorList = new CloudflareHelper().PurgeEverythingByDomainIds(cacheModel.DomainIds.Select(a => a.ToString()).Aggregate((i, j) => i + "," + j));
            }
            else
            {
                //Clears and refreshes api cache or full page cache.
                RefreshCache(cacheModel.ApplicationType, cacheModel.DomainIds.ToArray());
            }

            cacheModel.StartDate = DateTime.Now;
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return cacheModel;
        }
        #endregion

        #region Global configuration setting.

        // Get global configuration settings for application.
        public virtual ConfigurationSettingModel GetConfigurationSettings()
        {
            ConfigurationSettingModel configurationSettingModel = new ConfigurationSettingModel();
            ZnodeGlobalSetting allowedPromotions = null;
            ZnodeGlobalSetting isCalculateTaxAfterDiscount = null;
            ZnodeGlobalSetting isAllowWithOtherPromotionsAndCoupons = null;

            GetConfigurationDetails(out isAllowWithOtherPromotionsAndCoupons, out isCalculateTaxAfterDiscount, out allowedPromotions);

            if (HelperUtility.IsNotNull(isAllowWithOtherPromotionsAndCoupons))
            {
                configurationSettingModel.IsAllowWithOtherPromotionsAndCoupons = Convert.ToBoolean(isAllowWithOtherPromotionsAndCoupons.FeatureValues);
            }
            if (HelperUtility.IsNotNull(isCalculateTaxAfterDiscount))
            {
                configurationSettingModel.IsCalculateTaxAfterDiscount = Convert.ToBoolean(isCalculateTaxAfterDiscount.FeatureValues);
            }
            if (HelperUtility.IsNotNull(allowedPromotions))
            {
                configurationSettingModel.AllowedPromotions = allowedPromotions.FeatureValues;
            }

            return configurationSettingModel;
        }

        // Update global configuration settings for application.
        public virtual bool UpdateConfigurationSettings(ConfigurationSettingModel model)
        {
            ZnodeGlobalSetting allowedPromotions = null;
            ZnodeGlobalSetting isCalculateTaxAfterDiscount = null;
            ZnodeGlobalSetting isAllowWithOtherPromotionsAndCoupons = null;
            GetConfigurationDetails(out isAllowWithOtherPromotionsAndCoupons, out isCalculateTaxAfterDiscount, out allowedPromotions);

            List<ZnodeGlobalSetting> settingsToUpdate = new List<ZnodeGlobalSetting>();

            if (HelperUtility.IsNotNull(isCalculateTaxAfterDiscount))
            {
                isCalculateTaxAfterDiscount.FeatureValues = Convert.ToString(model.IsCalculateTaxAfterDiscount);
                isCalculateTaxAfterDiscount.ModifiedDate = model.ModifiedDate;
                settingsToUpdate.Add(isCalculateTaxAfterDiscount);
            }
            if (HelperUtility.IsNotNull(isAllowWithOtherPromotionsAndCoupons))
            {
                isAllowWithOtherPromotionsAndCoupons.FeatureValues = Convert.ToString(model.IsAllowWithOtherPromotionsAndCoupons);
                isAllowWithOtherPromotionsAndCoupons.ModifiedDate = model.ModifiedDate;
                settingsToUpdate.Add(isAllowWithOtherPromotionsAndCoupons);
            }
            if (HelperUtility.IsNotNull(allowedPromotions))
            {
                allowedPromotions.FeatureValues = model.AllowedPromotions;
                allowedPromotions.ModifiedDate = model.ModifiedDate;
                settingsToUpdate.Add(allowedPromotions);
            }

            bool isGlobalSettingUpdated = _globalSettingRepository.BatchUpdate(settingsToUpdate);

            if (isGlobalSettingUpdated)
            {
                ZnodeEventNotifier<ZnodePromotionCoupon> clearCacheInitializer = new ZnodeEventNotifier<ZnodePromotionCoupon>(new ZnodePromotionCoupon());
            }

            return isGlobalSettingUpdated;
        }

        #endregion

        #region PowerBi Details

        //Gets Power BI setting details
        public virtual PowerBISettingsModel GetPowerBISettings()
        {
            PowerBISettingsModel powerBISettingsModel = new PowerBISettingsModel
            {
                PowerBIApplicationId = DefaultGlobalConfigSettingHelper.PowerBIApplicationId,
                PowerBITenantId = DefaultGlobalConfigSettingHelper.PowerBITenantId,
                PowerBIReportId = DefaultGlobalConfigSettingHelper.PowerBIReportId,
                PowerBIGroupId = DefaultGlobalConfigSettingHelper.PowerBIGroupId,
                PowerBIUserName = DefaultGlobalConfigSettingHelper.PowerBIUserName,
                PowerBIPassword = DefaultGlobalConfigSettingHelper.PowerBIPassword
            };
            return powerBISettingsModel;
        }

        //Updates Power BI setting details

        public virtual bool UpdatePowerBISettings(PowerBISettingsModel powerBISettingsModel)
        {
            if (HelperUtility.IsNull(powerBISettingsModel))
                throw new ZnodeException(ErrorCodes.InvalidData, "Model cannot be null.");
            try
            {
                string[] features = new[] { GlobalSettingEnum.PowerBIApplicationId.ToString(), GlobalSettingEnum.PowerBITenantId.ToString(), GlobalSettingEnum.PowerBIGroupId.ToString(), GlobalSettingEnum.PowerBIReportId.ToString(), GlobalSettingEnum.PowerBIUserName.ToString(), GlobalSettingEnum.PowerBIPassword.ToString() };
                List<ZnodeGlobalSetting> lstZnodeGlobalSetting = _globalSettingRepository.Table.Where(o => features.Contains(o.FeatureName.Trim()))?.ToList();
                if (lstZnodeGlobalSetting.Count > 0)
                {
                    lstZnodeGlobalSetting.ForEach(feature =>
                    {
                        switch (feature.FeatureName.Trim())
                        {
                            case ZnodeConstant.PowerBIApplicationId:
                                {
                                    UpdatePowerBIGlobalSettings(powerBISettingsModel.PowerBIApplicationId, feature);
                                    break;
                                }
                            case ZnodeConstant.PowerBITenantId:
                                {
                                    UpdatePowerBIGlobalSettings(powerBISettingsModel.PowerBITenantId, feature);
                                    break;
                                }
                            case ZnodeConstant.PowerBIReportId:
                                {
                                    UpdatePowerBIGlobalSettings(powerBISettingsModel.PowerBIReportId, feature);
                                    break;
                                }
                            case ZnodeConstant.PowerBIGroupId:
                                {
                                    UpdatePowerBIGlobalSettings(powerBISettingsModel.PowerBIGroupId, feature);
                                    break;
                                }
                            case ZnodeConstant.PowerBIUserName:
                                {
                                    UpdatePowerBIGlobalSettings(powerBISettingsModel.PowerBIUserName, feature);
                                    break;
                                }
                            case ZnodeConstant.PowerBIPassword:
                                {
                                    UpdatePowerBIGlobalSettings(powerBISettingsModel.PowerBIPassword, feature);
                                    break;
                                }
                        }
                    });
                }
                return true;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage($"An exception occurred while updating the powerbi detail settings. ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error, ex);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #endregion

        #region Stock Notification Settings
        // Gets stock notice setting 
        public virtual StockNoticeSettingsModel GetStockNoticeSettings()
        {
            StockNoticeSettingsModel stockNoticeSettingsModel = new StockNoticeSettingsModel
            {
                DeleteAlreadySentEmails = DefaultGlobalConfigSettingHelper.DeleteAlreadySentEmails,
                DeletePendingEmails = DefaultGlobalConfigSettingHelper.DeletePendingEmails
            };
            return stockNoticeSettingsModel;
        }

        // Updates stock notice settings
        public virtual bool UpdateStockNoticeSettings(StockNoticeSettingsModel stockNoticeSettingsModel)
        {
            if (HelperUtility.IsNull(stockNoticeSettingsModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelNull);
            try
            {
                string[] features = new[] { GlobalSettingEnum.DeleteAlreadySentEmails.ToString(), GlobalSettingEnum.DeletePendingEmails.ToString() };
                List<ZnodeGlobalSetting> znodeGlobalSettingList = _globalSettingRepository.Table.Where(o => features.Contains(o.FeatureName))?.ToList();
                bool isSettingUpdated = false;
                if (znodeGlobalSettingList?.Count > 0)
                {
                    znodeGlobalSettingList?.ForEach(feature =>
                    {
                        switch (feature?.FeatureName)
                        {
                            case ZnodeConstant.DeleteAlreadySentEmails:
                                {
                                    isSettingUpdated = UpdateStockNoticeGlobalSettings(stockNoticeSettingsModel.DeleteAlreadySentEmails, feature);
                                    break;
                                }
                            case ZnodeConstant.DeletePendingEmails:
                                {
                                    isSettingUpdated = UpdateStockNoticeGlobalSettings(stockNoticeSettingsModel.DeletePendingEmails, feature);
                                    break;
                                }
                        }
                    });
                }
                return isSettingUpdated;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage($"An exception occurred while updating the stock notification detail settings with exceptions {ex.ErrorMessage}", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #endregion

        #endregion

        #region protected Methods
        protected virtual void ClearGlobalSettingsCache()
        {
            ClearCacheHelper.EnqueueEviction(new GlobalSettingCacheEvent()
            {
                Comment = $"From User updating timezone settings.",
            });
        }

        #endregion

        #region private Methods
        //Update DateFormats
        private bool UpdateDateFormat(int dateFormatId)
        {
            if (dateFormatId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.DateFormatIdNotLessThanOne);
            try
            {
                FilterTuple filter = new FilterTuple(ZnodeDateFormatEnum.DateFormatId.ToString(), ProcedureFilterOperators.Equals, dateFormatId.ToString());
                FilterCollection filtersList = new FilterCollection();
                filtersList.Add(filter);

                //gets the where clause.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());

                ZnodeDateFormat dateFormatList = _dateFormatRepository.GetEntity(whereClauseModel.WhereClause);

                if (!dateFormatList.IsDefault)
                {   //Get List OF Entity From Table 
                    List<ZnodeDateFormat> defaultConfigurationList = _dateFormatRepository.Table.Where(x => x.IsDefault).ToList();

                    defaultConfigurationList.ForEach(x => x.IsDefault = false);
                    //Set IsDefault equal to true for the entity to update
                    dateFormatList.IsDefault = true;
                    defaultConfigurationList.Add(dateFormatList);

                    //Update List Of Entity To Database
                    defaultConfigurationList.ForEach(x => _dateFormatRepository.Update(x));
                    return true;
                }
                else
                    return true;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorUpdatingDateFormat, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error, ex);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Update Time Format
        private bool UpdateTimeFormat(int timeFormatId)
        {
            if (timeFormatId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.TimeFormatIdNotLessThanOne);
            try
            {
                FilterTuple filter = new FilterTuple(ZnodeTimeFormatEnum.TimeFormatId.ToString(), ProcedureFilterOperators.Equals, timeFormatId.ToString());
                FilterCollection filtersList = new FilterCollection();
                filtersList.Add(filter);

                //gets the where clause.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());

                ZnodeTimeFormat timeFormatList = _timeFormatRepository.GetEntity(whereClauseModel.WhereClause);

                if (!timeFormatList.IsDefault)
                {   //Get List OF Entity From Table 
                    List<ZnodeTimeFormat> defaultConfigurationList = _timeFormatRepository.Table.Where(x => x.IsDefault).ToList();

                    defaultConfigurationList.ForEach(x => x.IsDefault = false);
                    //Set IsDefault equal to true for the entity to update
                    timeFormatList.IsDefault = true;
                    defaultConfigurationList.Add(timeFormatList);

                    //Update List Of Entity To Database
                    defaultConfigurationList.ForEach(x => _timeFormatRepository.Update(x));
                    return true;
                }
                else
                    return true;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorUpdatingTimeFormat, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error, ex);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                return false;
            }
        }


        //Update timezones
        private bool UpdateTimeZone(int timeZoneId)
        {
            if (timeZoneId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, "timeZoneId cannot be less than 1");
            try
            {
                FilterTuple filter = new FilterTuple(ZnodeTimeZoneEnum.TimeZoneId.ToString(), ProcedureFilterOperators.Equals, timeZoneId.ToString());
                FilterCollection filtersList = new FilterCollection();
                filtersList.Add(filter);

                //gets the where clause.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());

                ZnodeTimeZone timeZoneList = _timeZoneRepository.GetEntity(whereClauseModel.WhereClause);

                if (!timeZoneList.IsDefault)
                {   //Get List OF Entity From Table 
                    List<ZnodeTimeZone> defaultConfigurationList = _timeZoneRepository.Table.Where(x => x.IsDefault).ToList();
                    defaultConfigurationList.ForEach(x => x.IsDefault = false);
                    //Set IsDefault equal to true for the entity to update
                    timeZoneList.IsDefault = true;
                    defaultConfigurationList.Add(timeZoneList);

                    //Update List Of Entity To Database
                    defaultConfigurationList.ForEach(x => _timeZoneRepository.Update(x));
                    ClearGlobalSettingsCache();

                    return true;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"An exception occurred while updating timezone. Exception message-{ex.Message}", ZnodeLogging.Components.GlobalSettings.ToString());
                return false;
            }
        }

        //Update Display Unit
        private bool UpdateDisplayUnit(int displayUnitId)
        {
            if (Equals(displayUnitId, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.DisplayUnitIdNotLessThanOne);
            try
            {
                FilterTuple filter = new FilterTuple(ZnodeDisplayUnitEnum.DisplayUnitId.ToString(), ProcedureFilterOperators.Equals, displayUnitId.ToString());
                FilterCollection filterList = new FilterCollection();
                filterList.Add(filter);

                //gets the where clause.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());

                ZnodeDisplayUnit displayUnitList = _displayUnitRepository.GetEntity(whereClauseModel.WhereClause);
                if (!displayUnitList.IsDefault)
                {   //Get List OF Entity From Table 
                    List<ZnodeDisplayUnit> defaultConfigurationList = _displayUnitRepository.Table.Where(x => x.IsDefault).ToList();
                    defaultConfigurationList.ForEach(x => x.IsDefault = false);
                    //Set IsDefault equal to true for the entity to update
                    displayUnitList.IsDefault = true;
                    defaultConfigurationList.Add(displayUnitList);

                    //Update List Of Entity To Database
                    defaultConfigurationList.ForEach(x => _displayUnitRepository.Update(x));
                    return true;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorUpdatingDisplayUnit, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }

        //Update Price RoundOff.
        private bool UpdatePriceRoundOff(int priceFeatureValue)
        {
            if (priceFeatureValue < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PriceRoundOffNotLessThanZero);
            try
            {
                FilterTuple filter = new FilterTuple(ZnodeGlobalSettingEnum.FeatureName.ToString(), ProcedureFilterOperators.Is, GlobalSettingEnum.PriceRoundOff.ToString());
                FilterCollection filtersList = new FilterCollection();
                filtersList.Add(filter);

                //gets the where clause.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());

                ZnodeGlobalSetting globalSetting = _globalSettingRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);

                if (!Equals(globalSetting?.FeatureValues, priceFeatureValue.ToString()))
                {
                    globalSetting.FeatureValues = priceFeatureValue.ToString();
                    return _globalSettingRepository.Update(globalSetting);
                }
                return true;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorUpdatingPriceRoundOff, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error, ex);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Update Inventory RoundOff.
        private bool UpdateInventoryRoundOff(int inventoryFeatureValue)
        {
            if (inventoryFeatureValue < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InventoryRoundOffLessThanZero);
            try
            {
                FilterTuple filter = new FilterTuple(ZnodeGlobalSettingEnum.FeatureName.ToString(), ProcedureFilterOperators.Is, GlobalSettingEnum.InventoryRoundOff.ToString());
                FilterCollection filtersList = new FilterCollection();
                filtersList.Add(filter);

                //gets the where clause.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());

                ZnodeGlobalSetting globalSetting = _globalSettingRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);

                if (!Equals(globalSetting?.FeatureValues, inventoryFeatureValue.ToString()))
                {
                    globalSetting.FeatureValues = inventoryFeatureValue.ToString();
                    return _globalSettingRepository.Update(globalSetting);
                }
                return true;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorInventoryRoundOffUpdate, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error, ex);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Update Current Environment.
        private bool UpdateCurrentEnvironment(string currentEnvironment)
        {
            try
            {
                FilterTuple filter = new FilterTuple(ZnodeGlobalSettingEnum.FeatureName.ToString(), ProcedureFilterOperators.Is, GlobalSettingEnum.CurrentEnvironment.ToString());
                FilterCollection filtersList = new FilterCollection();
                filtersList.Add(filter);

                //gets the where clause.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());

                ZnodeGlobalSetting globalSetting = _globalSettingRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);

                if (!Equals(globalSetting?.FeatureValues, currentEnvironment))
                {
                    globalSetting.FeatureValues = currentEnvironment;
                    return _globalSettingRepository.Update(globalSetting);
                }
                return true;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorInventoryRoundOffUpdate, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error, ex);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Refreshes Application Pool or Api Cache
        private static void RefreshCache(string applicationType, int[] domainIds)
        {
            if (applicationType == ApplicationCacheTypeEnum.ApiCache.ToString())
            {
                ClearCacheHelper.EnqueueEviction(new ManuallyRefreshApiCacheEvent()
                {
                    Comment = $"From user triggering API Cache Refresh from Admin UI."
                });
            }

            if (applicationType == ApplicationCacheTypeEnum.FullPageCache.ToString())
            {
                ClearCacheHelper.EnqueueEviction(new ManuallyRefreshWebStoreCacheEvent()
                {
                    Comment = $"From user triggering WebStore Cache Refresh from Admin UI.",
                    DomainIds = domainIds
                });
            }
        }

        //Set global configuration settings for application.
        private void GetConfigurationDetails(out ZnodeGlobalSetting isAllowWithOtherPromotionsAndCoupons, out ZnodeGlobalSetting isCalculateTaxAfterDiscount, out ZnodeGlobalSetting allowedPromotions)
        {
            string[] features = new[] { GlobalSettingEnum.IsAllowWithOtherPromotionsAndCoupons.ToString(), GlobalSettingEnum.AllowedPromotions.ToString(), GlobalSettingEnum.IsCalculateTaxAfterDiscount.ToString() };
            List<ZnodeGlobalSetting> lstZnodeGlobalSetting = _globalSettingRepository.Table.Where(o => features.Contains(o.FeatureName.Trim()))?.ToList();

            isAllowWithOtherPromotionsAndCoupons = lstZnodeGlobalSetting?.FirstOrDefault(x => x.FeatureName == GlobalSettingEnum.IsAllowWithOtherPromotionsAndCoupons.ToString());
            isCalculateTaxAfterDiscount = lstZnodeGlobalSetting?.FirstOrDefault(x => x.FeatureName == GlobalSettingEnum.IsCalculateTaxAfterDiscount.ToString());
            allowedPromotions = lstZnodeGlobalSetting?.FirstOrDefault(x => x.FeatureName == GlobalSettingEnum.AllowedPromotions.ToString());
        }

        //Update PowerBI Settings
        private void UpdatePowerBIGlobalSettings(string powerBISettingsId, ZnodeGlobalSetting feature)
        {
            if (feature.FeatureValues != powerBISettingsId)
            {
                feature.FeatureValues = powerBISettingsId;
                _globalSettingRepository.Update(feature);
            }
        }

        // Update Stock Notice Settings.
        private bool UpdateStockNoticeGlobalSettings(string stockNoticeSettingsFeature, ZnodeGlobalSetting feature)
        {
            if (feature?.FeatureValues != stockNoticeSettingsFeature)
            {
                feature.FeatureValues = stockNoticeSettingsFeature;
                return _globalSettingRepository.Update(feature);
            }
            return true;
        }
        #endregion
    }
}
