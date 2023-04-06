using System;
using System.Collections.Generic;
using System.Linq;

using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.Search
{
    public class CMSContentPageSearchService : ICMSContentPageSearchService
    {
        #region Protected Variables       

        protected readonly IZnodeRepository<ZnodePublishWebstoreEntity> _publishWebstoreEntity;
        protected readonly IZnodeRepository<ZnodePublishContentPageConfigEntity> _publishContentpageConfigEntity;
        protected readonly IZnodeRepository<ZnodePublishTextWidgetEntity> _publishTextWidgetEntity;
        protected readonly IZnodeRepository<ZnodePublishSeoEntity> _publishSEOWidgetEntity;
       
        #endregion

        #region Constructor.
        public CMSContentPageSearchService()
        {
            _publishWebstoreEntity = new ZnodeRepository<ZnodePublishWebstoreEntity>(HelperMethods.Context);
            _publishContentpageConfigEntity = new ZnodeRepository<ZnodePublishContentPageConfigEntity>(HelperMethods.Context);
            _publishTextWidgetEntity = new ZnodeRepository<ZnodePublishTextWidgetEntity>(HelperMethods.Context);
            _publishSEOWidgetEntity = new ZnodeRepository<ZnodePublishSeoEntity>(HelperMethods.Context);

 
        }
        #endregion

        #region Public Methods.
        // Get the latest version id of portal.
        public virtual int GetLatestVersionId(int portalId, string revisionType, int localeId = 0)
        {
            ZnodePublishWebstoreEntity webStoreEntity = _publishWebstoreEntity.Table.FirstOrDefault(x => x.PortalId == portalId && x.PublishState == revisionType && x.LocaleId == localeId);
            return webStoreEntity != null ? webStoreEntity.VersionId : 0;
        }

        //Get the web store entity of portal.
        public virtual ZnodePublishWebstoreEntity GetWebStoreEntity(int versionId, int portalId, int localeId)
            => _publishWebstoreEntity.Table.Where(x => x.PortalId == portalId && x.VersionId == versionId && x.LocaleId == localeId).FirstOrDefault();
        

        //Get all CMS pages of perticular store.
        public virtual List<SearchCMSPages> GetAllCmsPages(int portalId, int versionId, int start, int pageLength, long indexStartTime, int localeId, out decimal totalPages)
        {
            int totalCount = 0;

            //Get all pages.
            List<ZnodePublishContentPageConfigEntity> cmsPages = _publishContentpageConfigEntity.Table.Where(x => x.PortalId == portalId && x.VersionId == versionId && x.IsActive).ToList();

            if (cmsPages?.Count() < pageLength)
                totalPages = 1;
            else
                totalPages = Math.Ceiling((decimal)totalCount / pageLength);

            WebStoreContentPageListModel listModel = new WebStoreContentPageListModel();

            GetContentPageData(localeId, listModel, cmsPages, portalId, versionId);

            ZnodePublishWebstoreEntity webStoreEntity = GetWebStoreEntity(versionId, portalId, localeId);

            List<SearchCMSPages> elasticCmsPagesList = GetElasticCmsPagesList(listModel, webStoreEntity.VersionId, indexStartTime);

            return elasticCmsPagesList;
        }

        //Get the list of all CMS pages of all version and locales of particular store.
        public virtual List<SearchCMSPages> GetAllCMSPagesOfAllVersionAndLocales(SearchCMSPagesParameterModel searchCmsPagesParameterModel, List<ZnodePublishWebstoreEntity> webStoreEntitylist)
        {
            List<SearchCMSPages> elasticAllCmsPagesList = new List<SearchCMSPages>();

           List<int> activelocalIds = searchCmsPagesParameterModel.ActiveLocales.Select(m => m.LocaleId)?.ToList();

            List<ZnodePublishContentPageConfigEntity> publishedCmsPages = _publishContentpageConfigEntity.Table.Where(x => x.PortalId == searchCmsPagesParameterModel.PortalId && activelocalIds.Contains(x.LocaleId) && x.IsActive).ToList();
            publishedCmsPages = publishedCmsPages.Where(x => (x.ActivationDate == null || x.ActivationDate.GetValueOrDefault().Date <= HelperUtility.GetDate()) && (x.ExpirationDate == null || x.ExpirationDate.GetValueOrDefault().Date >= HelperUtility.GetDate())).ToList();

            WebStoreContentPageListModel listModel = new WebStoreContentPageListModel();

            GetTextWidgetDataAllLocales(searchCmsPagesParameterModel.ActiveLocales, listModel, publishedCmsPages, searchCmsPagesParameterModel.PortalId);

            foreach (ZnodePublishWebstoreEntity webStoreEntity in webStoreEntitylist)
            {
                WebStoreContentPageListModel listModelByVersionId = new WebStoreContentPageListModel();
                listModelByVersionId.ContentPageList = listModel?.ContentPageList.Where(m => m.VersionId == webStoreEntity.VersionId && m.LocaleId == webStoreEntity.LocaleId)?.ToList();

                List<SearchCMSPages> elasticCmsPagesList = GetElasticCmsPagesList(listModelByVersionId, webStoreEntity.VersionId, searchCmsPagesParameterModel.IndexStartTime);
                elasticAllCmsPagesList?.AddRange(elasticCmsPagesList);
            }
            return elasticAllCmsPagesList;
        }

        //Get the list of all Blog and News of all version and locales of particular store.
        public virtual List<SearchCMSPages> GetAllBlogsNews(SearchCMSPagesParameterModel searchCmsPagesParameterModel)
        {
            List<SearchCMSPages> blognewslist = new List<SearchCMSPages>();

            List<int> activeLocales = searchCmsPagesParameterModel.ActiveLocales.Select(m => m.LocaleId)?.ToList();

            IZnodeRepository<ZnodePublishBlogNewsEntity> _publishBlogNewsEntity = new ZnodeRepository<ZnodePublishBlogNewsEntity>(HelperMethods.Context);
            List<ZnodePublishBlogNewsEntity> blogNewsList = _publishBlogNewsEntity.Table.Where(x => x.PortalId == searchCmsPagesParameterModel.PortalId && x.IsBlogNewsActive && activeLocales.Contains(x.LocaleId))?.ToList();
            blogNewsList = blogNewsList.Where(x => (x.ActivationDate == null || x.ActivationDate.GetValueOrDefault().Date <= HelperUtility.GetDate()) && (x.ExpirationDate == null || x.ExpirationDate.GetValueOrDefault().Date >= HelperUtility.GetDate())).ToList();

            if (HelperUtility.IsNotNull(blogNewsList))
            {
                string blogNewsCodes = string.Join(",", blogNewsList.Select(m => m.BlogNewsCode));

                List<ZnodePublishSeoEntity> blogNewsSeo = GetBlogNewsSEOSettingList(searchCmsPagesParameterModel.PortalId, blogNewsCodes);

                MapBlogNewsToSearchCMSPagesWithSEO(blogNewsList, blogNewsSeo, blognewslist, searchCmsPagesParameterModel.IndexStartTime);
            }
            
            return blognewslist;
        }

        // Get the latest version ids of portal from locale ids.
        public virtual List<ZnodePublishWebstoreEntity> GetVersionIds(int portalId, string revisionType, List<LocaleModel> localIds)
        {
            List<int> activeLocales = localIds.Select(m => m.LocaleId)?.ToList();
            List<ZnodePublishWebstoreEntity> webStoreEntitylist = _publishWebstoreEntity.Table.Where(x => x.PortalId == portalId && x.PublishState == revisionType && activeLocales.Contains(x.LocaleId))?.ToList();
            return webStoreEntitylist;
        }

        #endregion

        #region Protected Methods   

        //Get elastic CMS pages list
        protected virtual List<SearchCMSPages> GetElasticCmsPagesList(WebStoreContentPageListModel entitylist, int versionId, long indexStartTime)
        {
            if (HelperUtility.IsNotNull(entitylist.ContentPageList) && entitylist.ContentPageList.Count > 0 && versionId > 0)
            {
                List<SearchCMSPages> modellist = new List<SearchCMSPages>();

                foreach (WebStoreContentPageModel item in entitylist.ContentPageList)
                {
                    if (HelperUtility.IsNotNull(item))
                        modellist.Add(ToModel(item, versionId, indexStartTime));
                }
                return modellist;
            }
            else
                return new List<SearchCMSPages>();
        }

        //Get the ToModel of WebStoreContentPageModel
        protected virtual SearchCMSPages ToModel(WebStoreContentPageModel entity, int versionId, long indexStartTime)
        {            
            return new SearchCMSPages
            {
                versionid = versionId,
                localeid = entity.LocaleId,
                portalid = entity.PortalId,
                contentpageid = entity.ContentPageId,
                pagename = entity.PageName,
                profileid = entity.ProfileId.Length == 0 ? new int[] { 0 } : entity.ProfileId,
                isactive = true,
                pagetitle = entity.PageTitle,
                text = entity.Texts == null || entity.Texts.Length == 0 ? new string[] { "" } : entity.Texts,
                seodescription = entity.SEODescription,
                seotitle = entity.SEOTitle,
                seourl = entity.SEOUrl,
                timestamp = indexStartTime,
                didyoumean = string.Empty,
                blognewstype = string.Empty
            };
        }


        //Get Text Widget data for content page.
        protected virtual void GetContentPageData(int localeId, WebStoreContentPageListModel listModel, List<ZnodePublishContentPageConfigEntity> contentPageList, int portalId, int versionId)
        {
            if (contentPageList?.Count > 0)
            {
                List<ZnodePublishTextWidgetEntity> widgetEntity = _publishTextWidgetEntity.Table.Where(x => x.PortalId == portalId && x.VersionId == versionId && x.VersionId == versionId).ToList();
                List<ZnodePublishSeoEntity> seoEntity = _publishSEOWidgetEntity.Table.Where(x => x.LocaleId == localeId && x.PortalId == portalId && x.VersionId == versionId).ToList();


                listModel.ContentPageList = contentPageList?.ToModel<WebStoreContentPageModel>().ToList();

                if (listModel.ContentPageList?.Count > 0)
                {
                    listModel.ContentPageList.ForEach(contentPage =>
                    {
                        //Get Content page Text.
                        List<ZnodePublishTextWidgetEntity> textWidgetEntity = widgetEntity.Where(widget => widget.MappingId == contentPage.ContentPageId && widget.TypeOFMapping == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString()).ToList();

                        if (HelperUtility.IsNotNull(textWidgetEntity) && textWidgetEntity.Count > 0)
                        {
                            contentPage.Texts = textWidgetEntity.Select(x => x.Text).ToArray();
                            contentPage.TypeOFMapping = textWidgetEntity[0].TypeOFMapping;
                            contentPage.MappingId = textWidgetEntity[0].MappingId;
                            contentPage.ContentPageId = textWidgetEntity[0].MappingId;
                            contentPage.LocaleId = textWidgetEntity[0].LocaleId;
                        }

                        //Get content Page seo.
                        ZnodePublishSeoEntity seo = seoEntity?.FirstOrDefault(seoDetail => seoDetail.SEOCode == contentPage.PageName);

                        if (HelperUtility.IsNotNull(seo))
                        {
                            contentPage.SEOUrl = seo.SEOUrl;
                            contentPage.SEODescription = seo.SEODescription;
                            contentPage.SEOTitle = seo.SEOTitle;
                            contentPage.SEOKeywords = seo.SEOKeywords;
                        }
                    });
                }
    
            }
        }

        //Get content pages SEO  data.
        protected virtual List<ZnodePublishSeoEntity> GetPublishSEOSettingListForAllPages(int portalId, List<int?> localeIds, List<string> contentNames)
        {
            List<ZnodePublishSeoEntity> publishSEOList = _publishSEOWidgetEntity.Table.Where(x => localeIds.Contains(x.LocaleId) && x.PortalId == portalId && x.SEOTypeName == ZnodeConstant.ContentPage && contentNames.Contains(x.SEOCode)).ToList();
            return publishSEOList;
        }

        //Get blog news SEO  data.
        protected virtual List<ZnodePublishSeoEntity> GetBlogNewsSEOSettingList(int portalId, string blogNewsCodes, int? versionId = 0)
        {
            List<ZnodePublishSeoEntity> publishSEOList = _publishSEOWidgetEntity.Table.Where(x => x.SEOTypeName == ZnodeConstant.BlogNews  && x.PortalId == portalId && blogNewsCodes.Contains(x.SEOCode)).ToList();
            return publishSEOList;
        }

        // Maps BlogNews to searchCMSPages with SEO
        protected virtual void MapBlogNewsToSearchCMSPagesWithSEO(List<ZnodePublishBlogNewsEntity> blogNewsList, List<ZnodePublishSeoEntity> seoData, List<SearchCMSPages> blognewslistSearch, long indexStartTime)
        {
            if (HelperUtility.IsNotNull(blogNewsList))
            {
                foreach (ZnodePublishBlogNewsEntity blogNews in blogNewsList)
                {
                    SearchCMSPages blogs = new SearchCMSPages();

                    blogs.versionid = blogNews.VersionId;
                    blogs.localeid = blogNews.LocaleId;
                    blogs.portalid = blogNews.PortalId;
                    blogs.pagename = blogNews.BlogNewsTitle;
                    blogs.pagetitle = blogNews.BlogNewsTitle;
                    blogs.pagecode = blogNews.BlogNewsCode;
                    blogs.isactive = blogNews.IsBlogNewsActive;
                    blogs.profileid = new int[] { 0 };
                    blogs.text = blogNews.BlogNewsContent == null ? new string[] { "" } : new string[] { blogNews.BlogNewsContent };
                    blogs.blognewstype = blogNews.BlogNewsType;
                    blogs.blognewsid = blogNews.BlogNewsId;
                    blogs.timestamp = indexStartTime;

                    //Get blog news seo.
                    ZnodePublishSeoEntity seo = seoData?.FirstOrDefault(seoDetail => seoDetail.SEOCode == blogNews.BlogNewsCode && seoDetail.LocaleId == blogNews.LocaleId);

                    if (HelperUtility.IsNotNull(seo))
                    {
                        blogs.seourl = seo.SEOUrl;
                        blogs.seodescription = seo.SEODescription;
                        blogs.seotitle = seo.SEOTitle;
                    }
                    blognewslistSearch.Add(blogs);
                }
            }
        }

        //Get Text Widget data for content page of all locales.
        protected virtual void GetTextWidgetDataAllLocales(List<LocaleModel> localIds, WebStoreContentPageListModel listModel, List<ZnodePublishContentPageConfigEntity> contentPageList, int portalId)
        {
            if (contentPageList?.Count > 0)
            {
                List<int> activelocalIds = localIds.Select(m => m.LocaleId)?.ToList();
                List<int> contentPageIds = contentPageList.Select(m => m.ContentPageId).Distinct()?.ToList();
                List<string> contentNames = contentPageList.Select(m => m.PageName).Distinct()?.ToList();

                List<ZnodePublishTextWidgetEntity> widgetEntity =   _publishTextWidgetEntity.Table.Where(x => x.PortalId == portalId && activelocalIds.Contains(x.LocaleId) && x.TypeOFMapping == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString() && contentPageIds.Contains(x.MappingId))?.ToList();

                List<ZnodePublishSeoEntity> seoEntity = GetPublishSEOSettingListForAllPages(portalId, activelocalIds.Cast<int?>().ToList(), contentNames);

                listModel.ContentPageList = contentPageList?.ToModel<WebStoreContentPageModel>().ToList();

                GetContentPageDataForAllLocales(listModel.ContentPageList, seoEntity, widgetEntity, localIds);
            }
        }

        //Get Content page Seo details and text widget.
        protected virtual List<WebStoreContentPageModel> GetContentPageDataForAllLocales(List<WebStoreContentPageModel> contentPageList, List<ZnodePublishSeoEntity> seoEntityList, List<ZnodePublishTextWidgetEntity> widgetEntity, List<LocaleModel> localIds)
        {
            List<WebStoreContentPageModel> contentPagesOfAllLocales = new List<WebStoreContentPageModel>();
            //Assign content page seo details and text widget.
            if (contentPageList?.Count > 0)
            {
                foreach (LocaleModel locale in localIds)
                {
                    List<WebStoreContentPageModel> pageList = contentPageList.Where(m => m.LocaleId == locale.LocaleId).ToList();

                    pageList.ForEach(contentPage =>
                    {
                        //Get Content page Text.
                        List<ZnodePublishTextWidgetEntity> textWidgetEntity = widgetEntity.Where(widget => widget.MappingId == contentPage.ContentPageId && widget.TypeOFMapping == ZnodeCMSTypeofMappingEnum.ContentPageMapping.ToString() && widget.LocaleId == locale.LocaleId).ToList();

                        if (HelperUtility.IsNotNull(textWidgetEntity) && textWidgetEntity.Count > 0)
                        {
                            contentPage.Texts = textWidgetEntity.Select(x => x.Text).ToArray();
                            contentPage.TypeOFMapping = textWidgetEntity[0].TypeOFMapping;
                            contentPage.MappingId = textWidgetEntity[0].MappingId;
                            contentPage.ContentPageId = textWidgetEntity[0].MappingId;
                            contentPage.LocaleId = textWidgetEntity[0].LocaleId;
                        }

                        //Get content Page seo.
                        ZnodePublishSeoEntity seo = seoEntityList?.FirstOrDefault(seoDetail => seoDetail.SEOCode == contentPage.PageName && seoDetail.LocaleId == locale.LocaleId);

                        if (HelperUtility.IsNotNull(seo))
                        {
                            contentPage.SEOUrl = seo.SEOUrl;
                            contentPage.SEODescription = seo.SEODescription;
                            contentPage.SEOTitle = seo.SEOTitle;
                            contentPage.SEOKeywords = seo.SEOKeywords;
                        }
                    });

                    contentPagesOfAllLocales.AddRange(pageList);
                }
            }
            return contentPagesOfAllLocales;
        }

        #endregion Protected Methods
    }
}
