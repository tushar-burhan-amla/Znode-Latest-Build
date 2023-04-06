using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ISEOSettingClient : IBaseClient
    {
        /// <summary>
        /// Gets portal SEO settings.
        /// </summary>
        /// <param name="portalId">Portal ID to et default SEO settings.</param>
        /// <returns>Portal SEO Setting Model.</returns>
        PortalSEOSettingModel GetPortalSEOSetting(int portalId);

        /// <summary>
        /// Create portal SEO setting.
        /// </summary>
        /// <param name="model">Portal SEO setting model.</param>
        /// <returns>Newly created portal SEO Setting.</returns>
        PortalSEOSettingModel CreatePortalSEOSetting(PortalSEOSettingModel model);

        /// <summary>
        /// Updates a portal SEO setting.
        /// </summary>
        /// <param name="model">Portal SEO setting model to be updated.</param>
        /// <returns>Updated Portal SEO setting model.</returns>
        PortalSEOSettingModel UpdatePortalSEOSetting(PortalSEOSettingModel model);

        /// <summary>
        /// Creates SEO details for product/category/content-page.
        /// </summary>
        /// <param name="model">Model of SEO details to be created.</param>
        /// <returns>Newly created SEO details.</returns>
        SEODetailsModel CreateSEODetails(SEODetailsModel model);

        /// <summary>
        /// Updates SEO details for product/category/content-page. 
        /// </summary>
        /// <param name="model">Model of SEO details to be updated.</param>
        /// <returns>Updated SEO details.</returns>
        SEODetailsModel UpdateSEODetails(SEODetailsModel model);

        /// <summary>
        /// Gets SEO details for the specified Item by Item ID.
        /// </summary>
        /// <param name="itemId">ID of Product/Category/Content page.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>SEO details model.</returns>
        SEODetailsModel GetSEODetailId(int? itemId, int seoTypeId, int localeId, int portalId);

        /// <summary>
        ///Gets the list of seo details.
        /// </summary>
        /// <param name="expands"> Expands for seo details list.</param>
        /// <param name="filters">Filters for seo details list.</param>
        /// <param name="sorts">Sorts for seo details list.</param>
        /// <returns>seo details list model.</returns>
        SEODetailsListModel GetSeoDetails(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        ///Gets the list of seo details.
        /// </summary>
        /// <param name="expands"> Expands for seo details list.</param>
        /// <param name="filters">Filters for seo details list.</param>
        /// <param name="sorts">Sorts for seo details list.</param>
        /// <param name="pageIndex">Start page index for seo details list.</param>
        /// <param name="pageSize">Page size of seo details list.</param>
        /// <returns>seo details list model.</returns>
        SEODetailsListModel GetSeoDetails(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Publish the seo details
        /// </summary>
        /// <param name="itemId">ID of Product/Category/Content page</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>Publish status for seo</returns>
        PublishedModel Publish(string seoCode, int portalId, int localeId, int seoTypeId);

        /// <summary>
        /// Publish the seo details.
        /// </summary>
        /// <param name="seoCode"></param>
        /// <param name="portalId"></param>
        /// <param name="localeId"></param>
        /// <param name="seoTypeId"></param>
        /// <param name="targetPublishState"></param>
        /// <param name="takeFromDraftFirst"></param>
        /// <returns></returns>
        PublishedModel Publish(string seoCode, int portalId, int localeId, int seoTypeId, string targetPublishState = null, bool takeFromDraftFirst = false);

        /// <summary>
        /// Gets SEO details for the specified Item by Item ID.
        /// </summary>
        /// <param name="itemId">ID of Product/Category/Content page.</param>
        /// <param name="seoType">SEO Type</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>SEO details model.</returns>
        SEODetailsModel GetPublishSEODetail(int itemId, string seoType, int localeId, int portalId, string seoCode);

        /// <summary>
        /// Gets SEO details for the specified Item by Item ID.
        /// </summary>
        /// <param name="seoCode">seoCode of Product/Category/Content page.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <returns>SEO details model.</returns>
        SEODetailsModel GetSEODetailsBySEOCode(string seoCode, int seoTypeId, int localeId, int portalId);

        /// <summary>
        ///  Get Product List for SEO
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Publish Product List Model</returns>
        PublishProductListModel GetProductsForSEO(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets SEO details for the specified Item by Item ID.
        /// </summary>
        /// <param name="seoCode">seoCode of Product/Category/Content page.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="itemId">Item Id</param>
        /// <returns>SEO details model.</returns>
        SEODetailsModel GetDefaultSEODetails(string seoCode, int seoTypeId, int localeId, int portalId, int itemId);

        /// <summary>
        /// Delete existing Seo
        /// </summary>
        /// <param name="seoTypeId">Seo Type Id</param>
        /// <param name="portalid">portal Id</param>
        /// <param name="seoCode">Seo Code</param>
        /// <returns>true or false</returns>
        bool DeleteSeo(int seoTypeId, int portalId, string seoCode = "");

    }
}
