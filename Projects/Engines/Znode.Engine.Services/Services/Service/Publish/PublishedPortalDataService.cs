using System;
using System.Collections.Generic;
using System.Linq;

using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public class PublishedPortalDataService : BaseService, IPublishedPortalDataService
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodePublishSeoEntity> _publishSEOEntity;
        private readonly IZnodeRepository<ZnodePublishBlogNewsEntity> _publishBlogNewsEntity;
        private readonly IZnodeRepository<ZnodePublishTextWidgetEntity> _publishWidgetTextEntity;
        private readonly IZnodeRepository<ZnodePublishMessageEntity> _publishMessageEntity;
        private readonly IZnodeRepository<ZnodePublishGlobalMessageEntity> _publishGlobalMessageEntity;
        private readonly IZnodeRepository<ZnodePublishPortalBrandEntity> _publishPortalBrandEntity;
        private readonly IZnodeRepository<ZnodePublishWebstoreEntity> _publishWebstoreEntity;
        private readonly IZnodeRepository<ZnodePublishPortalCustomCssEntity> _publishPortalCustomCss;
        private readonly IZnodeRepository<ZnodePublishContentPageConfigEntity> _publishCMSConfigEntity;
        private readonly IZnodeRepository<ZnodePublishMediaWidgetEntity> _publishWidgetMediaEntity;
        private readonly IZnodeRepository<ZnodePublishWidgetSliderBannerEntity> _publishWidgetSliderBannerEntity;
        private readonly IZnodeRepository<ZnodePublishWidgetTitleEntity> _publishWidgetTitleEntity;
        private readonly IZnodeRepository<ZnodePublishSearchWidgetEntity> _publishWidgetSearchEntity;
        private readonly IZnodeRepository<ZnodePublishWidgetBrandEntity> _publishWidgetBrandEntity;
        private readonly IZnodeRepository<ZnodePublishWidgetCategoryEntity> _publishWidgetCategoryEntity;
        private readonly IZnodeRepository<ZnodePublishWidgetProductEntity> _publishWidgetProductEntity;
        private readonly IZnodeRepository<ZnodePublishVersionEntity> _publishVersionEntity;
        private readonly IZnodeRepository<ZnodePublishPreviewLogEntity>_publishPreviewLogEntity;
        private readonly IZnodeRepository<ZnodePublishContainerWidgetEntity> _publishContainerEntity;






        #endregion

        #region Constructor
        public PublishedPortalDataService()
        {
            _publishSEOEntity = new ZnodeRepository<ZnodePublishSeoEntity>(HelperMethods.Context);
            _publishBlogNewsEntity = new ZnodeRepository<ZnodePublishBlogNewsEntity>(HelperMethods.Context);
            _publishWidgetTextEntity = new ZnodeRepository<ZnodePublishTextWidgetEntity>(HelperMethods.Context);
            _publishMessageEntity = new ZnodeRepository<ZnodePublishMessageEntity>(HelperMethods.Context);
            _publishGlobalMessageEntity = new ZnodeRepository<ZnodePublishGlobalMessageEntity>(HelperMethods.Context);
            _publishPortalBrandEntity = new ZnodeRepository<ZnodePublishPortalBrandEntity>(HelperMethods.Context);
            _publishWebstoreEntity = new ZnodeRepository<ZnodePublishWebstoreEntity>(HelperMethods.Context);
            _publishPortalCustomCss = new ZnodeRepository<ZnodePublishPortalCustomCssEntity>(HelperMethods.Context);
            _publishCMSConfigEntity = new ZnodeRepository<ZnodePublishContentPageConfigEntity>(HelperMethods.Context);
            _publishWidgetMediaEntity = new ZnodeRepository<ZnodePublishMediaWidgetEntity>(HelperMethods.Context);
            _publishWidgetSliderBannerEntity = new ZnodeRepository<ZnodePublishWidgetSliderBannerEntity>(HelperMethods.Context);
            _publishWidgetTitleEntity = new ZnodeRepository<ZnodePublishWidgetTitleEntity>(HelperMethods.Context);
            _publishWidgetSearchEntity = new ZnodeRepository<ZnodePublishSearchWidgetEntity>(HelperMethods.Context);
            _publishWidgetBrandEntity = new ZnodeRepository<ZnodePublishWidgetBrandEntity>(HelperMethods.Context);
            _publishWidgetCategoryEntity = new ZnodeRepository<ZnodePublishWidgetCategoryEntity>(HelperMethods.Context);
            _publishWidgetProductEntity = new ZnodeRepository<ZnodePublishWidgetProductEntity>(HelperMethods.Context);
            _publishVersionEntity = new ZnodeRepository<ZnodePublishVersionEntity>(HelperMethods.Context);
            _publishPreviewLogEntity = new ZnodeRepository<ZnodePublishPreviewLogEntity>(HelperMethods.Context);
            _publishContainerEntity = new ZnodeRepository<ZnodePublishContainerWidgetEntity>(HelperMethods.Context);


        }
        #endregion


        //Gets Webstore Entity Details
        public virtual ZnodePublishWebstoreEntity GetWebstoreEntity(int portalId, int localeId, string contentState)
             => _publishWebstoreEntity.Table.FirstOrDefault(x => x.PortalId == portalId && x.LocaleId == localeId && x.PublishState == contentState.ToString());

        //Gets the Version Entity Details
        public virtual List<ZnodePublishVersionEntity> GetVersionEntity(List<int> catalogIds)
             => _publishVersionEntity.Table.Where(x => catalogIds.Contains(x.ZnodeCatalogId)).ToList();


        //Gets Webstore Portal Details
        public virtual ZnodePublishWebstoreEntity GetWebstorePortalDetails(int portalId)
             => _publishWebstoreEntity.Table.FirstOrDefault(x => x.PortalId == portalId );

        //Gets Webstore Portal Details
        public virtual List<ZnodePublishWebstoreEntity> GetWebstoreEntity()
             => _publishWebstoreEntity.Table.ToList();

        //Gets dynamic styles
        public virtual List<ZnodePublishPortalCustomCssEntity> GetDynamicStyleList(int localeId, int portalId)
          => _publishPortalCustomCss.Table.Where(x => x.LocaleId == localeId && x.PortalId == portalId && x.VersionId == WebstoreVersionId)?.ToList();

        //Fetch the list of content pages associated
        public virtual List<ZnodePublishContentPageConfigEntity> GetContentPageList(int portalId, string profileId)
            => _publishCMSConfigEntity.Table.Where(x => x.PortalId == portalId && x.IsActive && (x.ProfileId.Contains(profileId) || string.IsNullOrEmpty(x.ProfileId)) && x.VersionId == WebstoreVersionId)?.ToList();

        public virtual List<ZnodePublishPreviewLogEntity> GetPreviewLogEntity(PageListModel pageListModel)
          =>  _publishPreviewLogEntity.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount).ToList();



        #region SEO 
        //Get SEO Setting details based on the filters
        public virtual List<ZnodePublishSeoEntity> GetSEOSettings(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _publishSEOEntity.GetEntityListWithoutOrderBy(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();
        }

        //Gets the List of SEO Entity
        public virtual ZnodePublishSeoEntity GetSEOEntityByCode(string seoCode, string seoTypeName, int portalId, int localeId)
        {
            int? versionId = GetCatalogVersionId();
            return _publishSEOEntity.Table.FirstOrDefault(x => x.SEOTypeName == seoTypeName && x.SEOCode == seoCode && x.LocaleId == localeId && x.PortalId == portalId && x.VersionId == versionId);
        }
        //Gets the SEO Entity
        public virtual List<ZnodePublishSeoEntity> GetSEOEntity(string seoUrl, int portalId)
            => _publishSEOEntity.Table.Where(x => x.SEOUrl == seoUrl && x.PortalId == portalId)?.ToList();

        // To get the first or default seo entity based on the provided arguments.
        public virtual ZnodePublishSeoEntity GetSEOEntityDetails (string seoUrl, int portalId, int? versionId = null)
        {
            if (HelperUtility.IsNotNull(versionId) && versionId > 0)
            {
                return _publishSEOEntity.Table.FirstOrDefault(x => x.VersionId == versionId && x.SEOUrl == seoUrl && x.PortalId == portalId);
            }

            return _publishSEOEntity.Table.FirstOrDefault(x => x.SEOUrl == seoUrl && x.PortalId == portalId);
        }
        #endregion

        #region Blog News
        //Get List of Blog news data based on parameters 
        public virtual List<ZnodePublishBlogNewsEntity> GetBlogNewsDataList(int portalId, int localeId, string blogNewsType, string activationDate = null)
        {
            if (!String.IsNullOrEmpty(activationDate))
                return _publishBlogNewsEntity.Table.Where(x => x.PortalId == portalId && x.LocaleId == localeId && x.BlogNewsType == blogNewsType && x.VersionId == WebstoreVersionId && (x.ActivationDate == null || x.ActivationDate <= DateTime.UtcNow) && (x.ExpirationDate == null || x.ExpirationDate >= DateTime.UtcNow))?.ToList();
            else
                return _publishBlogNewsEntity.Table.Where(x => x.PortalId == portalId && x.LocaleId == localeId && x.BlogNewsType == blogNewsType && x.VersionId == WebstoreVersionId)?.ToList();
        }
          


        //Get Blogs News Entity based on parameters
        public virtual ZnodePublishBlogNewsEntity GetBlogNewsData(int portalId, int localeId, int blogNewsId, string activationDate = null)
        {
            if (!String.IsNullOrEmpty(activationDate))
                return _publishBlogNewsEntity.Table.FirstOrDefault(x => x.PortalId == portalId && x.LocaleId == localeId && x.BlogNewsId == blogNewsId && x.VersionId == WebstoreVersionId && (x.ActivationDate == null || x.ActivationDate <= DateTime.UtcNow) && (x.ExpirationDate == null || x.ExpirationDate >= DateTime.UtcNow));
            else
                return _publishBlogNewsEntity.Table.FirstOrDefault(x => x.PortalId == portalId && x.LocaleId == localeId && x.BlogNewsId == blogNewsId && x.VersionId == WebstoreVersionId);

        }

        #endregion

        #region Content Block

        public virtual List<ZnodePublishMessageEntity> GetMessageList(int portalId, int localeId)
          => _publishMessageEntity.Table.Where(x => x.PortalId == portalId && x.LocaleId == localeId && x.VersionId == WebstoreVersionId)?.ToList();

        public virtual List<ZnodePublishGlobalMessageEntity> GetGlobalMessageList(int localeId)
          => _publishGlobalMessageEntity.Table.Where(x => x.LocaleId == localeId && x.VersionId == GlobalVersionId)?.ToList();


        #endregion

        #region Brands

        //Get published Brands based on filters
        public virtual List<ZnodePublishPortalBrandEntity> GetBrandList(PageListModel pageListModel)
         => _publishPortalBrandEntity.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues)?.ToList();

        //Get Published Brand Data
        public virtual ZnodePublishPortalBrandEntity GetPublishedBrand(int portalId, int localeId, int brandId)
         => _publishPortalBrandEntity.Table.FirstOrDefault(x => x.PortalId == portalId && x.BrandId == brandId && x.LocaleId == localeId && x.VersionId == WebstoreVersionId && x.IsActive);

        #endregion

        #region CMS Widgets

        #region Text Widgets

        //Get text widget based on the the filters
        public virtual ZnodePublishTextWidgetEntity GetTextWidget(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _publishWidgetTextEntity.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
        }

        //Get text widget list based on parameters
        public virtual List<ZnodePublishTextWidgetEntity> GetTextWidgetList(int portalId, int localeId)
            => _publishWidgetTextEntity.Table.Where(x => x.PortalId == portalId && x.LocaleId == localeId && x.VersionId == WebstoreVersionId)?.ToList();

        #endregion

        #region Slider Banner Widget

        //Get Slider Banner Data base on filters
        public virtual ZnodePublishWidgetSliderBannerEntity GetSliderBanner(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _publishWidgetSliderBannerEntity.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
        }
        #endregion

        #region Media Widget

        //Get Media widget data based on filters
        public virtual ZnodePublishMediaWidgetEntity GetMediaWidget(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _publishWidgetMediaEntity.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
        }

        #endregion

        #region Product Widget

        //Get Product Widget list based on filters
        public virtual List<ZnodePublishWidgetProductEntity> GetProductWidget(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _publishWidgetProductEntity.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();
        }
        #endregion

        #region Category Widget

        //Get Category Widget list based on filters
        public virtual List<ZnodePublishWidgetCategoryEntity> GetCategoryWidget(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _publishWidgetCategoryEntity.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();
        }
        #endregion

        #region Link widget

        //Get Link Widget list based on filters
        public virtual List<ZnodePublishWidgetTitleEntity> GetLinkWidget(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _publishWidgetTitleEntity.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();
        }
        #endregion

        #region Brand Widget

        //Get Brand Widget list based on filters
        public virtual List<ZnodePublishWidgetBrandEntity> GetBrandWidget(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _publishWidgetBrandEntity.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();
        }

        #endregion

        #region Search Widget
        public virtual ZnodePublishSearchWidgetEntity GetSearchWidget(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _publishWidgetSearchEntity.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);
        }

        #endregion

        #region Container Widget

        //Get Content Container Data base on filters
        public virtual string GetContentContainer(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            return _publishContainerEntity.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ContainerKey;
        }
        #endregion
        #endregion

    }
}
