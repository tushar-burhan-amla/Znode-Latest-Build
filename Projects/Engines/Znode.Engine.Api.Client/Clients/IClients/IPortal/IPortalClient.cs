using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IPortalClient : IBaseClient
    {
        /// <summary>
        /// Get the list of all portals.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>PortalListModel.</returns>
        PortalListModel GetPortalList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get All Sort Settings
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Payment Settings list.</param>
        /// <param name="filters">Filters to be applied on Payment Settings list.</param>
        /// <param name="sorts">Sorting to be applied on Payment Settings list.</param>
        /// <param name="pageIndex">Start page index of Payment Settings list.</param>
        /// <param name="pageSize">Page size of Payment Settings list.</param>
        /// <returns>Payment Setting List Model </returns>
        PortalSortSettingListModel GetPortalSortSettings(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get All Page Settings
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Payment Settings list.</param>
        /// <param name="filters">Filters to be applied on Payment Settings list.</param>
        /// <param name="sorts">Sorting to be applied on Payment Settings list.</param>
        /// <param name="pageIndex">Start page index of Payment Settings list.</param>
        /// <param name="pageSize">Page size of Payment Settings list.</param>
        /// <returns>Payment Setting List Model </returns>
        PortalPageSettingListModel GetPortalPageSettings(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Remove associated sort settings to portal.
        /// </summary>
        /// <param name="model">Association model.</param>
        /// <returns>Returns true if association removed successfully else false.</returns>
        bool RemoveAssociatedSortSettings(SortSettingAssociationModel model);


        /// <summary>
        /// Remove associated page settings to portal.
        /// </summary>
        /// <param name="model">Association model.</param>
        /// <returns>Returns true if association removed successfully else false.</returns>
        bool RemoveAssociatedPageSettings(PageSettingAssociationModel model);

        /// <summary>
        /// Associate sort settings to portal.
        /// </summary>
        /// <param name="model">Association model.</param>
        /// <returns>Returns true if associated successfully else false.</returns>
        bool AssociateSortSettings(SortSettingAssociationModel model);

        /// <summary>
        /// Associate sort settings to portal.
        /// </summary>
        /// <param name="model">Association model.</param>
        /// <returns>Returns true if associated successfully else false.</returns>
        bool AssociatePageSettings(PageSettingAssociationModel model);

        /// <summary>
        /// Update portal page setting
        /// </summary>
        /// <param name="domainViewModel"></param>
        /// <returns></returns>
        bool UpdatePortalPageSetting(PortalPageSettingModel domainViewModel);

#if DEBUG
        /// <summary>
        /// Get the list of all portals.
        /// </summary> 
        /// <returns>PortalListModel.</returns>
        PortalListModel GetDevPortalList();

#endif

        /// <summary>
        /// Get detials of portal by portal Id.
        /// </summary>
        /// <param name="portalId">Id of portal to get portal detials.</param>
        /// <param name="expands">>Expand Collection.</param>
        /// <returns>Portal model.</returns>
        PortalModel GetPortal(int portalId, ExpandCollection expands);

        /// <summary>
        /// Get details of portal by portal storeCode.
        /// </summary>
        /// <param name="storeCode">storeCode to get portal details</param>
        /// <param name="expands">>Expand Collection.</param>
        /// <returns>Portal model.</returns>
        PortalModel GetPortal(string storeCode, ExpandCollection expands);
        /// <summary>
        /// Create new portal.
        /// </summary>
        /// <param name="portalModel">Portal model.</param>
        /// <returns>Newly created portal model.</returns>
        PortalModel CreatePortal(PortalModel portalModel);

        /// <summary>
        /// Update Portal.
        /// </summary>
        /// <param name="portalModel">Portal Model.</param>
        /// <returns>Updated portal model.</returns>
        PortalModel UpdatePortal(PortalModel portalModel);

        /// <summary>
        /// Delete portal by portal Id.
        /// </summary>
        /// <param name="portalIds">Id of portal to delete portal.</param>
        /// <param name="isDeleteByStoreCode">true if delete is perform by storeCode</param>
        /// <returns></returns>
        bool DeletePortal(ParameterModel portalIds, bool isDeleteByStoreCode);

        /// <summary>
        /// Copy Store based on portalId.
        /// </summary>
        /// <param name="portalModel">Portal model to be Copied.</param>
        /// <returns>Return true or false</returns>
        bool CopyStore(PortalModel portalModel);

        /// <summary>
        /// Get all portals by CatalogId
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        PortalListModel GetPortalListByCatalogId(int catalogId);

        /// <summary>
        /// Get the list all portal features.
        /// </summary>
        /// <returns>List of all portal features.</returns>
        List<PortalFeatureModel> GetPortalFeatureList();
        
        /// <summary>
        /// Check portal code already exist or not.
        /// </summary>
        /// <param name="portalCode">portalCode</param>
        /// <returns>Returns true if portal code exist.</returns>
        bool IsPortalCodeExist(string portalCode);
        #region Inventory Management

        /// <summary>
        /// Gets list of associated warehouse as per portalId.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expands to be retrieved along with mapper list.</param>
        /// <returns>Returns PortalWarehouseModel.</returns>
        PortalWarehouseModel GetAssociatedWarehouseList(int portalId, FilterCollection filters, ExpandCollection expands);

        /// <summary>
        /// Associate warehouse to respective portal.
        /// </summary>
        /// <param name="portalWarehouseModel">PortalWarehouseModel to be associated.</param>
        /// <returns>True/False value according the status of associate operation.</returns>
        bool AssociateWarehouseToStore(PortalWarehouseModel portalWarehouseModel);

        #endregion

        #region Portal Locale
        /// <summary>
        /// Get active Locale list.
        /// </summary>
        /// <param name="expands">Expands for Locale  </param>
        /// <param name="filters">Filters for Locale</param>
        /// <param name="sorts">Sorts for Locale</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>Returns LocaleListModel</returns>
        LocaleListModel GetLocaleList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Updates Locales.
        /// </summary>       
        /// <param name="DefaultGlobalConfigListModel">DefaultGlobalConfigListModel to be updated.</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateLocale(DefaultGlobalConfigListModel globalConfigurationListModel);
        #endregion

        #region Shipping Association
        /// <summary>
        /// Get portal shipping on the basis of portalId.
        /// </summary>
        /// <param name="portalId">portalId to get portal shipping details.</param>
        /// <returns>Returns portalShippingModel.</returns>
        PortalShippingModel GetPortalShippingInformation(int portalId, FilterCollection filters);

        /// <summary>
        /// Update Portal shipping data.
        /// </summary>
        /// <param name="portalShippingModel">portalShippingModel to update.</param>
        /// <returns>Returns updated portalShippingModel.</returns>
        PortalShippingModel UpdatePortalShipping(PortalShippingModel portalShippingModel);
        #endregion

        #region Tax Portal Association
        /// <summary>
        /// Get portal tax on the basis of portalId.
        /// </summary>
        /// <param name="portalId">portalId to get portal shipping details.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Returns portalShippingModel.</returns>
        TaxPortalModel GetTaxPortalInformation(int portalId, ExpandCollection expands);

        /// <summary>
        /// Update Portal tax data.
        /// </summary>
        /// <param name="taxPortalModel">portalShippingModel to update.</param>
        /// <returns>Returns updated taxPortalModel.</returns>
        TaxPortalModel UpdateTaxPortal(TaxPortalModel taxPortalModel);
        #endregion            

        #region Tax
        /// <summary>
        /// Associate and unassociate taxclass to portal.
        /// </summary>
        /// <param name="taxClassPortalModel">TaxClassPortalModel</param>
        /// <returns>Returns status as true/false. </returns>
        bool AssociateAndUnAssociateTaxClass(TaxClassPortalModel taxClassPortalModel);

        /// <summary>
        /// Set default tax for portal.
        /// </summary>
        /// <param name="taxClassPortalModel">TaxClassPortalModel</param>
        /// <returns>Returns status as true/false. </returns>
        bool SetPortalDefaultTax(TaxClassPortalModel taxClassPortalModel);
        #endregion

        #region Portal Tracking Pixel
        /// <summary>
        /// Get Tracking Pixels. 
        /// </summary>
        /// <param name="portalId">portalId.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>portalTrackingPixel.</returns>
        PortalTrackingPixelModel GetPortalTrackingPixel(int portalId, ExpandCollection expands);

        /// <summary>
        /// Save Portal Tracking Pixel.
        /// </summary>
        /// <param name="portalTrackingPixel">portalTrackingPixel</param>
        /// <returns>Returns true if saved else false.</returns>
        bool SavePortalTrackingPixel(PortalTrackingPixelModel portalTrackingPixel);
        #endregion

        /// <summary>
        ///  Get portal publish status.
        /// </summary>
        /// <param name="filters">Filters to be applied on associated portal list.</param>
        /// <param name="sorts">Sorting to be applied on associated portal list.</param>
        /// <param name="pageIndex">Start page index of associated portal list.</param>
        /// <param name="pageSize">Page size of Associated portal list.</param>
        /// <returns>Publish portal Log List Model</returns>
        PublishPortalLogListModel GetPortalPublishStatus(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);


        /// <summary>
        /// Get barcode setting details
        /// </summary>
        /// <returns></returns>
        BarcodeReaderModel GetBarcodeScannerDetail();

        #region Robots.txt
        /// <summary>
        /// Get robot.txt data.
        /// </summary>
        /// <param name="portalId">Uses portal id.</param>
        /// <param name="expands">Expands Collectiom.</param>
        /// <returns>Returns model with information.</returns>
        RobotsTxtModel GetRobotsTxt(int portalId, ExpandCollection expands);

        /// <summary>
        /// Save Robots.txt.
        /// </summary>
        /// <param name="model">Uses Model with data.</param>
        /// <returns>Returns true if saved successfully else false.</returns>
        bool SaveRobotsTxt(RobotsTxtModel model);

        /// <summary>
        /// Get PortalApproval details by portalId
        /// </summary>
        /// <param name="portalId">int portalId</param>
        /// <returns>Portal Approval details</returns>
        PortalApprovalModel GetPortalApproverDetailsById(int portalId);

        /// <summary>
        /// Delete approverLevel by Id.
        /// </summary>
        /// <param name="userApproverId">User Approver Id.</param>
        /// <returns>Returns true if deleted sucessfully else return false.</returns>
        bool DeletePortalApproverUserById(ParameterModel userApproverId);

        /// <summary>
        /// Save/Update the Portal Approval details.
        /// </summary>
        /// <param name="portalApprovalModel">Portal Approval Model.</param>
        /// <returns>boolean</returns>
        bool SaveUpdatePortalApprovalDetails(PortalApprovalModel portalApprovalModel);
        #endregion


    }
}
