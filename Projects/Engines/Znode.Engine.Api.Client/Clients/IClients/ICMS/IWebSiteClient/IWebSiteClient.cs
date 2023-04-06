using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IWebSiteClient : IBaseClient
    {
        /// <summary>
        /// Get the Theme Associated Portals.
        /// </summary>
        /// <param name="filters">Filter Collection for Portal List associated to Website Configuration.</param>
        /// <param name="sortCollection">sorts for Portal List associated to Website Configuration.</param>
        /// <param name="pageIndex">Index of requesting page</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>Return the Portal in PortalListModel Format.</returns>
        PortalListModel GetPortalList(FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? recordPerPage);

        /// <summary>
        /// Get WebSite Logo Details by portalId
        /// </summary>
        /// <param name="portalId">portalId to get WebSite Logo Details</param>
        /// <returns>WebSiteLogo  Model</returns>
        WebSiteLogoModel GetWebSiteLogo(int portalId);

        /// <summary>
        /// Save the WebSite Logo Details
        /// </summary>
        /// <param name="model">Model of type WebSiteLogoViewModel</param>
        /// <returns>return model in WebSiteLogoModel Format. </returns>
        WebSiteLogoModel SaveWebSiteLogo(WebSiteLogoModel companyModel);

        /// <summary>
        /// Publish CMS configuration
        /// </summary>
        /// <param name="portalId">PortalId</param>
        /// <param name="targetPublishState">Target publish state.</param>
        /// <param name="publishContent">Content type to publish.</param>
        /// <returns>Return True/False</returns>
        bool Publish(int portalId, string targetPublishState = null, string publishContent = null);

        /// <summary>
        /// Get pim catalog id associated to supplied portal id.
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        string GetAssociatedCatalogId(int portalId);

        #region Portal Product Page
        /// <summary>
        /// Get the list of portal page product associated to selected store in website configuration.
        /// </summary>
        /// <param name="portalId">Id of portal./param>
        /// <returns>List of Portal Product Page.</returns>
        PortalProductPageModel GetPortalProductPageList(int portalId);

        /// <summary>
        /// Assign new pdp template to product type.
        /// </summary>
        /// <param name="portalProductPageModel">PortalProductPageModel</param>
        /// <returns>Return true if updated else false.</returns>
        bool UpdatePortalProductPage(PortalProductPageModel portalProductPageModel);

        /// <summary>
        /// Get the widget id by its code.
        /// </summary>
        /// <param name="widgetCode">Widget Code.</param>
        /// <returns>Returns Widget Id.</returns>
        int GetWidgetIdByCode(string widgetCode);
        #endregion
    }
}
