using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Constants;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using System.Diagnostics;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public abstract class BaseService : ZnodeBusinessBase
    {
        #region Properties
        protected int PortalId
        {
            get
            {
                string domainName = GetPortalDomainName() ?? HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").Trim();
                return Convert.ToInt32(ZnodeConfigManager.GetSiteConfig(domainName)?.PortalId);
            }
        }

        protected int? WebstoreVersionId
        {
            get { return GetWebstoreVersionId(); }
        }

        protected int? GlobalVersionId
        {
            get { return GetGlobalVersionId(); }
        }

        protected int PublishStateId
        {
            get { return (byte)GetPortalPublishState(); }
        }
        #endregion

        #region Protected Methods

        //Adds expands to navigation properties
        protected virtual void SetExpands(string navProp, List<string> navigationProperties)
        {
            navigationProperties.Add(navProp);
        }

        //Set default Page Size & Index based on Page Collection.
        protected virtual void SetPaging(NameValueCollection page, out int pagingStart, out int pagingLength)
        {
            // We use int.MaxValue for the paging length to ensure we always get total results back
            pagingStart = 1;
            pagingLength = int.MaxValue;

            // Only do if both index and size are given
            if (!Equals(page, null) && page.HasKeys() && !string.IsNullOrEmpty(page.Get(PageKeys.Index)) && !string.IsNullOrEmpty(page.Get(PageKeys.Size)))
            {
                pagingStart = Convert.ToInt32(page.Get(PageKeys.Index));
                pagingLength = Convert.ToInt32(page.Get(PageKeys.Size));
            }
        }

        //Get User ID of logged in user.
        protected virtual int GetLoginUserId()
        {
            const string headerUserId = "Znode-UserId";
            int userId = 0;
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerUserId], out userId);
            ZnodeLogging.LogMessage("userId: ", string.Empty, TraceLevel.Verbose, userId);
            return userId;
        }

        //Get Profile Id of logged in user.
        protected virtual int GetProfileId()
        {
            const string headerProfileId = "Znode-ProfileId";
            int profileId = 0;
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerProfileId], out profileId);
            ZnodeLogging.LogMessage("profileId: ", string.Empty, TraceLevel.Verbose, profileId);
            return profileId;
        }
        #region email Templates

        /// <summary>
        /// Get Email template Details by Code.
        /// </summary>
        /// <param name="code"></param>
        protected virtual EmailTemplateMapperModel GetEmailTemplateByCode(string code, int portalId = 0, int localeId = 0)
        {
            //Set Default Locale, in case no locale exists.
            if (localeId == 0)
                localeId = GetDefaultLocaleId();

            List<EmailTemplateMapperModel> emailTemplates = GetEmailTemplateList(code, portalId, localeId);

            if (!emailTemplates.Any())
            {
                localeId = GetDefaultLocaleId();

                List<EmailTemplateMapperModel> emailTemplatesByDefaultLocale = GetEmailTemplateList(code, portalId, localeId);
                return emailTemplatesByDefaultLocale.FirstOrDefault();
            }
            //Get the Configured Email Template Details based on the Template Code & Locale.
            return emailTemplates.FirstOrDefault();
        }

        //This is used to fetch all the required email templates based on email template codes
        protected virtual List<EmailTemplateMapperModel> GetOrderEmailTemplateListByCodes(string code, int portalId = 0, int localeId = 0)
        {
            //Set Default Locale, in case no locale exists.
            if (localeId == 0)
                localeId = GetDefaultLocaleId();

            List<EmailTemplateMapperModel> emailTemplates = GetEmailTemplates(code, portalId, localeId);

            //Get the Configured Email Template Details based on the Template Code & Locale.
            return emailTemplates;
        }

        //Get Email Template List
        private List<EmailTemplateMapperModel> GetEmailTemplates(string codes, int portalId, int localeId)
        {
            ZnodeRepository<ZnodeEmailTemplateArea> _emailTemplateAreaRepository = new ZnodeRepository<ZnodeEmailTemplateArea>();
            ZnodeRepository<ZnodeEmailTemplateMapper> _emailTemplateMapperRepository = new ZnodeRepository<ZnodeEmailTemplateMapper>();
            ZnodeRepository<ZnodeEmailTemplate> _emailTemplateRepository = new ZnodeRepository<ZnodeEmailTemplate>();
            ZnodeRepository<ZnodeEmailTemplateLocale> _emailTemplateLocaleRepository = new ZnodeRepository<ZnodeEmailTemplateLocale>();

            return (from emailTemplateArea in _emailTemplateAreaRepository.Table
                    join emailTemplateMapper in _emailTemplateMapperRepository.Table on emailTemplateArea.EmailTemplateAreasId equals emailTemplateMapper.EmailTemplateAreasId
                    join emailTemplate in _emailTemplateRepository.Table on emailTemplateMapper.EmailTemplateId equals emailTemplate.EmailTemplateId
                    join emailTemplateLocale in _emailTemplateLocaleRepository.Table on emailTemplate.EmailTemplateId equals emailTemplateLocale.EmailTemplateId
                    where codes.Contains(emailTemplateArea.Code)  && emailTemplateMapper.PortalId == portalId && emailTemplateMapper.IsActive && emailTemplateLocale.LocaleId == localeId
                    select new EmailTemplateMapperModel()
                    {
                        Code = emailTemplateArea.Code,
                        EmailTemplateId = emailTemplate.EmailTemplateId,
                        EmailTemplateAreasId = emailTemplateArea.EmailTemplateAreasId,
                        TemplateName = emailTemplate.TemplateName,
                        Descriptions = emailTemplateLocale.Content,
                        Subject = emailTemplateLocale.Subject,
                        IsEnableBcc = emailTemplateMapper.IsEnableBcc,
                    }).ToList();
        }

        private  List<EmailTemplateMapperModel> GetEmailTemplateList(string code, int portalId, int localeId)
        {
            ZnodeRepository<ZnodeEmailTemplateArea> _emailTemplateAreaRepository = new ZnodeRepository<ZnodeEmailTemplateArea>();
            ZnodeRepository<ZnodeEmailTemplateMapper> _emailTemplateMapperRepository = new ZnodeRepository<ZnodeEmailTemplateMapper>();
            ZnodeRepository<ZnodeEmailTemplate> _emailTemplateRepository = new ZnodeRepository<ZnodeEmailTemplate>();
            ZnodeRepository<ZnodeEmailTemplateLocale> _emailTemplateLocaleRepository = new ZnodeRepository<ZnodeEmailTemplateLocale>();

            return (from emailTemplateArea in _emailTemplateAreaRepository.Table
                    join emailTemplateMapper in _emailTemplateMapperRepository.Table on emailTemplateArea.EmailTemplateAreasId equals emailTemplateMapper.EmailTemplateAreasId
                    join emailTemplate in _emailTemplateRepository.Table on emailTemplateMapper.EmailTemplateId equals emailTemplate.EmailTemplateId
                    join emailTemplateLocale in _emailTemplateLocaleRepository.Table on emailTemplate.EmailTemplateId equals emailTemplateLocale.EmailTemplateId
                    where emailTemplateArea.Code == code && emailTemplateMapper.PortalId == portalId && emailTemplateMapper.IsActive && emailTemplateLocale.LocaleId == localeId
                    select new EmailTemplateMapperModel()
                    {
                        Code = emailTemplateArea.Code,
                        EmailTemplateId = emailTemplate.EmailTemplateId,
                        EmailTemplateAreasId = emailTemplateArea.EmailTemplateAreasId,
                        TemplateName = emailTemplate.TemplateName,
                        Descriptions = emailTemplateLocale.Content,
                        Subject = emailTemplateLocale.Subject,
                        IsEnableBcc = emailTemplateMapper.IsEnableBcc,
                    }).ToList();
        }

        #endregion

        /// <summary>
        /// Replace filter key name 
        /// </summary>
        /// <param name="filters">Reference of filter collection</param>
        /// <param name="keyName">Old Key Name</param>
        /// <param name="newKeyName">New key name</param>
        protected virtual void ReplaceFilterKeyName(ref FilterCollection filters, string keyName, string newKeyName)
        {
            FilterCollection tempCollection = new FilterCollection();

            for (int i = 0; i < filters.Count; i++)
            {
                FilterTuple tuple = filters[i];
                if (Equals(tuple.Item1.ToLower(), keyName.ToLower()))
                    tempCollection.Insert(i, new FilterTuple(newKeyName, tuple.Item2, tuple.Item3));
                else
                    tempCollection.Insert(i, tuple);
            }
            ZnodeLogging.LogMessage("filter key and new filter key name: ", string.Empty, TraceLevel.Verbose, new object[] { newKeyName, newKeyName });
            filters = tempCollection;
        }

        /// <summary>
        /// Replace sort key name
        /// </summary>
        /// <param name="sort">Sort collection</param>
        /// <param name="keyName">Old key name</param>
        /// <param name="newKeyName">New key name</param>
        protected virtual void ReplaceSortKeyName(ref NameValueCollection sort, string keyName, string newKeyName)
        {
            if (HelperUtility.IsNotNull(sort))
            {
                NameValueCollection newCollection = new NameValueCollection();

                for (int index = 0; index < sort.Keys.Count; index++)
                {
                    if (sort.Keys.Get(index) == keyName)
                        newCollection.Add(newKeyName, sort.GetValues(index)?.FirstOrDefault());
                }
                ZnodeLogging.LogMessage("Sort key and new sort key name: ", string.Empty, TraceLevel.Verbose, new object[] { newKeyName, newKeyName });
                sort = newCollection;
            }
        }

        //Get list of published portals.
        protected virtual int[] GetPublishedPortals()
        {
            string publishedPortal = string.Empty;

            IZnodeRepository<ZnodePublishWebstoreEntity> _webstoreVersionEntity = new ZnodeRepository<ZnodePublishWebstoreEntity>(HelperMethods.Context);

            IEnumerable<int> portals  = _webstoreVersionEntity.Table.Select(x => x.PortalId).Distinct();

            ZnodeLogging.LogMessage("publishedPortal: ", string.Empty, TraceLevel.Verbose, publishedPortal);
            return portals.ToArray();
        }

        //Get the Authorized portal Ids for the login user.       
        protected virtual string GetAvailablePortals(int userId = 0)
        {
            string portalAccess = string.Empty;

            //Get User associated portals.
            ZnodeLogging.LogMessage("userId to get user portal list ", string.Empty, TraceLevel.Verbose, userId);
            List<ZnodeUserPortal> userPortals = GetUserPortal(userId);
            ZnodeLogging.LogMessage("userPortals list count: ", string.Empty, TraceLevel.Verbose, userPortals?.Count);

            if (HelperUtility.IsNotNull(userPortals) && userPortals.Count > 0)
            {
                if (userPortals.Any(x => x.PortalId == null))
                {
                    List<int> portalIds = GetPortalListFromCache();
                    if (portalIds.Count > 0)
                        portalAccess = string.Join(",", portalIds);
                }
                else
                    portalAccess = string.Join(",", userPortals.Select(x => x.PortalId));
            }

            ZnodeLogging.LogMessage("portalAccess: ", string.Empty, TraceLevel.Verbose, portalAccess);
            return portalAccess;
        }

        //Bind the Filter conditions for the authorized portal access.
        protected virtual void BindUserPortalFilter(ref FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            //Get Authorized Portal Access for login User.
            string portalAccess = GetAvailablePortals();

            //Checking for portal id already exists in filters or not. 
            if (!filters.Exists(x => x.Item1.ToLower() == ZnodePortalEnum.PortalId.ToString().ToLower()))
            {
                //Set the filters for Authorized portal access, for the login user.
                if (!string.IsNullOrEmpty(portalAccess))
                    filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.In, portalAccess));
            }
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
        }

        //Returns the default content state to be used as the final destination for published content.
        //Falls back to PRODUCTION in case the default publish state could not be acquired from database.
        protected virtual ZnodePublishStatesEnum GetDefaultPublishState()
        {
            if (Equals(HttpRuntime.Cache["DefaultPublishState"], null))
            {
                ZnodePublishStatesEnum publishState = FetchDefaultPublishState();
                HttpRuntime.Cache.Insert("DefaultPublishState", publishState);
            }

            return (ZnodePublishStatesEnum)HttpRuntime.Cache.Get("DefaultPublishState");
        }

        //Get Content state mapped to supplied application type.
        protected virtual ZnodePublishStatesEnum GetPublishStateFromApplicationType(ApplicationTypesEnum applicationType)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            List<PublishStateMappingModel> applicationTypeMappings = GetAvailablePublishStateMappings();
            ZnodeLogging.LogMessage("applicationTypeMappings list count: ", string.Empty, TraceLevel.Verbose, applicationTypeMappings?.Count);

            ZnodePublishStatesEnum publishState = 0;

            if (HelperUtility.IsNotNull(applicationTypeMappings))
            {
                string publishStateCode = applicationTypeMappings.Where(x => x.ApplicationType == applicationType.ToString() && x.IsEnabled)?.FirstOrDefault()?.PublishState;

                if (!string.IsNullOrEmpty(publishStateCode))
                    Enum.TryParse(publishStateCode, out publishState);
            }

            ZnodeLogging.LogMessage("publishState: ", string.Empty, TraceLevel.Verbose, publishState);
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return publishState;
        }

        //Get Portal Details required for sending email content. Also get the configured domain url based on the WebStore Application type.
        protected virtual PortalModel GetCustomPortalDetails(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            PortalModel model = new PortalModel();
            if (portalId > 0)
            {
                IZnodeRepository<ZnodePortal> znodePortal = new ZnodeRepository<ZnodePortal>();
                IZnodeRepository<ZnodeDomain> znodePortalDomain = new ZnodeRepository<ZnodeDomain>();
                ZnodeLogging.LogMessage("portalId to get storeLogo: ", string.Empty, TraceLevel.Verbose, portalId);
                string storeLogo = GetStoreLogoPath(portalId);
                ZnodeLogging.LogMessage("storeLogo: ", string.Empty, TraceLevel.Verbose, storeLogo);

                model = (from portal in znodePortal.Table
                         join domain in znodePortalDomain.Table.Where(x => x.PortalId == portalId && x.ApplicationType == ApplicationTypesEnum.WebStore.ToString() && x.IsActive && x.IsDefault) on portal.PortalId equals domain.PortalId into pf
                         from portalDomain in pf.DefaultIfEmpty()
                         where portal.PortalId == portalId
                         select new PortalModel
                         {
                             StoreName = portal.StoreName,
                             AdministratorEmail = portal.AdminEmail,
                             DomainUrl = portalDomain.DomainName,
                             StoreLogo = storeLogo,
                             IsEnableSSL = portal.UseSSL,
                             CustomerServiceEmail = portal.CustomerServiceEmail,
                             CustomerServicePhoneNumber = portal.CustomerServicePhoneNumber
                         }).FirstOrDefault();
            }
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return model;
        }

        // Get SKUs by product Id(s).
        protected virtual List<View_PimProductAttributeValue> GetSKUList(string PimProductIds)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            IZnodeRepository<View_PimProductAttributeValue> _viewLoadManageProductRepository = new ZnodeRepository<View_PimProductAttributeValue>();
            FilterCollection filter = new FilterCollection
            {
                new FilterTuple(View_PimProductAttributeValueEnum.PimProductId.ToString(), FilterOperators.In, PimProductIds?.ToString()),
                new FilterTuple(View_PimProductAttributeValueEnum.AttributeCode.ToString(), FilterOperators.Is, ZnodeConstant.ProductSKU)
            };
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection());
            ZnodeLogging.LogMessage("PimProductIds to get SKU list: ", string.Empty, TraceLevel.Verbose, PimProductIds?.ToString());
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return _viewLoadManageProductRepository.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();
        }

        //Get current catalog version id by catalog id.
        protected virtual int? GetCatalogVersionId(int publishCatalogId = 0, int localeId = 0, int portalId = 0)
        {
            var headers = HttpContext.Current.Request.Headers;

            if (!string.IsNullOrEmpty(headers["Znode-Locale"]))
                int.TryParse(headers["Znode-Locale"], out localeId);

            IZnodeRepository<ZnodePublishVersionEntity> _versionEntity = new ZnodeRepository<ZnodePublishVersionEntity>(HelperMethods.Context);
            ZnodePublishStatesEnum contentState = GetPortalPublishState();

            publishCatalogId = publishCatalogId > 0 ? publishCatalogId : GetPortalCatalogId().GetValueOrDefault();
            localeId = localeId > 0 ? localeId : GetDefaultLocaleId();

            int? version = (from versionEntity in _versionEntity.Table.Where(x => x.ZnodeCatalogId == publishCatalogId && x.LocaleId == localeId && x.RevisionType == contentState.ToString() && x.IsPublishSuccess) select versionEntity.VersionId).FirstOrDefault();
  
            return version.HasValue ? version : 0;
          
        }

        //Get current catalog version id by catalog id.
        protected virtual int? GetCatalogVersionId(int publishCatalogId, ZnodePublishStatesEnum contentState)
        {
            IZnodeRepository<ZnodePublishVersionEntity> _versionEntity = new ZnodeRepository<ZnodePublishVersionEntity>(HelperMethods.Context);

            int localeId = GetPortalLocale() > 0 ? GetPortalLocale() : GetDefaultLocaleId();
            publishCatalogId = publishCatalogId > 0 ? publishCatalogId : GetPortalCatalogId().GetValueOrDefault();

            int? version = Convert.ToInt32(_versionEntity.Table.FirstOrDefault(x => x.ZnodeCatalogId == publishCatalogId && x.LocaleId == localeId && x.RevisionType == contentState.ToString() && x.IsPublishSuccess)?.VersionId);

            return version.HasValue ? version : 0;

        }

        protected virtual int? GetCatalogVersionForDefaultPublishState(int portalId = 0, int localeId = 0, int? publishCatalogId = null)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            IZnodeRepository<ZnodePublishVersionEntity> _versionEntity = new ZnodeRepository<ZnodePublishVersionEntity>(HelperMethods.Context);

            ZnodePublishStatesEnum contentState = GetDefaultPublishState();

            //Check for portalId and get the catalog associated to that portal.
            if (publishCatalogId < 1 && portalId > 0)
                publishCatalogId = GetPortalCatalogId(portalId);

            localeId = localeId > 0 ? localeId : GetDefaultLocaleId();

            List<ZnodePublishVersionEntity> versionEntity = _versionEntity.Table.Where(x => x.LocaleId == localeId && x.RevisionType == contentState.ToString())?.ToList();

            int? versionId = publishCatalogId.HasValue && publishCatalogId.Value > 0 ? versionEntity.FirstOrDefault(x => x.ZnodeCatalogId == publishCatalogId)?.VersionId : versionEntity.FirstOrDefault()?.VersionId;

            ZnodeLogging.LogMessage("portalId, localeId and publishCatalogId to get CatalogVersionId : ", string.Empty, TraceLevel.Verbose, new object[] { portalId, localeId, publishCatalogId });
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return versionId;
        }


        protected virtual int? GetPortalCatalogId(int portalId = 0)
        {
            return GetPortalPublishCatalogFromCache(portalId > 0 ? portalId : this.PortalId);
        }

        //Get current catalog All version id separeted by comma.
        protected virtual string GetCatalogAllVersionIds()
        {
            IZnodeRepository<ZnodePublishVersionEntity> _versionEntity = new ZnodeRepository<ZnodePublishVersionEntity>(HelperMethods.Context);
            return string.Join(",", _versionEntity.Table.Select(x => x.VersionId));
        }

        //Get current catalog version id separeted by comma based on revisiontype production.
        protected virtual string GetCatalogAllVersionIds(int localeId)
        {
            IZnodeRepository<ZnodePublishVersionEntity> _versionEntity = new ZnodeRepository<ZnodePublishVersionEntity>(HelperMethods.Context);
            localeId = localeId > 0 ? localeId : GetDefaultLocaleId();
            return string.Join(",", _versionEntity.Table.Where(x => x.LocaleId == localeId && x.RevisionType == ZnodePublishStatesEnum.PRODUCTION.ToString())?.Select(x=>x.VersionId));
        }
        //Check for User Portal Access.
        protected virtual void CheckUserPortalAccess(int portalId)
        {
            ZnodeLogging.LogMessage("portalId to check user portal access: ", string.Empty, TraceLevel.Verbose, portalId);
            if (portalId > 0)
            {
                string portalAccess = GetAvailablePortals();
                if (!string.IsNullOrEmpty(portalAccess))
                {
                    bool hasPortalAccess = portalAccess.Split(',').Any(x => x == portalId.ToString());
                    if (!hasPortalAccess)
                        throw new ZnodeUnauthorizedException(ErrorCodes.NotPermitted, Admin_Resources.UserDontHaveTheRequestedPortalAccess, HttpStatusCode.Unauthorized);
                }
            }
        }

        protected virtual void CheckUserPortalAccessForLogin(int portalId, int userId = 0)
        {
            if (portalId > 0)
            {
                ZnodeLogging.LogMessage("portalId and userId to check portal access for login: ", string.Empty, TraceLevel.Verbose, new object[] { portalId, userId });
                string portalAccess = GetAvailablePortals(userId);
                if (!string.IsNullOrEmpty(portalAccess))
                {
                    bool hasPortalAccess = portalAccess.Split(',').Any(x => x == portalId.ToString());
                    if (!hasPortalAccess)
                        throw new ZnodeUnauthorizedException(ErrorCodes.NotPermitted, Admin_Resources.UserDontHaveTheRequestedPortalAccess, HttpStatusCode.Unauthorized);
                }
            }
        }

        //Get Active locale list.

        protected static List<LocaleModel> GetActiveLocaleList()
        {
            List<LocaleModel> data = Equals(HttpRuntime.Cache["ActiveLocaleList"], null)
               ? InsertAndGetActiveLocales()
               : (List<LocaleModel>)HttpRuntime.Cache.Get("ActiveLocaleList");

            ZnodeLogging.LogMessage("Active portal list: ", string.Empty, TraceLevel.Verbose, data?.Count);
            return data;
        }



        //Get locale name by locale id.
        protected static string GetLocaleName(int localeId)
        {
            LocaleModel locale = GetActiveLocaleList()?.FirstOrDefault(x => x.LocaleId == localeId);

            ZnodeLogging.LogMessage("Locale name: ", string.Empty, TraceLevel.Verbose, locale?.Name);
            return locale?.Name;
        }

        //Get userName by userId. 
        protected static UserModel GetUserNameByUserId(int userId)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodeLogging.LogMessage("userId to get user name: ", string.Empty, TraceLevel.Verbose, userId);
            if (userId > 0)
            {
                IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();
                IZnodeRepository<AspNetUser> _aspNetUserRepository = new ZnodeRepository<AspNetUser>();
                IZnodeRepository<AspNetZnodeUser> _aspNetZnodeUserRepository = new ZnodeRepository<AspNetZnodeUser>();

                return (from user in _userRepository.Table
                        join aspNetUser in _aspNetUserRepository.Table on user.AspNetUserId equals aspNetUser.Id
                        join aspNetZnodeUser in _aspNetZnodeUserRepository.Table on aspNetUser.UserName equals aspNetZnodeUser.AspNetZnodeUserId
                        where user.UserId == userId
                        select new UserModel
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            UserName = aspNetZnodeUser.UserName,
                            PhoneNumber = user.PhoneNumber,
                            Email = user.Email
                        })?.FirstOrDefault();
            }
            return new UserModel();
        }

        //Get the list of all global attributes for the user
        protected virtual List<GlobalAttributeValuesModel> GetGlobalLevelAttributeList(int entityId, string entityType)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            List<GlobalAttributeValuesModel> userAttributes = new List<GlobalAttributeValuesModel>();
            if (entityId > 0 && !string.IsNullOrEmpty(entityType))
            {
                IZnodeViewRepository<GlobalAttributeValuesModel> globalAttributeValues = new ZnodeViewRepository<GlobalAttributeValuesModel>();
                globalAttributeValues.SetParameter("EntityName", entityType, ParameterDirection.Input, DbType.String);
                globalAttributeValues.SetParameter("GlobalEntityValueId", entityId, ParameterDirection.Input, DbType.Int32);
                ZnodeLogging.LogMessage("entityId and entityType to get user attributes: ", string.Empty, TraceLevel.Verbose, new object[] { entityId, entityType });
                userAttributes = globalAttributeValues.ExecuteStoredProcedureList("Znode_GetGlobalEntityAttributeValue @EntityName, @GlobalEntityValueId").ToList();
            }
            ZnodeLogging.LogMessage("userAttributes list count: ", string.Empty, TraceLevel.Verbose, userAttributes.Count);
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return userAttributes;
        }

        //Get the Media Server Url
        protected static string GetMediaServerUrl(MediaConfigurationModel configuration)
        {
            if (HelperUtility.IsNotNull(configuration))
            {
                return string.IsNullOrWhiteSpace(configuration.CDNUrl) ? configuration.URL
                           : configuration.CDNUrl.EndsWith("/") ? configuration.CDNUrl : $"{configuration.CDNUrl}/";
            }
            return string.Empty;
        }

        //Gets default culture code.
        protected virtual string GetDefaultCulture()
        {
            IDefaultGlobalConfigService defaultGlobalConfigService = new DefaultGlobalConfigService();
            List<DefaultGlobalConfigModel> defaultGlobalSettingData = defaultGlobalConfigService.GetDefaultGlobalConfigList()?.DefaultGlobalConfigs;
            return defaultGlobalSettingData?.Where(x => string.Equals(x.FeatureName, GlobalSettingEnum.Culture.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Select(x => x.FeatureValues)?.FirstOrDefault();
        }

        #endregion

        #region Public Method


        public static int GetDefaultLocaleId()
            => GetActiveLocaleList()?.FirstOrDefault()?.LocaleId ?? 0;

        /// <summary>
        /// Set locale filter if not present
        /// </summary>
        /// <param name="filters">filters</param>
        public virtual void SetLocaleFilterIfNotPresent(ref FilterCollection filters)
        {
            filters = HelperUtility.IsNull(filters) ? new FilterCollection() : filters;

            if (!filters.Any(x => x.FilterName.ToLower() == ZnodeLocaleEnum.LocaleId.ToString().ToLower()))
                filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, DefaultGlobalConfigSettingHelper.Locale);
        }

        /// <summary>
        /// Set locale filter if not present
        /// </summary>
        /// <param name="filters">filters</param>
        public virtual void SetVersionFilterIfNotPresent(ref FilterCollection filters)
        {
            filters = HelperUtility.IsNull(filters) ? new FilterCollection() : filters;
            int? versionId = GetCatalogVersionId();
            if (versionId.HasValue && !filters.Any(x => x.FilterName.ToLower() == WebStoreEnum.VersionId.ToString().ToLower()))
                filters.Add(WebStoreEnum.VersionId.ToString(), FilterOperators.Equals, versionId.Value.ToString());
        }

        //Sets the product index filter while taking unique products from published data.
        public virtual void SetProductIndexFilter(FilterCollection filters)
        {
            if (!filters.Any(filter => (filter.FilterName == Znode.Libraries.ECommerce.Utilities.FilterKeys.ZnodeCategoryIds
                                    && (filter.FilterOperator == FilterOperators.Equals || filter.FilterOperator == FilterOperators.In)
                                    && !string.IsNullOrEmpty(filter.FilterValue.Trim())))
             && !filters.Any(filter => filter.FilterName == Znode.Libraries.ECommerce.Utilities.FilterKeys.ProductIndex))
                filters.Add(new FilterTuple(Znode.Libraries.ECommerce.Utilities.FilterKeys.ProductIndex, FilterOperators.Equals, ZnodeConstant.DefaultPublishProductIndex.ToString()));
        }

        //Sets the category index filter while taking unique categories from published data.
        public virtual void SetCategoryIndexFilter(FilterCollection filters)
        {
            string IsCallFromWebstore = filters.FirstOrDefault(x => x.FilterName.Equals(WebStoreEnum.IsCallFromWebstore.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue;

            if (!Equals(IsCallFromWebstore, ZnodeConstant.TrueValue))
            {
                if (!filters.Any(filter => filter.FilterName == Znode.Libraries.ECommerce.Utilities.FilterKeys.CategoryIndex))
                    filters.Add(new FilterTuple(Znode.Libraries.ECommerce.Utilities.FilterKeys.CategoryIndex, FilterOperators.Equals, ZnodeConstant.DefaultPublishCategoryIndex.ToString()));
            }
            else
            {
                filters.RemoveAll(x => x.Item1 == WebStoreEnum.IsCallFromWebstore.ToString().ToLower());
            }
        }


        public virtual int GetCurrentVersion(int catalogLogId, int localeId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            ZnodePublishCatalogLog znodePublishCatalogLog = null;

            IZnodeRepository<ZnodePublishCatalogLog> _publishCatalogLogRepository = new ZnodeRepository<ZnodePublishCatalogLog>();

            ZnodeLogging.LogMessage("catalogLogId to get publish catalog log: ", string.Empty, TraceLevel.Verbose, catalogLogId);
            ZnodePublishCatalogLog previousPortalLog = _publishCatalogLogRepository.Table.OrderByDescending(x => x.PublishCatalogLogId).FirstOrDefault(x => x.PublishCatalogId == catalogLogId);
            if (HelperUtility.IsNotNull(previousPortalLog))
            {
                znodePublishCatalogLog = _publishCatalogLogRepository.Insert(new ZnodePublishCatalogLog { PublishCatalogId = previousPortalLog.PublishCatalogId, PimCatalogId = previousPortalLog.PimCatalogId, IsCatalogPublished = previousPortalLog.IsCatalogPublished, IsProductPublished = true, LocaleId = localeId, UserId = GetLoginUserId(), LogDateTime = DateTime.Now });
                int currentVersionId = znodePublishCatalogLog.PublishCatalogLogId;
                ZnodeLogging.LogMessage("currentVersionId: ", string.Empty, TraceLevel.Verbose, currentVersionId);
                return currentVersionId;
            }
            else
                return 0;
        }
        //This method check the access of manage screen for sales rep user (e.g order,return,quote,voucher,pending payment,pending order).
        public void ValidateAccessForSalesRepUser(string entityType, int entityId, string ReturnNo = "")
        {
            ZnodeLogging.LogMessage("Execution ValidateAccessForSalesRepUser started.", string.Empty, TraceLevel.Info);
            int salesRepId = 0; int status = 0;
            var headers = HttpContext.Current.Request.Headers;
            Int32.TryParse(headers["Znode-SaleRepAsUserId"], out salesRepId);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("SalesRepUserId", salesRepId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("EntityType", entityType, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("EntityId", entityId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("ReturnNo", ReturnNo, ParameterDirection.Input, DbType.String);
            IList<View_ReturnBoolean> accessResult = objStoredProc.ExecuteStoredProcedureList("Znode_GetSalesRepAccessConfirmation @SalesRepUserId,@EntityType,@EntityId,@Status OUT,@ReturnNo", 3, out status);

            if ((accessResult?.FirstOrDefault()?.Status != null) && (accessResult.FirstOrDefault().Status.Value == false))
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorAccessMessage, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                throw new ZnodeUnauthorizedException(ErrorCodes.NotPermitted, Admin_Resources.ErrorAccessMessage, HttpStatusCode.Unauthorized);
            }
            ZnodeLogging.LogMessage("Execution ValidateAccessForSalesRep end.", string.Empty, TraceLevel.Info);
        }
        #endregion

        #region Private Method

        //Get Webstore version id for current content state.
        private int? GetWebstoreVersionId()
        {
            IZnodeRepository<ZnodePublishWebstoreEntity> _versionEntity = new ZnodeRepository<ZnodePublishWebstoreEntity>(HelperMethods.Context);
            ZnodePublishStatesEnum publishState = GetPortalPublishState();
            int localeId = GetPortalLocale();
    
            int? version = (from webstoreVersionEntity in _versionEntity.Table.Where(x => x.PortalId == PortalId && x.PublishState == publishState.ToString() && x.LocaleId == localeId)
                            select webstoreVersionEntity.VersionId).FirstOrDefault();

            return version.HasValue? version : 0;
        }

        //Get Webstore version id for current content state.
        private int? GetGlobalVersionId()
        {
            IZnodeRepository<ZnodePublishGlobalVersionEntity> _versionEntity = new ZnodeRepository<ZnodePublishGlobalVersionEntity>(HelperMethods.Context);
            ZnodePublishStatesEnum publishState = GetPortalPublishState();
            int localeId = GetPortalLocale();
            int? version = Convert.ToInt32(_versionEntity.Table.FirstOrDefault(x => x.PublishState == publishState.ToString() && x.LocaleId == localeId)?.VersionId);
            return version.HasValue ? version : 0;
        }

        //Get Content State for this portal.
        protected virtual ZnodePublishStatesEnum GetPortalPublishState()
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            const string headerPublishState = "Znode-PublishState";
            ZnodePublishStatesEnum publishState;
            var headers = HttpContext.Current.Request.Headers;
            Enum.TryParse(headers[headerPublishState], true, out publishState);

            if (publishState == 0)
            {
                //If state not found in request header. Try to achieve the same using DomainName header of the same request.
                ApplicationTypesEnum applicationType = GetApplicationTypeForDomain();

                if (applicationType != 0)
                {
                    publishState = GetPublishStateFromApplicationType(applicationType);

                    if (publishState != 0)
                        return publishState;
                }

                //Fall back to the default content state.
                publishState = GetDefaultPublishState();
            }
            ZnodeLogging.LogMessage("Portal publish state: ", string.Empty, TraceLevel.Verbose, publishState);
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return publishState;
        }

        private string GetPortalDomainName()
        {
            const string headerDomainName = "Znode-DomainName";
            var headers = HttpContext.Current.Request.Headers;
            string domainName = headers[headerDomainName];
            ZnodeLogging.LogMessage("domainName: ", string.Empty, TraceLevel.Verbose, domainName);
            return domainName;
        }

        //Get LocaleId for this portal.
        private int GetPortalLocale()
        {
            const string headerLocaleState = "Znode-Locale";
            int localeId;
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerLocaleState], out localeId);

            //Fall back to default locale from the active locale's list.
            if (localeId < 1)
                localeId = GetActiveLocaleList().FirstOrDefault(x => x.IsDefault).LocaleId;

            ZnodeLogging.LogMessage("Portal locale Id: ", string.Empty, TraceLevel.Verbose, localeId);
            return localeId;
        }

        //Get the User Portal Details based on logged in user.
        private List<ZnodeUserPortal> GetUserPortal(int userId = 0)
        {
            
            //Get id for the logged in user.
            if (userId < 1)
                userId = GetLoginUserId();

            return GetUserPortalListFromCache(userId);
        }

        //Get Store Logo Path.
        private string GetStoreLogoPath(int portalId)
        {
            ZnodeLogging.LogMessage("portalId to generate query: ", string.Empty, TraceLevel.Verbose, portalId);
            IZnodeRepository<ZnodePublishWebstoreEntity> _webstoreVersionEntity = new ZnodeRepository<ZnodePublishWebstoreEntity>(HelperMethods.Context);
            string storeLogo = (from webstoreVersionEntity in _webstoreVersionEntity.Table
                                .Where(x => x.PortalId == portalId)
                                 select webstoreVersionEntity.WebsiteLogo).FirstOrDefault();

            MediaConfigurationModel configurationModel = GetService<IMediaConfigurationService>().GetDefaultMediaConfiguration();
            string serverPath = GetMediaServerUrl(configurationModel);
            string thumbnailPath = $"{serverPath}{configurationModel.ThumbnailFolderName}";
            return CreateImageByUrl($"{thumbnailPath}/{HttpUtility.UrlPathEncode(storeLogo)}"); //url encode the store logo image in order to escape any spaces in the image name
        }

        private string CreateImageByUrl(string url)
            => string.IsNullOrEmpty(url) ? string.Empty : $"<img src={url} class='img-responsive' style='max-width:150px;'></img></br>";

        //If active locales not present in cache then insert into cache.
        private static List<LocaleModel> InsertAndGetActiveLocales()
        {
            ZnodeRepository<View_GetLocaleDetails> _localeRepository = new ZnodeRepository<View_GetLocaleDetails>();
            List<LocaleModel> locales = new List<LocaleModel>();
            IList<View_GetLocaleDetails> entityList = _localeRepository.GetEntityList("");
            locales = entityList?.ToModel<LocaleModel>()?.ToList();
            ZnodeCacheDependencyManager.Insert("ActiveLocaleList", locales, "View_GetLocaleDetails");
            ZnodeLogging.LogMessage("Active locale list count: ", string.Empty, TraceLevel.Verbose, locales?.Count);
            return locales;
        }

        private static ZnodePublishStatesEnum FetchDefaultPublishState()
        {
            IZnodeRepository<ZnodePublishState> _publishStateRepository = new ZnodeRepository<ZnodePublishState>();
            string publishStateCode = _publishStateRepository.Table.Where(x => x.IsContentState && x.IsDefaultContentState)?.FirstOrDefault()?.PublishStateCode;

            ZnodePublishStatesEnum publishState;

            if (!string.IsNullOrEmpty(publishStateCode) && Enum.TryParse(publishStateCode, true, out publishState))
                return publishState;
            else
                return ZnodePublishStatesEnum.PRODUCTION;
        }

        protected virtual List<PublishStateMappingModel> GetAvailablePublishStateMappings()
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            if (Equals(HttpRuntime.Cache["PublishStateMappings"], null))
            {
                IZnodeRepository<ZnodePublishStateApplicationTypeMapping> _publishStateMappingRepository = new ZnodeRepository<ZnodePublishStateApplicationTypeMapping>();
                IZnodeRepository<ZnodePublishState> _publishStateRepository = new ZnodeRepository<ZnodePublishState>();

                List<PublishStateMappingModel> publishStateMappings = (from publishState in _publishStateMappingRepository.Table
                                                                       join PS in _publishStateRepository.Table on publishState.PublishStateId equals PS.PublishStateId
                                                                       where publishState.IsActive
                                                                       select new PublishStateMappingModel
                                                                       {
                                                                           PublishStateMappingId = publishState.PublishStateMappingId,
                                                                           ApplicationType = publishState.ApplicationType,
                                                                           PublishStateCode = PS.PublishStateCode,
                                                                           Description = publishState.Description,
                                                                           IsDefault = PS.IsDefaultContentState,
                                                                           IsEnabled = publishState.IsEnabled,
                                                                           PublishStateId = publishState.PublishStateId,
                                                                           PublishState = PS.PublishStateCode
                                                                       }).ToList();

                HttpRuntime.Cache.Insert("PublishStateMappings", publishStateMappings);
            }

            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return (List<PublishStateMappingModel>)HttpRuntime.Cache.Get("PublishStateMappings");
        }

        protected virtual bool IsWebstorePreviewEnabled()
        {
            return GetAvailablePublishStateMappings()?.Count(x => !x.IsDefault && x.IsEnabled) > 0;
        }

        protected virtual bool NonDefaultPublishStateExists(out ZnodePublishStatesEnum publishState)
        {
            string nonDefaultPublishState = GetAvailablePublishStateMappings()?.Where(x => !x.IsDefault && x.IsEnabled)?.FirstOrDefault()?.PublishStateCode;

            if (!string.IsNullOrEmpty(nonDefaultPublishState))
                return Enum.TryParse(nonDefaultPublishState, out publishState);

            publishState = 0;
            return false;
        }

        protected virtual ApplicationTypesEnum GetApplicationTypeForDomain()
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            IZnodeRepository<ZnodeDomain> _domainRepository = new ZnodeRepository<ZnodeDomain>();

            ApplicationTypesEnum applicationType = 0;

            string domainName = GetPortalDomainName();

            if (!string.IsNullOrEmpty(domainName))
            {
                FilterCollection filters = new FilterCollection()
                {
                    new FilterTuple(Znode.Libraries.ECommerce.Utilities.FilterKeys.DomainName, FilterOperators.Equals, "\"" + domainName + "\"")
                };

                ZnodeDomain domain = _domainRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);

                if (HelperUtility.IsNotNull(domain))
                    Enum.TryParse(domain.ApplicationType, true, out applicationType);
            }
            ZnodeLogging.LogMessage("applicationType: ", string.Empty, TraceLevel.Verbose, applicationType);
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return applicationType;
        }


        //Get the User Portal Details based on logged in user.
        protected virtual List<int> GetPortalByCatalog(int catalogId)
        {
            IZnodeRepository<ZnodePortalCatalog> _catalogPortalRepository = new ZnodeRepository<ZnodePortalCatalog>();

            //Get the User Portal Details based on logged in user.
            ZnodeLogging.LogMessage("catalogId to get portal Ids: ", string.Empty, TraceLevel.Verbose, catalogId);
            List<int> portalIds = _catalogPortalRepository.Table.Where(x => x.PublishCatalogId == catalogId).Select(x => x.PortalId).ToList();
            ZnodeLogging.LogMessage("portalIds: ", string.Empty, TraceLevel.Verbose, portalIds);
            return portalIds;
        }

        //Get the current portal of webstore from header.
        protected virtual int GetLocaleIdFromHeader()
        {
            int localeId = 0;
            var headers = HttpContext.Current.Request.Headers;

            if (!string.IsNullOrEmpty(headers["Znode-Locale"]))
                int.TryParse(headers["Znode-Locale"], out localeId);

            if (localeId == 0)
                localeId = GetDefaultLocaleId();

            ZnodeLogging.LogMessage("localeId: ", string.Empty, TraceLevel.Verbose, localeId);
            return localeId;
        }

        // This method is used to get Publish catalog against portal from cache
        private List<int> GetPortalListFromCache()
        {
            string cacheKey = "PortalListFromCache";
            if (HelperUtility.IsNull(HttpRuntime.Cache.Get(cacheKey)) || IsCacheRefresh())
            {
                IZnodeRepository<ZnodePortal> _portalRepository = new ZnodeRepository<ZnodePortal>();
                List<int> portalIds = _portalRepository.Table.Select(x => x.PortalId).ToList();

                if (HelperUtility.IsNotNull(portalIds) && portalIds.Count > 0)
                    HttpRuntime.Cache[cacheKey] = portalIds;
            }
            return HelperUtility.IsNull(HttpRuntime.Cache.Get(cacheKey)) ? null : (List<int>)(HttpRuntime.Cache.Get(cacheKey));
        }
        // This method is used to get Publish catalog against portal from cache
        private int? GetPortalPublishCatalogFromCache(int portalId)
        {
            string cacheKey = $"PortalPublishCatalogCache_{portalId}";
            if (HelperUtility.IsNull(HttpRuntime.Cache.Get(cacheKey)) || Convert.ToInt32(HttpRuntime.Cache.Get(cacheKey)) == 0 || IsCacheRefresh())
            {
                ZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
                int? publishCatalogId = _portalCatalogRepository.Table?.Where(x => x.PortalId == (portalId > 0 ? portalId : this.PortalId))?.FirstOrDefault()?.PublishCatalogId;
                if (HelperUtility.IsNotNull(publishCatalogId) && publishCatalogId > 0)
                    HttpRuntime.Cache[cacheKey] = publishCatalogId;
            }
            return (HelperUtility.IsNull(HttpRuntime.Cache.Get(cacheKey)) || Convert.ToInt32(HttpRuntime.Cache.Get(cacheKey)) == 0) ? 0 : Convert.ToInt32(HttpRuntime.Cache.Get(cacheKey));
        }

        // This method is used to get list of assign portal to user from cache
        private List<ZnodeUserPortal> GetUserPortalListFromCache(int userId)
        {
            string cacheKey = $"UserPortalCache_{userId}";
            if (HelperUtility.IsNull(HttpRuntime.Cache.Get(cacheKey)) || IsCacheRefresh())
            {
                IZnodeRepository<ZnodeUserPortal> _userPortalRepository = new ZnodeRepository<ZnodeUserPortal>();
                List<ZnodeUserPortal> userPortals = _userPortalRepository.Table.Where(x => x.UserId == userId).ToList();
                if (HelperUtility.IsNotNull(userPortals))
                    HttpRuntime.Cache[cacheKey] = userPortals;
            }
            return HelperUtility.IsNull(HttpRuntime.Cache.Get(cacheKey)) ? null : (List<ZnodeUserPortal>)HttpRuntime.Cache.Get(cacheKey);
        }
        protected virtual bool IsCacheRefresh()
        {
            var cache = HttpContext.Current.Request["cache"];
            if (!string.IsNullOrEmpty(cache) && cache.ToLower() == "refresh")
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
