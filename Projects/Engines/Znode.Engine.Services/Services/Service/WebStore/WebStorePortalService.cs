using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public partial class PortalService
    {
        #region Public Methods

        //Get Portal information by Portal Id.
        public virtual WebStorePortalModel WebStoreGetPortal(int portalId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter portalId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, portalId);

            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PortalIdNotLessThanOne);

             WebStorePortalModel webStorePortalModel = GetService<IPublishedPortalDataService>().GetWebstorePortalDetails(portalId)?.ToModel<WebStorePortalModel>();

            if (IsNotNull(webStorePortalModel))
            {
                webStorePortalModel.PortalFeatureValues = ZnodeConfigManager.GetSiteConfigFeatureValueList(portalId);
                webStorePortalModel.CustomerServiceEmail = ZnodeConfigManager.SiteConfig.CustomerServiceEmail;
                webStorePortalModel.CustomerServicePhoneNumber = ZnodeConfigManager.SiteConfig.CustomerServicePhoneNumber;
                GetPortalLocales(webStorePortalModel);
                GetPortalParentTheme(webStorePortalModel);
                SetDefaultServer(webStorePortalModel);               
                GetPortalProfileID(webStorePortalModel, portalId);
                SetPortalProfileCatalogId(webStorePortalModel);
                SetPortalCatalogId(webStorePortalModel, portalId);
                GetPortalSortSetting(webStorePortalModel, portalId);       
                GetPortalPageSetting(webStorePortalModel, portalId);
                GetPortalCurrency(webStorePortalModel);
                GetPortalCompareSetting(webStorePortalModel);
                GetPortalAddressValidationSetting(webStorePortalModel);
                SetPortalInventorySetting(portalId, webStorePortalModel);
                GetTagManagerSetting(portalId, webStorePortalModel);
                GetPortalTrackingPixelForPortal(portalId, webStorePortalModel);
                GetRobotText(webStorePortalModel, portalId);
                BindFullPageCacheDetails(webStorePortalModel);
                webStorePortalModel.WebstoreVersionId = WebstoreVersionId.GetValueOrDefault();
                GetApprovalManagement(webStorePortalModel, portalId);
                GetPortalKlaviyoSetting(portalId, webStorePortalModel);
            }
            ZnodeLogging.LogMessage("PortalId, PublishCatalogId, CMSThemeId, PortalThemeId and ProfileId properties of WebStorePortalModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { PortalId = webStorePortalModel?.PortalId, PublishCatalogId = webStorePortalModel?.PublishCatalogId, CMSThemeId = webStorePortalModel?.CMSThemeId, PortalThemeId = webStorePortalModel?.PortalThemeId, ProfileId = webStorePortalModel?.ProfileId });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return webStorePortalModel;
        }

        public virtual WebStorePortalModel WebStoreGetPortal(int portalId, int localeId, ApplicationTypesEnum applicationType, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters portalId, localeId and applicationType: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { portalId, localeId, applicationType });

            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PortalIdNotLessThanOne);

            if (localeId < 1)
                localeId = GetPortalDefaultLocaleId(portalId);

            ZnodePublishStatesEnum contentState = GetPublishStateFromApplicationType(applicationType);
            ZnodeLogging.LogMessage("contentState returned from GetPublishStateFromApplicationType method: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, contentState);

            WebStorePortalModel webStorePortalModel = GetService<IPublishedPortalDataService>().GetWebstoreEntity(portalId, localeId, contentState.ToString()).ToModel<WebStorePortalModel>();

            if (IsNotNull(webStorePortalModel))
            {
                webStorePortalModel.PortalFeatureValues = ZnodeConfigManager.GetSiteConfigFeatureValueList(portalId);
                webStorePortalModel.CustomerServiceEmail = ZnodeConfigManager.SiteConfig.CustomerServiceEmail;
                webStorePortalModel.SalesPhoneNumber = ZnodeConfigManager.SiteConfig.SalesPhoneNumber;
                webStorePortalModel.StoreCode = ZnodeConfigManager.SiteConfig.StoreCode;
                webStorePortalModel.CustomerServicePhoneNumber = ZnodeConfigManager.SiteConfig.CustomerServicePhoneNumber;
                webStorePortalModel.SalesEmail = ZnodeConfigManager.SiteConfig.SalesEmail;
                webStorePortalModel.AdministratorEmail = ZnodeConfigManager.SiteConfig.AdminEmail;
                GetPortalLocales(webStorePortalModel);
                GetPortalParentTheme(webStorePortalModel);
                SetDefaultServer(webStorePortalModel);
                GetPortalProfileID(webStorePortalModel, portalId);
                SetPortalProfileCatalogId(webStorePortalModel);
                SetPortalCatalogId(webStorePortalModel, portalId);  
                GetPortalSortSetting(webStorePortalModel, portalId);
                GetPortalPageSetting(webStorePortalModel, portalId);
                GetPortalCurrency(webStorePortalModel);
                GetPortalCompareSetting(webStorePortalModel);
                GetPortalAddressValidationSetting(webStorePortalModel);
                SetPortalInventorySetting(portalId, webStorePortalModel);
                GetTagManagerSetting(portalId, webStorePortalModel);
                GetPortalTrackingPixelForPortal(portalId, webStorePortalModel);
                GetRobotText(webStorePortalModel, portalId);
                BindFullPageCacheDetails(webStorePortalModel);
                GetStoreGlobalAttributeData(webStorePortalModel);
                webStorePortalModel.DynamicStyle = GetDynamicStyle(localeId, portalId);
                webStorePortalModel.UserVerificationTypeCode = (UserVerificationTypeEnum)Enum.Parse(typeof(UserVerificationTypeEnum), ZnodeConfigManager.SiteConfig.UserVerificationType);
                GetApprovalManagement(webStorePortalModel, portalId);
                webStorePortalModel.RecommendationSetting = GetRecommendationSetting(portalId);
                GetPortalSaveForLaterSetting(webStorePortalModel);
                GetPortalProductInheritance(webStorePortalModel);
                GetPortalAddToCartOptionForProductSliders(webStorePortalModel);
                GetPortalKlaviyoSetting(portalId, webStorePortalModel);
                webStorePortalModel.IsSmsProviderEnabled = IsSmsProviderEnabled(portalId);
            }
            ZnodeLogging.LogMessage("PortalId, PublishCatalogId, CMSThemeId, PortalThemeId and ProfileId properties of WebStorePortalModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { PortalId = webStorePortalModel?.PortalId, PublishCatalogId = webStorePortalModel?.PublishCatalogId, CMSThemeId = webStorePortalModel?.CMSThemeId, PortalThemeId = webStorePortalModel?.PortalThemeId, ProfileId = webStorePortalModel?.ProfileId });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return webStorePortalModel;
        }

        public virtual WebStorePortalModel WebStoreGetPortal(string domainName, NameValueCollection expands)
        {
            int? portalId = _domainRepository.Table.FirstOrDefault(x => x.DomainName == domainName)?.PortalId;
            if (Equals(portalId, null)) return null;
            return WebStoreGetPortal(Convert.ToInt32(portalId), expands);
        }

        // This method is returning default localeid from the portalLocale if not found then returning the first active localeid from global locale
        protected virtual int GetPortalDefaultLocaleId(int portalId)
        {
            ZnodePortalLocale defaultLocale = _portalLocaleRepository.Table.FirstOrDefault(x => x.PortalId == portalId && x.IsDefault == true);
            if (defaultLocale == null || defaultLocale.LocaleId < 1)
            {
                ZnodeLogging.LogMessage("No default locale set for webstore having portalId: "+ portalId, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                return GetActiveLocaleList().FirstOrDefault(x => x.IsDefault).LocaleId;
            }
            return defaultLocale.LocaleId;
        }
        #endregion

        #region Private Methods

        //Get dynamic styles .
        protected virtual string GetDynamicStyle(int localeId = 0, int portalId = 0)
        {
            localeId = localeId > 0 ? localeId : GetDefaultLocaleId();
            List<ZnodePublishPortalCustomCssEntity> dynamicStyleEntities = GetService<IPublishedPortalDataService>().GetDynamicStyleList(localeId, portalId);

            return dynamicStyleEntities?.Count() > 0 ? dynamicStyleEntities.FirstOrDefault().DynamicStyle : "";
        }

        //To get the product recommendation settings from database.
        protected virtual RecommendationSettingModel GetRecommendationSetting(int portalId)
        {
            return _recommendationRepository.Table.FirstOrDefault(x=>x.PortalId == portalId).ToModel<RecommendationSettingModel>();
        }

        //List of portals.
        private void GetPortalLocales(WebStorePortalModel model)
        {
            //Get Portal Locale data.
            model.PortalLocales = (from portalLocale in _portalLocaleRepository.Table
                                   join locale in _locales.Table on portalLocale.LocaleId equals locale.LocaleId
                                   where portalLocale.PortalId == model.PortalId && locale.IsActive
                                   select new LocaleModel
                                   {
                                       Name = locale.Name,
                                       LocaleId = portalLocale.LocaleId,
                                       IsDefault = portalLocale.IsDefault,
                                       Code = locale.Code,
                                       IsActive = locale.IsActive
                                   }).ToList();
            ZnodeLogging.LogMessage("PortalLocales list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { PortalLocalesListCount = model?.PortalLocales?.Count });
        }

        private void GetRobotText(WebStorePortalModel model, int portalId)
        {
            string DefaultRobotTag = _robotsTxtRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.DefaultRobotTag;
            model.DefaultRobotTag= !string.IsNullOrEmpty(DefaultRobotTag) && DefaultRobotTag.ToLower() != "none" ? DefaultRobotTag.Replace("_", ",") : string.Empty;

        }

        //Get Parent theme.
        private void GetPortalParentTheme(WebStorePortalModel model)
        {
            if (model.CMSThemeId.HasValue)
            {
                //Get Portal parent theme data.
                FilterCollection filter = new FilterCollection() { new FilterTuple(ZnodeCMSThemeEnum.CMSThemeId.ToString(), FilterOperators.Equals, model.CMSThemeId.Value.ToString()) };
                ZnodeCMSTheme theme = _cmsTheme.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause, GetExpandsForParentTheme());

                if (HelperUtility.IsNotNull(theme?.ZnodeCMSTheme2))
                {
                    model.CMSParentThemeId = theme.ParentThemeId;
                    model.ParentThemeName = theme.ZnodeCMSTheme2.Name;
                }
            }
            ZnodeLogging.LogMessage("CMSParentThemeId and ParentThemeName properties of WebStorePortalModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { CMSParentThemeId = model?.CMSParentThemeId, ParentThemeName = model?.ParentThemeName });
        }

        //Set the default Media Server URL.
        private void SetDefaultServer(WebStorePortalModel model)
        {
            IMediaConfigurationService mediaConfiguration = GetService<IMediaConfigurationService>();
            MediaConfigurationModel configuration = mediaConfiguration.GetDefaultMediaConfiguration();
            string serverPath = GetMediaServerUrl(configuration);
            if (HelperUtility.IsNotNull(configuration))
                model.MediaServerUrl = serverPath;
            model.MediaServerThumbnailUrl = $"{serverPath}{configuration.ThumbnailFolderName}";
            SetImageFolderPath(model, serverPath);
        }

        //Set the image folder path for all sizes
        private void SetImageFolderPath(WebStorePortalModel model, string serverPath)
        {
            GlobalMediaDisplaySettingModel displaySetting = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration()?.GlobalMediaDisplaySetting;

            if(IsNotNull(displaySetting))
            {
                model.ImageLargeUrl = $"{serverPath}{"Catalog/"}{displaySetting.MaxLargeWidth}/";

                model.ImageMediumUrl = $"{serverPath}{"Catalog/"}{displaySetting.MaxMediumWidth}/";

                model.ImageSmallUrl = $"{serverPath}{"Catalog/"}{displaySetting.MaxSmallWidth}/";

                model.ImageCrossSellUrl = $"{serverPath}{"Catalog/"}{displaySetting.MaxCrossSellWidth}/";

                model.ImageThumbnailUrl = $"{serverPath}{"Catalog/"}{displaySetting.MaxThumbnailWidth}/";

                model.ImageSmallThumbnailUrl = $"{serverPath}{"Catalog/"}{displaySetting.MaxSmallThumbnailWidth}/";
            }
            else
            {
                ZnodeLogging.LogMessage("There is no global media display setting available.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            }

        }

        //Generate filters for Portal Id.
        private static FilterCollection PortalFilter(int portalId)
            => new FilterCollection() { new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()) };

        private static FilterCollection PortalContentStateFilter(int portalId, int localeId, ZnodePublishStatesEnum publishState)
            => new FilterCollection() { new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()), new FilterTuple(ZnodeConstant.LocaleId, FilterOperators.Equals, localeId.ToString()), new FilterTuple(ZnodeConstant.PublishState, FilterOperators.Equals, publishState.ToString()) };

        //Set catalog id for portal.
        private void SetPortalCatalogId(WebStorePortalModel webStorePortalModel, int portalId)
        {
            webStorePortalModel.PublishCatalogId = _portalCatalogRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.PublishCatalogId;
        }

        //Set catalog id associated to portal profile.
        protected virtual void SetPortalProfileCatalogId(WebStorePortalModel webStorePortalModel)
        {
            int profileCatalogId = GetPublishedProfileCatalogId(webStorePortalModel);
            webStorePortalModel.PortalProfileCatalogId = profileCatalogId;
        }


        //Get profile id for portal.
        private void GetPortalProfileID(WebStorePortalModel webStorePortalModel, int portalId)
            => webStorePortalModel.ProfileId = (_portalProfileRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(new FilterCollection() { new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()),
                new FilterTuple(ZnodePortalProfileEnum.IsDefaultAnonymousProfile.ToString(), FilterOperators.Equals, "true") }.ToFilterDataCollection()).WhereClause)?.ProfileId).GetValueOrDefault();

        //Get publish profile catalog id for portal.
        protected virtual int GetPublishedProfileCatalogId(WebStorePortalModel webStorePortalModel)
        {
                int publishProfileCatalogId = 0;
                if (webStorePortalModel.ProfileId > 0 && webStorePortalModel.PortalFeatureValues?.Count > 0 && webStorePortalModel.PortalFeatureValues.ContainsKey(StoreFeature.Enable_Profile_Based_Search.ToString()) && webStorePortalModel.PortalFeatureValues[StoreFeature.Enable_Profile_Based_Search.ToString()])
                {
                    IZnodeRepository<ZnodeProfile> _profile = new ZnodeRepository<ZnodeProfile>();
                    publishProfileCatalogId = (from profile in _profile.Table
                                               where profile.ProfileId == webStorePortalModel.ProfileId
                                               select profile.PimCatalogId)?.FirstOrDefault() ?? 0; 
                }
                return publishProfileCatalogId;
        }

        //Set Portal currency details.
        private void GetPortalCurrency(WebStorePortalModel model)
        {
            //Get Portal currency data.
            CurrencyModel currencyModel = (from asl in _portalUnitRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(PortalFilter(model.PortalId).ToFilterDataCollection()).WhereClause)
                                           join portalCurrency in _currencyRepository.GetEntityList(string.Empty) on asl.CurrencyId equals portalCurrency.CurrencyId
                                           join portalCulture in _cultureRepository.GetEntityList(string.Empty) on asl.CultureId equals portalCulture.CultureId
                                           select new CurrencyModel
                                           {
                                               CultureCode = portalCulture.CultureCode,
                                               CurrencyCode = portalCurrency.CurrencyCode,
                                               Symbol = portalCulture.Symbol
                                           }
                                   )?.FirstOrDefault();
            //Set Portal currency details.
            if (IsNotNull(currencyModel))
            {
                model.CurrencySymbol = currencyModel.Symbol;
                model.CurrencyCode = currencyModel.CurrencyCode;
                model.CultureCode = currencyModel.CultureCode;
            }
            ZnodeLogging.LogMessage("CurrencyCode and CultureCode properties of WebStorePortalModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { CurrencyCode = model?.CurrencyCode, CultureCode = model?.CultureCode });
        }

        //Get product compare setting.
        private void GetPortalCompareSetting(WebStorePortalModel webStorePortalModel)
        {
            if (webStorePortalModel.PortalFeatureValues?.Count > 0 && webStorePortalModel.PortalFeatureValues.ContainsKey(StoreFeature.Enable_Product_Compare.ToString()) && webStorePortalModel.PortalFeatureValues[StoreFeature.Enable_Product_Compare.ToString()])
                webStorePortalModel.EnableCompare = true;
        }

        //Get address validation setting.
        private void GetPortalAddressValidationSetting(WebStorePortalModel webStorePortalModel)
        {
            if (webStorePortalModel.PortalFeatureValues?.Count > 0 && webStorePortalModel.PortalFeatureValues.ContainsKey(StoreFeature.Address_Validation.ToString()) && webStorePortalModel.PortalFeatureValues[StoreFeature.Address_Validation.ToString()])
                webStorePortalModel.EnableAddressValidation = true;
        }

        //Get SaveForLater setting.
        private void GetPortalSaveForLaterSetting(WebStorePortalModel webStorePortalModel)
        {
            if (webStorePortalModel?.PortalFeatureValues?.Count > 0 && webStorePortalModel.PortalFeatureValues.ContainsKey(StoreFeature.Enable_Save_For_Later.ToString()) && webStorePortalModel.PortalFeatureValues[StoreFeature.Enable_Save_For_Later.ToString()])
                webStorePortalModel.EnableSaveForLater = true;
        }

        //Get ProductInheritance setting.
        private void GetPortalProductInheritance(WebStorePortalModel webStorePortalModel)
        {
            if (webStorePortalModel?.PortalFeatureValues?.Count > 0 && webStorePortalModel.PortalFeatureValues.ContainsKey(StoreFeature.Enable_Product_Inheritance.ToString()) && webStorePortalModel.PortalFeatureValues[StoreFeature.Enable_Product_Inheritance.ToString()])
                webStorePortalModel.IsProductInheritanceEnabled = true;
        }

        //Get Add to cart option for product sliders setting.
        public void GetPortalAddToCartOptionForProductSliders(WebStorePortalModel webStorePortalModel)
        {
            if (webStorePortalModel?.PortalFeatureValues?.Count > 0 && webStorePortalModel.PortalFeatureValues.ContainsKey(StoreFeature.Enable_Add_To_Cart_Option_For_Product_Sliders.ToString()) && webStorePortalModel.PortalFeatureValues[StoreFeature.Enable_Add_To_Cart_Option_For_Product_Sliders.ToString()])
                webStorePortalModel.IsAddToCartOptionForProductSlidersEnabled = true;
        }

        //set stored based In Stock, Out Of Stock, Back Order Message.
        private void SetPortalInventorySetting(int portalId, WebStorePortalModel webStorePortalModel)
        {
            ZnodePortal portalDetails = _portalRepository.Table.Where(x => x.PortalId == portalId)?.FirstOrDefault();
            if(IsNotNull(portalDetails))
            {
                webStorePortalModel.InStockMessage = portalDetails.InStockMsg;
                webStorePortalModel.OutOfStockMessage = portalDetails.OutOfStockMsg;
                webStorePortalModel.BackOrderMessage = portalDetails.BackOrderMsg;
                webStorePortalModel.StoreName = portalDetails.StoreName;
                webStorePortalModel.DefaultOrderStateID = Convert.ToInt32(portalDetails.DefaultOrderStateID);

                ZnodeLogging.LogMessage("StoreName property of WebStorePortalModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { StoreName = webStorePortalModel?.StoreName });
            }
        }

        //Get google tag manager setting.
        private void GetTagManagerSetting(int portalId, WebStorePortalModel webStorePortalModel)
        {
            ZnodeGoogleTagManager tagManager = _tagManager.Table.Where(x => x.PortalId == portalId)?.FirstOrDefault();
            if (IsNotNull(tagManager))
            {
                webStorePortalModel.IsEnabledTagManager = tagManager.IsActive;
                webStorePortalModel.ContainerId = tagManager.ContainerId;
                webStorePortalModel.AnalyticsIdForAddToCart = tagManager.AnalyticsIdForAddToCart;
                webStorePortalModel.AnalyticsIdForRemoveFromCart = tagManager.AnalyticsIdForRemoveFromCart;
                webStorePortalModel.AnalyticsUId = tagManager.AnalyticsUId;
                webStorePortalModel.AnalyticsIsActive = tagManager.AnalyticsIsActive.GetValueOrDefault();
                webStorePortalModel.EnableEnhancedEcommerce = tagManager.EnableEnhancedEcommerce;
                ZnodeLogging.LogMessage("ContainerId and AnalyticsUId properties of WebStorePortalModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { ContainerId = webStorePortalModel?.ContainerId, AnalyticsUId = webStorePortalModel?.AnalyticsUId });
            }
        }

        //Check Key Already exist in Dictionary.
        private bool CheckKeyAlreadyExist(string key, string value, Dictionary<string, string> pixelSettings)
       => (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && !pixelSettings.ContainsKey(key));

        //Bind full page application cache details.
        private void BindFullPageCacheDetails(WebStorePortalModel webStorePortalModel)
        {
            IZnodeRepository<ZnodeApplicationCache> _applicationCacheRepository = new ZnodeRepository<ZnodeApplicationCache>();
            CacheModel fullPageCacheDetails = _applicationCacheRepository.Table.FirstOrDefault(x => x.ApplicationType == ApplicationCacheTypeEnum.FullPageCache.ToString())?.ToModel<CacheModel>();
            if (IsNotNull(fullPageCacheDetails))
            {
                webStorePortalModel.Duration = fullPageCacheDetails.Duration;
                webStorePortalModel.IsFullPageCacheActive = fullPageCacheDetails.IsActive;
            }
        }

        //Get Store Global AttributeData.
        private void GetStoreGlobalAttributeData(WebStorePortalModel portal)
        {
            if (IsNotNull(portal) && portal?.PortalLocales?.Count > 0 && portal.PortalLocales.Any(x => x.IsActive && x.IsDefault))
            {
                IGlobalAttributeGroupEntityService globalAttribute = GetService<IGlobalAttributeGroupEntityService>();
                portal.GlobalAttributes = globalAttribute.GetEntityAttributeDetails(portal.PortalId, ZnodeConstant.Store);
            }
        }

        //Get Page id for portal.
        private void GetPortalPageSetting(WebStorePortalModel webStorePortalModel, int portalId)
        {
            webStorePortalModel.PageList = (from pp in _portalPageSettingRepository.Table
                                            join p in _pageSettingRepository.Table on pp.PageSettingId equals p.PageSettingId
                                            where pp.PortalId == portalId
                                            select new PortalPageSettingModel() { PageDisplayName = pp.PageDisplayName, PageValue = p.PageValue, DisplayOrder = p.DisplayOrder, IsDefault = pp.IsDefault }).OrderBy(x => x.DisplayOrder)?.ToList() ?? null;

        }
        //Get Sort id for portal.
        private void GetPortalSortSetting(WebStorePortalModel webStorePortalModel, int portalId)
        {
            webStorePortalModel.SortList = (from pp in _portalSortSettingRepository.Table
                                            join p in _sortSettingRepository.Table on pp.SortSettingId equals p.SortSettingId
                                            where pp.PortalId == portalId
                                            select new PortalSortSettingModel() { SortDisplayName = pp.SortDisplayName, SortValue = p.SortValue, DisplayOrder = p.DisplayOrder})?.OrderBy(x => x.DisplayOrder).ToList() ?? null;

        }
        //Get Sort ApprovalManagement for portal.
        private void GetApprovalManagement(WebStorePortalModel webStorePortalModel, int portalId)
        {
            if(IsNotNull(webStorePortalModel) && portalId > 0)
                webStorePortalModel.EnableApprovalManagement = (_portalApprovalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.EnableApprovalManagement).GetValueOrDefault();
        }
        #endregion

        #region Protected Methods
        //Get Portal Tracking Pixel Data.
        protected virtual void GetPortalTrackingPixelForPortal(int portalId, WebStorePortalModel webStorePortalModel)
        {
            PortalTrackingPixelModel portalPixelSettings = GetPortalTrackingPixel(portalId, null);
            webStorePortalModel.TrackingPixelScriptCode = portalPixelSettings.TrackingPixelScriptCode;
        }
        #endregion

        #region
        // For Klaviyo Setting
        private void GetPortalKlaviyoSetting(int portalId, WebStorePortalModel webStorePortalModel)
        {
            IZnodeRepository<ZnodePortalKlaviyoSetting> _znodeKlaviyoRepository = new ZnodeRepository<ZnodePortalKlaviyoSetting>();
            ZnodePortalKlaviyoSetting znodePortalKlaviyoSetting = _znodeKlaviyoRepository.Table?.FirstOrDefault(x => x.PortalId == portalId);
            if (IsNotNull(znodePortalKlaviyoSetting))
            {
                webStorePortalModel.IsKlaviyoEnable = znodePortalKlaviyoSetting.IsActive;
             }
        }
        #endregion

        #region SMSFlagDetails
        // For SMSFlagDetails
        protected virtual bool IsSmsProviderEnabled(int portalId)
        {
            bool isSmsProviderEnabled = false;
            IZnodeRepository<ZnodePortalSmsSetting> _portalSmsSettingRepository = new ZnodeRepository<ZnodePortalSmsSetting>();
            ZnodePortalSmsSetting znodePortalSmsSetting = _portalSmsSettingRepository.Table?.FirstOrDefault(x => x.PortalId == portalId);
            if (IsNotNull(znodePortalSmsSetting))
            {
                isSmsProviderEnabled = znodePortalSmsSetting.IsSMSSettingEnabled;
            }
            return isSmsProviderEnabled;
        }
        #endregion
    }
}
