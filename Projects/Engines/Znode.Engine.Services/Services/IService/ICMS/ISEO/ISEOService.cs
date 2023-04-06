using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ISEOService
    {
        /// <summary>
        /// Gets portal SEO setting for the specified Portal ID.
        /// </summary>
        /// <param name="portalId">Portal ID for which SEO setting needs to be retrieved.</param>
        /// <returns>Portal SEO setting Model.</returns>
        PortalSEOSettingModel GetPortalSEOSetting(int portalId);

        /// <summary>
        /// Creates a new Portal SEO Setting.
        /// </summary>
        /// <param name="model">Portal SEO Setting to be inserted.</param>
        /// <returns>Newly created Portal SEO settings.</returns>
        PortalSEOSettingModel CreatePortalSEOSetting(PortalSEOSettingModel model);

        /// <summary>
        /// Updates Portal SEO setting.
        /// </summary>
        /// <param name="model">Model to be updated.</param>
        /// <returns>Portal SEO setting model to be updated.</returns>
        bool UpdatePortalSEOSetting(PortalSEOSettingModel model);

        /// <summary>
        /// Gets SEO details according to ID.
        /// </summary>
        /// <param name="itemId">SEO details ID to get SEO details.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <returns>SEO details model.</returns>
        SEODetailsModel GetSEODetails(int itemId, int seoTypeId, int localeId, int portalId);

        /// <summary>
        /// Get product SEO details.
        /// </summary>
        /// <param name="localeId">Locale Id.</param>
        /// <param name="seoId">SEO Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="seoTypeName">SEO Type Name</param>
        /// <param name="seoCode">SEO Code</param>
        /// <returns>SEO details model.</returns>
        SEODetailsModel GetProductSeoData(int localeId, int seoId, int portalId, string seoTypeName, string seoCode);

        /// <summary>
        /// Gets SEO details according to ID.
        /// </summary>
        /// <param name="seoCode">SeoCode to get SEO details.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <returns>SEO details model.</returns>
        SEODetailsModel GetSEODetailsBySEOCode(string seoCode, int seoTypeId, int localeId, int portalId);

        /// <summary>
        /// Gets SEO details according to ID.
        /// </summary>
        /// <param name="seoCode">SeoCode to get SEO details.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="itemId">Item Id</param>
        /// <returns>SEO details model.</returns>
        SEODetailsModel GetDefaultSEODetails(string seoCode, int seoTypeId, int localeId, int portalId, int itemId);

        /// <summary>
        /// Creates SEO details for product/category/content-pages.
        /// </summary>
        /// <param name="model">SEO details model to be created.</param>
        /// <returns>Newly created seo detail model.</returns>
        SEODetailsModel CreateSEODetails(SEODetailsModel model);

        /// <summary>
        /// Gets SEO details according to ID.
        /// </summary>
        /// <param name="itemId">SEO details ID to get SEO details.</param>
        /// <param name="seoType">SEO Type</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <returns>SEO details model.</returns>
        SEODetailsModel GetPublishSEODetails(int seoDetailId, string seoType, int localeId, int portalId, string seoCode);

        /// <summary>
        /// Updates SEO details.
        /// </summary>
        /// <param name="model">SEO details to be updated.</param>
        /// <returns>True if SEO details model is updated;False if SEO details model fails to update.</returns>
        bool UpdateSEODetails(SEODetailsModel model);

        /// <summary>
        /// Get seo details list.
        /// </summary>
        /// <param name="expands"> Expands for seo details list.</param>
        /// <param name="filters">Filters for seo details list.</param>
        /// <param name="sorts">Sorts for seo details list.</param>
        /// <param name="page">Paging information about seo details list.</param>
        /// <returns>Seo details list model.</returns>
        SEODetailsListModel GetSEODetailsList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Category for SEO
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>Publish Category List Model</returns>
        PublishCategoryListModel GetCategoryListForSEO(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Product List for SEO
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>Publish Product List Model</returns>
        PublishProductListModel GetProductsForSEO(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Publish the seo details
        /// </summary>
        /// <param name="seoCode"></param>
        /// <param name="portaId"></param>
        /// <param name="localeId"></param>
        /// <param name="seoTypeId"></param>
        /// <param name="targetPublishState"></param>
        /// <param name="takeFromDraftFirst"></param>
        /// <returns></returns>
        PublishedModel Publish(string seoCode, int portalId, int localeId, int seoTypeId, string targetPublishState, bool takeFromDraftFirst);

        /// <summary>
        /// Delete Seo Detail by using seoTypeId, portalId and seoCode.
        /// </summary>
        /// <param name="seoTypeId">seoType Id</param>
        /// <param name="portalId">portal Id</param>
        /// /// <param name="seoCode">seoCode</param>
        /// <returns>Returns true if record deleted successfully, else false.</returns>.
        bool DeleteSeo(int seoTypeId, int portalId, string seoCode);

        #region Webstore 

        /// <summary>
        /// Get Portal Seo Default Setting.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <returns>Returns ZnodeCMSPortalSEOSetting.</returns>
        ZnodeCMSPortalSEOSetting GetPortalSeoDefaultSetting(string portalId);

        /// <summary>
        /// Get Seo Setting.
        /// </summary>
        /// <param name="seoTypeName">SEO Type Name.</param>
        /// <param name="portalId">Portal Id.</param>
        /// <returns>Returns List<ZnodeCMSSEODetail>.</returns>
        List<SEODetailsModel> GetSeoSettingList(string seoTypeName, int portalId, int localeId);

        /// <summary>
        /// Get Seo Setting.
        /// </summary>
        /// <param name="seoTypeName">SEO Type Name.</param>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="localeId">locale Id.</param>
        /// <returns>Returns List ZnodeCMSSEODetail.</returns>
        SEODetailsModel GetSeoSetting(int seoId,string seoTypeName, int portalId, int localeId);

        /// <summary>
        /// Get Seo Setting.
        /// </summary>
        /// <param name="seoTypeName">SEO Type Name.</param>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="localeId">locale Id.</param>
        /// <returns>Returns ZnodePublishSeoEntity.</returns>
        ZnodePublishSeoEntity GetPublishSeoSetting(string seoCode, string seoTypeName, int portalId, int localeId);

        /// <summary>
        /// Get Seo Setting.
        /// </summary>
        /// <param name="seoTypeName">SEO Type Name.</param>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="portalId">Version Id of Catalog.</param>
        /// <returns>Returns List of ZnodePublishSeoEntity</returns>
        List<ZnodePublishSeoEntity> GetPublishSEOSettingList(string seoTypeName, int? portalId, int localeId = 0,int? versionId = 0);

        /// <summary>
        /// Get Seo details
        /// </summary>
        /// <param name="portalId">portal id</param>
        /// <param name="seoType">seo type</param>
        /// <returns>list of seo details</returns>
        List<ZnodeCMSSEODetail> GetSEODetailsList(int portalId, string seoType);
        #endregion
    }
}
