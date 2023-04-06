using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface IPortalCache
    {
        /// <summary>
        /// Get the list of all portals.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of portal in string format by serializing it.</returns>
        string GetPortalList(string routeUri, string routeTemplate);

#if DEBUG
        /// <summary>
        /// Get the list of all portals.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of portal in string format by serializing it.</returns>
        string GetDevPortalList(string routeUri, string routeTemplate);

#endif

        /// <summary>
        /// Get the details of portal by portal Id..
        /// </summary>
        /// <param name="portalId">Id of portal to get portal details.</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>portal model in string format by serializing it.</returns>
        string GetPortal(int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the details of portal by Store Code.
        /// </summary>
        /// <param name="storeCode">storeCode to get portal details</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>portal model in string format by serializing it.</returns>
        string GetPortal(string storeCode, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of features to create/update portal.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of portal features in string format by serializing it.</returns>
        string GetPortalFeatures(string routeUri, string routeTemplate);

        /// <summary>
        /// Get all portals on Catalog Id
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        string GetPortalListByCatalogId(int catalogId, string routeUri, string routeTemplate);


        /// <summary>
        /// Get associated warehouse as per selected portal Id.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of portal features in string format by serializing it.</returns>
        string GetAssociatedWarehouseList(int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Active Locale list
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>return Locale List</returns>
        string LocaleList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get portal shipping on the basis of portalId.
        /// </summary>
        /// <param name="portalId">portalId.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns portal shipping data.</returns>
        string GetPortalShippingInformation(int portalId, string routeUri, string routeTemplate);

        /// <summary>
        ///Get portal publish status.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Publish portal Log List Model</returns>
        PublishPortalLogListModel GetPortalPublishStatus(string routeUri, string routeTemplate);

        /// <summary>
        /// Get portal tax information.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns tax information against a portal.</returns>
        string GetTaxPortalInformation(int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Portal Tracking Pixel by portal Id..
        /// </summary>
        /// <param name="portalId">Id of portal to get Tracking Pixel.</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>portal tracking pixel model in string format by serializing it.</returns>
        string GetPortalTrackingPixel(int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get robots.txt data. 
        /// </summary>
        /// <param name="portalId">PortalId to get robots.txt data.</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>Model in string format.</returns>
        string GetRobotsTxt(int portalId, string routeUri, string routeTemplate);


        /// <summary>
        /// Get Details of Portal Approval.
        /// </summary>
        /// <param name="portalId">Portal Id to get Approval Management Details</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>Model in string format</returns>
        string GetPortalApprovalDetails(int portalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Details of Portal Approver List.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>Model in string format</returns>
        string GetPortalApproverList(int portalApprovalId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Portal Approval type.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetPortalApprovalTypeList(string routeUri, string routeTemplate);

        /// <summary>
        /// GetGet Portal Approval level.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetPortalApprovalLevelList(string routeUri, string routeTemplate);


        /// <summary>
        /// Get Sort Setting list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns GiftCard list.</returns>
        string GetPortalSortSettings(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Page Setting list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns GiftCard list.</returns>
        string GetPortalPageSettings(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the barcode setting specific to barcode.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of portal in string format by serializing it.</returns>
        string GetBarcodeScanner(string routeUri, string routeTemplate);
    }
}
