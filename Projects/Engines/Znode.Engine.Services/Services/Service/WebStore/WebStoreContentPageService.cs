using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public partial class ContentPageService 
    {
        #region Public Methods
        //Get Content page list.
        public virtual WebStoreContentPageListModel GetContentPagesList(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            int portalId,localeId = 0;
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);

            string profileId = filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.ProfileIds, StringComparison.InvariantCultureIgnoreCase))?.FilterValue;

            WebStoreContentPageListModel listModel = new WebStoreContentPageListModel();

            listModel.ContentPageList = GetContentPageConfigData(profileId, portalId);

            GetContentPageData(localeId, listModel.ContentPageList, portalId);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return listModel;
        }
        #endregion

        #region Private Methods

        //Get content page data .
        protected virtual List<WebStoreContentPageModel> GetContentPageConfigData(string profileId, int portalId)
        {

            List<ZnodePublishContentPageConfigEntity> list = ZnodeDependencyResolver.GetService<IPublishedPortalDataService>().GetContentPageList(portalId, profileId);

            ZnodeLogging.LogMessage("ContentPageConfigEntity list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, list?.Count);
            return GetFilterDateResult(list)?.ToModel<WebStoreContentPageModel>().ToList();
        }

        //Get Content page seo data
        protected virtual void GetContentPageData(int localeId, List<WebStoreContentPageModel> contentPageList, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter portalId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, portalId);

            IPublishedPortalDataService publishedDataService = ZnodeDependencyResolver.GetService<IPublishedPortalDataService>();

            //Get the text widget details
            List<ZnodePublishTextWidgetEntity> textWidgetData = publishedDataService.GetTextWidgetList(portalId, localeId);

            List<ZnodePublishSeoEntity> seoSettings = publishedDataService.GetSEOSettings(GetFiltersForSEO(portalId, localeId, ZnodeConstant.ContentPage));

            ZnodeCMSPortalSEOSetting portalSeoSetting = _portalSEOSettingRepository.Table.FirstOrDefault(x => x.PortalId == portalId);
           
            //Assign content page seo details.
            if (contentPageList?.Count > 0)
            {
                contentPageList.ForEach(contentPage =>
                {
                    //Get Content page Text.
                    ZnodePublishTextWidgetEntity textWidgetEntity = textWidgetData?.FirstOrDefault(widget => widget.MappingId == contentPage.ContentPageId && widget.TypeOFMapping == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString());
                    if (HelperUtility.IsNotNull(textWidgetEntity))
                    {
                        contentPage.Text = textWidgetEntity.Text;
                        contentPage.WidgetsKey = textWidgetEntity.WidgetsKey;
                        contentPage.TypeOFMapping = textWidgetEntity.TypeOFMapping;
                        contentPage.MappingId = textWidgetEntity.MappingId;
                    }

                    //Get content Page seo.
                    ZnodePublishSeoEntity seo = seoSettings?
                                .FirstOrDefault(seoDetail => seoDetail.SEOCode == contentPage.PageName);

                    if (HelperUtility.IsNotNull(seo))
                    {
                        contentPage.SEOUrl = seo.SEOUrl;
                        contentPage.SEODescription = GetSeoDetails(seo.SEODescription, portalSeoSetting?.ContentDescription, contentPage);
                        contentPage.SEOTitle = GetSeoDetails(seo.SEOTitle, portalSeoSetting?.CategoryTitle, contentPage);
                        contentPage.SEOKeywords = GetSeoDetails(seo.SEOKeywords, portalSeoSetting?.ContentKeyword, contentPage);
                        contentPage.CanonicalURL = seo.CanonicalURL ?? string.Empty;
                        contentPage.RobotTag = seo.RobotTag ?? string.Empty;
                    }
                });
            }


            foreach (ZnodePublishTextWidgetEntity entity in textWidgetData)
            {
                contentPageList?.Add(new WebStoreContentPageModel
                {
                    Text = entity.Text,
                    WidgetsKey = entity.WidgetsKey,
                    TypeOFMapping = entity.TypeOFMapping,
                    MappingId = entity.MappingId,
                    ContentPageId = entity.MappingId,
                    LocaleId = entity.LocaleId,
                });
            }
            ZnodeLogging.LogMessage("contentPageList count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, contentPageList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
        }
        #endregion

        //Get SEO according to portal default setting.
        protected virtual  string GetSeoDetails(string actualSEOSettings, string siteConfigSEOSettings, WebStoreContentPageModel entity)
        {
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { actualSEOSettings = actualSEOSettings, siteConfigSEOSettings = siteConfigSEOSettings });
            string seoDetailsText = actualSEOSettings;
            if (string.IsNullOrEmpty(actualSEOSettings) && !string.IsNullOrEmpty(siteConfigSEOSettings))
            {
                string seoDetails = siteConfigSEOSettings;
                seoDetails = seoDetails.Replace(ZnodeConstant.DefaultSEOName, entity.PageName);
                seoDetailsText = seoDetails;
            }
            ZnodeLogging.LogMessage("seoDetailsText to be returned: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, seoDetailsText);
            return seoDetailsText;
        }

        private List<ZnodePublishContentPageConfigEntity> GetFilterDateResult(List<ZnodePublishContentPageConfigEntity> list) =>
        list.Where(x => (x.ActivationDate == null || x.ActivationDate.GetValueOrDefault().Date <= HelperUtility.GetDate()) && (x.ExpirationDate == null || x.ExpirationDate.GetValueOrDefault().Date >= HelperUtility.GetDate())).ToList();


        protected virtual FilterCollection GetFiltersForSEO(int? portalId, int localeId, string seoTypeName)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple("SEOTypeName", FilterOperators.Is, seoTypeName));
            filters.Add(new FilterTuple("LocaleId", FilterOperators.Equals, localeId.ToString()));
            filters.Add(new FilterTuple("VersionId", FilterOperators.Equals, WebstoreVersionId.ToString()));
            if (HelperUtility.IsNotNull(portalId))
                filters.Add(new FilterTuple("PortalId", FilterOperators.Equals, portalId.ToString()));
            return filters;
        }


    }

   
}
