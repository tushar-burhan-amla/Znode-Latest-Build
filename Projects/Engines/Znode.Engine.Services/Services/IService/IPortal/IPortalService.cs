using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPortalService
    {
        /// <summary>
        /// Get the list of all portals.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>PortalsList Model.</returns>
        PortalListModel GetPortalList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get portal details by portal Id.
        /// </summary>
        /// <param name="portalId">Id of portal to get portal details.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Portal model.</returns>
        PortalModel GetPortal(int portalId, NameValueCollection expands);

        /// <summary>
        /// Get all portals by CatalogId
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        PortalListModel GetPortalListByCatalogId(int catalogId);

#if DEBUG
        /// <summary>
        /// Get the list of all portals.
        /// </summary> 
        /// <returns>PortalsList Model.</returns>
        PortalListModel GetDevPortalList();
#endif

        /// <summary>
        /// Get portal details by store Code
        /// </summary>
        /// <param name="storeCode">StoreCode of portal to get portal details.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Portal model.</returns>
        PortalModel GetPortal(string storeCode, NameValueCollection expands);

        /// <summary>
        /// Create new portal.
        /// </summary>
        /// <param name="portalModel">Portal model</param>
        /// <returns>Newly created portal.</returns>
        PortalModel CreatePortal(PortalModel portalModel);

        /// <summary>
        /// Update portal.
        /// </summary>
        /// <param name="portalModel">Portal Model.</param>
        /// <returns>return true if portal update else false.</returns>
        bool UpdatePortal(PortalModel portalModel);

        /// <summary>
        /// Delete a portal by portal Id.
        /// </summary>
        /// <param name="portalId">Id of portal to delete portal.</param>
        /// <returns>return true if deleted else false.</returns>
        bool DeletePortal(ParameterModel portalIds, bool isDeleteByStoreCode);

        /// <summary>
        /// Copy a store.
        /// </summary>
        /// <param name="portalModel">Portal model to be Copied</param>
        /// <returns>Return true or false.</returns>
        bool CopyStore(PortalModel portalModel);

        /// <summary>
        /// Get the list of features to create/update portal.
        /// </summary>
        /// <returns></returns>
        List<PortalFeatureModel> GetPortalFeatures();

        /// <summary>
        /// Get associated warehouse as per selected portal Id.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Returns PortalWarehouseModel</returns>
        PortalWarehouseModel GetAssociatedWarehouseList(int portalId, FilterCollection filters, NameValueCollection expands);

        /// <summary>
        /// Associate warehouse to store.
        /// </summary>
        /// <param name="model">PortalWarehouseModel to be associated.</param>
        /// <returns>Http response containing boolean value whether warehouse are associated or not.</returns>
        bool AssociateWarehouseToStore(PortalWarehouseModel model);

        /// <summary>
        /// Check portal code already exist or not.
        /// </summary>
        /// <param name="parameterModel">parameter model</param>
        /// <returns>Returns true if portal code already exist.</returns>
        bool IsCodeExists(HelperParameterModel parameterModel);

        #region Portal Locale
        /// <summary>
        /// Gets a list of Active Locale.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Locale list.</param>
        /// <param name="filters">Filters to be applied on Locale list.</param>
        /// <param name="sorts">Sorting to be applied on Locale list.</param>
        /// <returns>Returns Locale list model.</returns>
        LocaleListModel LocaleList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Updates Locale.
        /// </summary>
        /// <param name="globalConfigListModel">DefaultGlobalConfigListModel to be updated</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateLocale(DefaultGlobalConfigListModel globalConfigListModel);
        #endregion

        #region Shipping Association
        /// <summary>
        /// Get portal shipping data on the basis of portalId.
        /// </summary>
        /// <param name="portalId">portalId.</param>
        /// <param name="filters">Filter collection.</param>
        /// <returns>Returns Portal Shipping Model.</returns>
        PortalShippingModel GetPortalShippingInformation(int portalId, FilterCollection filters);

        /// <summary>
        /// Update portal shipping data.
        /// </summary>
        /// <param name="portalShippingModel">portalShippingModel to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdatePortalShipping(PortalShippingModel portalShippingModel);
        #endregion

        #region Tax Association
        /// <summary>
        /// Get portal tax data on the basis of portalId.
        /// </summary>
        /// <param name="portalId">portalId.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Returns Portal Tax Model.</returns>
        TaxPortalModel GetTaxPortalInformation(int portalId, NameValueCollection expands);

        /// <summary>
        /// Update portal tax data.
        /// </summary>
        /// <param name="taxPortalModel">TaxPortalModel to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateTaxPortal(TaxPortalModel taxPortalModel);
        #endregion

        #region WebStorePortal
        /// <summary>
        /// Get Portal information on the basis of Portal Id for Web Store.
        /// </summary>
        /// <param name="portalId">Portal Id whose information has to be fetched.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Returns WebStorePortal Model containing portal information.</returns>
        WebStorePortalModel WebStoreGetPortal(int portalId, NameValueCollection expands);

        /// <summary>
        /// Get Portal information on the basis of Portal Id for Web Store.
        /// </summary>
        /// <param name="portalId">Portal Id whose information has to be fetched.</param>
        /// <param name="localeId">Locale Id to fetch the content for.</param>
        /// <param name="applicationType">Application type of the portal which has to be picked.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Returns WebStorePortal Model containing portal information.</returns>
        WebStorePortalModel WebStoreGetPortal(int portalId, int localeId, ApplicationTypesEnum applicationType, NameValueCollection expands);


        /// <summary>
        /// Get domainName information on the basis of domainName for Web Store.
        /// </summary>
        /// <param name="domainName">Portal Id whose information has to be fetched.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Returns WebStorePortal Model containing portal information.</returns>
        WebStorePortalModel WebStoreGetPortal(string domainName, NameValueCollection expands);
        
        #endregion

        #region Tax
        /// <summary>
        /// Associate/UnAssociate tax class to portal.
        /// </summary>
        /// <param name="taxClassPortalModel">TaxClassPortal Model </param>
        /// <returns>return status as true/false.</returns>
        bool AssociateAndUnAssociateTaxClass(TaxClassPortalModel taxClassPortalModel);

        /// <summary>
        /// Set default tax for portal.
        /// </summary>
        /// <param name="taxClassPortalModel">TaxClassPortalModel</param>
        /// <returns>return status as true/false. </returns>
        bool SetPortalDefaultTax(TaxClassPortalModel taxClassPortalModel);
        #endregion

        #region Portal Tracking Pixel
        /// <summary>
        /// Get Tracking Pixels. 
        /// </summary>
        /// <param name="portalId">portal Id.</param>
        /// <param name="expands">expand collection.</param>
        /// <returns>portalTrackingPixelModel</returns>
        PortalTrackingPixelModel GetPortalTrackingPixel(int portalId, NameValueCollection expands);

        /// <summary>
        /// Save Portal Tracking Pixel.
        /// </summary>
        /// <param name="portalTrackingPixelModel">portalTrackingPixelModel</param>
        /// <returns>Returns true if saved else false.</returns>
        bool SavePortalTrackingPixel(PortalTrackingPixelModel portalTrackingPixelModel);
        #endregion

        /// <summary>
        /// Get portal publish status.
        /// </summary>
        /// <param name="filters">List of filter tuples</param>
        /// <param name="sorts">Collection of sorting parameters</param>
        /// <param name="page">Collection of paging parameters</param>
        /// <returns>Publish Portal Log List Model</returns>
        PublishPortalLogListModel GetPortalPublishStatus(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        #region Robots.txt
        /// <summary>
        /// Get robot.txt data.
        /// </summary>
        /// <param name="portalId">Portal id to get data.</param>
        /// <param name="expands">Expands Collection.</param>
        /// <returns>Returns model with data.</returns>
        RobotsTxtModel GetRobotsTxt(int portalId, NameValueCollection expands);

        /// <summary>
        /// Save robots.txt data.
        /// </summary>
        /// <param name="robotsTxtModel">Uses model with data.</param>
        /// <returns>Returns true if data saved successfully else returns false.</returns>
        bool SaveRobotsTxt(RobotsTxtModel robotsTxtModel);

        /// <summary>
        /// Save/Update the Portal Approval details.
        /// </summary>
        /// <param name="portalApprovalModel">Portal Approval Model.</param>
        /// <returns>boolean</returns>
        bool SaveUpdatePortalApprovalDetails(PortalApprovalModel portalApprovalModel);
        #endregion

        #region Approval Routing
        /// <summary>
        /// Get Portal Approval Details
        /// </summary>
        /// <param name="portalId">Portal Id of Store</param>
        /// <returns></returns>
        PortalApprovalModel GetPortalApprovalDetails(int portalId);

        /// <summary>
        /// Get Portal Approver List
        /// </summary>
        /// <param name="filters">List of filter tuples</param>
        /// <returns>User Approver List</returns>
        List<UserApproverModel> GetPortalApproverList(int portalApprovalId);

        /// <summary>
        /// Get Portal Approval type.
        /// </summary>
        /// <returns>Returns list of portal approver types.</returns>
        PortalApprovalTypeListModel GetPortalApprovalTypeList();

        /// <summary>
        /// Get Portal Approval level.
        /// </summary>
        /// <returns>Returns list of portal approver levels.</returns>
        PortalApprovalLevelListModel GetPortalApprovalLevelList();

        #endregion

        #region Portal Search Filter Setting
        /// <summary>
        /// Get Sort Setting List
        /// </summary>
        /// <param name="expands">expand collection list </param>
        /// <param name="filters">filters list</param>
        /// <param name="sorts">sort list</param>
        /// <param name="page">paging parameters </param>
        /// <returns>Payment Setting List Model</returns>
        PortalSortSettingListModel GetPortalSortSettingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Page Setting List
        /// </summary>
        /// <param name="expands">expand collection list </param>
        /// <param name="filters">filters list</param>
        /// <param name="sorts">sort list</param>
        /// <param name="page">paging parameters </param>
        /// <returns>Payment Setting List Model</returns>
        PortalPageSettingListModel GetPortalPageSettingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Remove Associated sort setting
        /// </summary>
        /// <param name="associationModel"></param>
        /// <returns></returns>
        bool RemoveAssociatedSortSettings(SortSettingAssociationModel associationModel);

        /// <summary>
        /// Remove Associated page setting
        /// </summary>
        /// <param name="associationModel"></param>
        /// <returns></returns>
        bool RemoveAssociatedPageSettings(PageSettingAssociationModel associationModel);

        /// <summary>
        /// Associate sort settings
        /// </summary>
        /// <param name="associationModel"></param>
        /// <returns></returns>
        bool AssociateSortSettings(SortSettingAssociationModel associationModel);

        /// <summary>
        /// Associate page settings
        /// </summary>
        /// <param name="associationModel"></param>
        /// <returns></returns>
        bool AssociatePageSettings(PageSettingAssociationModel associationModel);

        /// <summary>
        /// Update portal page setting
        /// </summary>
        /// <param name="portalPageSettingModel"></param>
        bool UpdatePortalPageSetting(PortalPageSettingModel portalPageSettingModel);
        #endregion

        /// <summary>
        /// Get barcode setting specific.
        /// </summary>
        /// <returns></returns>
        BarcodeReaderModel GetBarcodeScanner();
    }
}
