using System.Collections.Generic;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPublishedPortalDataService
    {
        /// <summary>
        /// gets the Webstore Entity
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="localeId">localeId</param>
        /// <param name="contentState">contentState</param>
        /// <returns>ZnodePublishWebstoreEntity</returns>
        ZnodePublishWebstoreEntity GetWebstoreEntity(int portalId, int localeId, string contentState);

        /// <summary>
        /// Gets the version entity
        /// </summary>
        /// <param name="catalogIds">catalog Ids</param>
        /// <returns>List of ZnodePublishVersionEntity<ZnodePublishVersionEntity></returns>
        List<ZnodePublishVersionEntity> GetVersionEntity(List<int> catalogIds);

        /// <summary>
        /// Gets List Of Webstore Entity
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <returns>List of ZnodePublishWebstoreEntity</returns>
        List<ZnodePublishWebstoreEntity> GetWebstoreEntity();

        /// <summary>
        /// gets the portal details
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <returns>ZnodePublishWebstoreEntity </returns>
        ZnodePublishWebstoreEntity GetWebstorePortalDetails(int portalId);


        /// <summary>
        /// gets the dynamic styles
        /// </summary>
        /// <param name="localeId">locale Id</param>
        /// <param name="portalId">portal Id</param>
        /// <returns>List<ZnodePublishPortalCustomCssEntity></returns>
        List<ZnodePublishPortalCustomCssEntity> GetDynamicStyleList(int localeId, int portalId);

        /// <summary>
        /// Gets the content Page List
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <param name="profileId">profile Id</param>
        /// <returns>List<ZnodePublishContentPageConfigEntity></returns>
        List<ZnodePublishContentPageConfigEntity> GetContentPageList(int portalId, string profileId);

        /// <summary>
        /// gets the Preview Log entity
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <returns>List<ZnodePublishPreviewLogEntity></returns>
        List<ZnodePublishPreviewLogEntity> GetPreviewLogEntity(PageListModel pageListModel);


        #region SEO 

        /// <summary>
        /// Gets the SEO setting
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>List<ZnodePublishSeoEntity></returns>
        List<ZnodePublishSeoEntity> GetSEOSettings(FilterCollection filters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seoUrl"></param>
        /// <param name="portalId"></param>
        /// <returns></returns>
        List<ZnodePublishSeoEntity> GetSEOEntity(string seoUrl, int portalId);

        /// <summary>
        /// To get the first or default seo entity based on the provided arguments.
        /// </summary>
        /// <param name="seoUrl">SEO Url</param>
        /// <param name="portalId">Portal ID</param>
        /// <param name="versionId">Version ID</param>
        /// <returns>ZnodePublishSeoEntity</returns>
        ZnodePublishSeoEntity GetSEOEntityDetails(string seoUrl, int portalId, int? versionId = null);

        /// <summary>
        /// Gets SEO Entity by SEO Code
        /// </summary>
        /// <param name="seoCode">SEO code</param>
        /// <param name="seoTypeName">SEO type Name</param>
        /// <param name="portalId">Portal ID</param>
        /// <param name="localeId">Locale ID</param>
        /// <returns></returns>
        ZnodePublishSeoEntity GetSEOEntityByCode(string seoCode, string seoTypeName, int portalId, int localeId);


        #endregion

        #region Blogs And News

        /// <summary>
        /// Gets the Blog and News Data
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <param name="localeId">locale Id</param>
        /// <param name="blogNewsType">blogNewsType</param>
        /// <param name="activationDate">activationDate</param>
        /// <returns> List<ZnodePublishBlogNewsEntity></returns>
        List<ZnodePublishBlogNewsEntity> GetBlogNewsDataList(int portalId, int localeId, string blogNewsType, string activationDate = null);

        /// <summary>
        /// Gets the Blog News Data
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <param name="localeId">locale Id</param>
        /// <param name="blogNewsId">blogNews Id</param>
        /// <param name="activationDate">activationDate date</param>
        /// <returns>ZnodePublishBlogNewsEntity</returns>
        ZnodePublishBlogNewsEntity GetBlogNewsData(int portalId, int localeId, int blogNewsId, string activationDate = null);
        #endregion

        #region Brands

        /// <summary>
        /// Gets the Brand List
        /// </summary>
        /// <param name="pageListModel">pageListModel</param>
        /// <returns> List<ZnodePublishPortalBrandEntity></returns>
        List<ZnodePublishPortalBrandEntity> GetBrandList(PageListModel pageListModel);

        /// <summary>
        /// Get Published Brand
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <param name="localeId">locale Id</param>
        /// <param name="brandId">brand Id</param>
        /// <returns>ZnodePublishPortalBrandEntity</returns>
        ZnodePublishPortalBrandEntity GetPublishedBrand(int portalId, int localeId, int brandId);
        #endregion

        #region Content Blocks

        /// <summary>
        /// Gets the message List
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <param name="localeId">locale Id</param>
        /// <returns>List<ZnodePublishMessageEntity></returns>
        List<ZnodePublishMessageEntity> GetMessageList(int portalId, int localeId);

        /// <summary>
        /// Gets the global message List
        /// </summary>
        /// <param name="localeId">locale Id</param>
        /// <returns>List<ZnodePublishGlobalMessageEntity></returns>
        List<ZnodePublishGlobalMessageEntity> GetGlobalMessageList(int localeId);
        #endregion

        #region CMS Widgets

        /// <summary>
        /// Gets the text Widget
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>ZnodePublishTextWidgetEntity</returns>
        ZnodePublishTextWidgetEntity GetTextWidget(FilterCollection filters);

        /// <summary>
        /// Get text Widget List
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <param name="localeId">locale Id</param>
        /// <returns>List<ZnodePublishTextWidgetEntity></returns>
        List<ZnodePublishTextWidgetEntity> GetTextWidgetList(int portalId, int localeId);

        /// <summary>
        /// Get Slider Banner
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>ZnodePublishWidgetSliderBannerEntity</returns>
        ZnodePublishWidgetSliderBannerEntity GetSliderBanner(FilterCollection filters);

        /// <summary>
        /// Get Media Widget
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>ZnodePublishMediaWidgetEntity</returns>
        ZnodePublishMediaWidgetEntity GetMediaWidget(FilterCollection filters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>List<ZnodePublishWidgetProductEntity></returns>
        List<ZnodePublishWidgetProductEntity> GetProductWidget(FilterCollection filters);

        /// <summary>
        /// Get category widget
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>List<ZnodePublishWidgetCategoryEntity></returns>
        List<ZnodePublishWidgetCategoryEntity> GetCategoryWidget(FilterCollection filters);

        /// <summary>
        /// Get Brand Widget
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>List of ZnodePublishWidgetBrandEntity</returns>
        List<ZnodePublishWidgetBrandEntity> GetBrandWidget(FilterCollection filters);

        /// <summary>
        /// Gets the link Widget
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>List of ZnodePublishSearchWidgetEntity</returns>
        List<ZnodePublishWidgetTitleEntity> GetLinkWidget(FilterCollection filters);

        /// <summary>
        /// Get Search Widget
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>ZnodePublishSearchWidgetEntity</returns>
        ZnodePublishSearchWidgetEntity GetSearchWidget(FilterCollection filters);

        #endregion

        /// <summary>
        /// Get Content Container
        /// </summary>
        /// <param name="filters">filters</param>
        /// <returns>ContainerKey</returns>
        string GetContentContainer(FilterCollection filters);

    }
}
