using System;

namespace Znode.Engine.Services
{
    public interface IPublishPortalDataService
    {
        /// <summary>
        /// Publish Portal
        /// </summary>
        /// <param name="portalId">Portal Id to be published</param>
        /// <param name="targetPublishState">Revision type</param>
        /// <param name="publishContent">Publish CMS content or store Setting</param>
        /// <param name="takeFromDraftFirst">takeFromDraftFirst</param>
        /// <returns>Publish status</returns>
        bool PublishPortal(int portalId, Guid jobId, string targetPublishState = null, string publishContent = null, bool takeFromDraftFirst = false);

        /// <summary>
        /// Publish Content Page
        /// </summary>
        /// <param name="contentPageId">contentPageId to be published</param>
        /// <param name="portalId">Portal associated with the content Page</param>
        /// <param name="localeId">LocaleId</param>
        /// <param name="targetPublishState">Revision type</param>
        /// <param name="takeFromDraftFirst">takeFromDraftFirst</param>
        /// <returns>Publish status</returns>
        bool PublishContentPage(int contentPageId, int portalId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false);

        /// <summary>
        /// Publish Slider banner
        /// </summary>
        /// <param name="cmsPortalSliderId">cmsPortalSliderId to be published</param>
        /// <param name="portalId">Portal associated with the slider Banner</param>
        /// <param name="localeId">LocaleId</param>
        /// <param name="targetPublishState">Revision type</param>
        /// <param name="takeFromDraftFirst">takeFromDraftFirst</param>
        /// <returns>Publish status</returns>
        bool PublishSlider(string cmsPortalSliderId, int portalId, int localeId, string targetPublishState = null, bool takeFromDraftFirst = false);

        /// <summary>
        /// Publish Content Block
        /// </summary>
        /// <param name="cmsMessageKeyId">cmsMessageKeyId to be published</param>
        /// <param name="portalId">Portal associated with the slider Banner</param>
        /// <param name="localeId">LocaleId</param>
        /// <param name="targetPublishState">Revision type</param>
        /// <param name="takeFromDraftFirst">takeFromDraftFirst</param>
        /// <returns>Publish status</returns>
        bool PublishManageMessage(string cmsMessageKeyId, int portalId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false);

        /// <summary>
        /// Publish SEO
        /// </summary>
        /// <param name="seoCode">SEO Code</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="targetPublishState">Revision Type</param>
        /// <returns>Publish Status</returns>
        bool PublishSEO(string seoCode, int portalId, int localeId, int seoTypeId, string targetPublishState);

        /// <summary>
        /// Publish Blog/News
        /// </summary>
        /// <param name="blogNewsCode">BlogNewsCode to be published</param>
        /// <param name="blogNewsType">BlogNewsType type of page</param>
        /// <param name="portalId">Portal associated with the content Page</param>
        /// <param name="localeId">LocaleId</param>
        /// <param name="targetPublishState">Revision type</param>
        /// <param name="takeFromDraftFirst">takeFromDraftFirst</param>
        /// <returns>Publish status</returns>
        bool PublishBlogNews(string blogNewsCode, string blogNewsType, int portalId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false);

    }
}
