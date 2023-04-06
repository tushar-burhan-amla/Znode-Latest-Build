
namespace Znode.Engine.Api.Cache
{
    public interface ISEOCache
    {
        /// <summary>
        ///  Get Portal SEO Setting.
        /// </summary>
        /// <param name="portalId">Portal ID.</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetPortalSEOSetting(int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets SEO details for SEO detail ID.
        /// </summary>
        /// <param name="seoDetailId">SEO detail ID.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>String data.</returns>
        string GetSEODetails(int seoDetailId, int seoTypeId, int localeId, int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets SEO details for SEO detail ID.
        /// </summary>
        /// <param name="seoCode">seoCode.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>String data.</returns>
        string GetSEODetailsBySEOCode(string seoCode, int seoTypeId, int localeId, int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get seo details.
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>return seo details.</returns>
        string GetSeoDetailsList(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets SEO details for SEO detail ID.
        /// </summary>
        /// <param name="seoDetailId">SEO detail ID.</param>
        /// <param name="seoType">SEO Type</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="seoCode"></param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>String data.</returns>
        string GetPublishSEODetails(int seoDetailId, string seoType, int localeId, int portalId,string seoCode, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Categories for SEO.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetCategoryListForSEO(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Products list for SEO
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns></returns>
        string GetProductsForSEO(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets SEO details for SEO detail ID.
        /// </summary>
        /// <param name="seoCode">seoCode.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="itemId">Item Id</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>String data.</returns>
        string GetDefaultSEODetails(string seoCode, int seoTypeId, int localeId, int portalId, int itemId,string routeUri, string routeTemplate);

    }
}
