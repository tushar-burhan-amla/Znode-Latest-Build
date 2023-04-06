using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IWebSiteService
    {
        /// <summary>
        /// Get the Portal List, for which the Themes are assigned.
        /// </summary>
        /// <param name="filters">List of filter tuples</param>
        /// <param name="sorts">Collection of sorting parameters</param>
        /// <param name="page">Collection of paging parameters</param>
        /// <returns>Return the List of Portals.</returns>
        PortalListModel GetPortalList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Web Site Logo Details by Portal Id.
        /// </summary>
        /// <param name="portalId">Id for the Portal</param>
        /// <returns>Return Portal Logo Details in WebSiteLogoModel Format.</returns>
        WebSiteLogoModel GetWebSiteLogoDetails(int portalId);

        /// <summary>
        /// Save the WebSite Logo Details.
        /// </summary>
        /// <param name="model">Model of type WebSiteLogoModel</param>
        /// <returns>return true or false</returns>
        bool SaveWebSiteLogo(WebSiteLogoModel model);


        /// <summary>
        /// Publish CMS configuration
        /// </summary>
        /// <param name="portalId">Store to publish.</param>
        /// <param name="targetPublishState">This parameter specifies the target state for the publish to be performed for. If not supplied, will fall back to the default state as target.</param>
        /// <param name="publishContent">A comma separated string to specify the type of content to be included in the publish process. If not supplied, will fall back to publishing only the store settings.</param>
        /// <param name="takeFromDraftFirst">Pass true, if the process has to first take the drafted data from SQL in all cases. If not supplied, it will fall back to not take drafted data before publish.</param>
        /// <returns></returns>
        bool Publish(int portalId, string targetPublishState = null, string publishContent = null, bool takeFromDraftFirst = false);

        /// <summary>
        /// Publish CMS configuration asynchronously.
        /// </summary>
        /// <param name="portalId">Store to publish.</param>
        /// <param name="targetPublishState">This parameter specifies the target state for the publish to be performed for. If not supplied, will fall back to the default state as target.</param>
        /// <param name="publishContent">A comma separated string to specify the type of content to be included in the publish process. If not supplied, will fall back to publishing only the store settings.</param>
        /// <param name="takeFromDraftFirst">Pass true, if the process has to first take the drafted data from SQL in all cases. If not supplied, it will fall back to not take drafted data before publish.</param>
        /// <returns></returns>
        bool PublishAsync(int portalId, string targetPublishState = null, string publishContent = null, bool takeFromDraftFirst = false);

        /// <summary>
        /// Get pim catalog id associated to the supplied portal id.
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        int GetAssociatedCatalogId(int portalId);

        #region Portal Product Page
        /// <summary>
        /// Get the list of portal page product associated to selected store in website configuration.
        /// </summary>
        /// <param name="portalId">Id of store.</param>
        /// <returns>List of portal page product.</returns>
        PortalProductPageModel GetPortalProductPageList(int portalId);

        /// <summary>
        /// Assign new pdp template to product type.
        /// </summary>
        /// <param name="portalProductPageModel">PortalProductPageModel</param>
        /// <returns>return true if updated.</returns>
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
