using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Models;
using Znode.Engine.Api.Client.Expands;

namespace Znode.Engine.Admin.Agents
{
    public interface IStoreAgent
    {
        /// <summary>
        /// Get the list of all portals.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>PortalListModel.</returns>
        StoreListViewModel GetStoreList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null);

        /// <summary>
        /// Get details of store by store Id.
        /// </summary>
        /// <param name="portalId">Id of portal to get portal details.</param>
        /// <param name="expands">Expands collection.</param>
        /// <returns>Store view model.</returns>
        StoreViewModel GetStore(int portalId, ExpandCollection expands = null);

        /// <summary>
        /// Get details of store by store Code.
        /// </summary>
        /// <param name="storeCode">storeCode to get portal details</param>
        /// <param name="expands">Expands collection.</param>
        /// <returns>Store view model.</returns>
        StoreViewModel GetStore(string storeCode, ExpandCollection expands = null);
        /// <summary>
        /// Create new Store.
        /// </summary>
        /// <param name="storeViewModel">Store View Model.</param>
        /// <returns>Returns StoreViewModel.</returns>
        StoreViewModel CreateStore(StoreViewModel storeViewModel);

        /// <summary>
        /// Update store.
        /// </summary>
        /// <param name="storeViewModel">Store View Model.</param>
        /// <param name="errorMessage">Outputs error message according to error code.</param>
        /// <returns>returns true if store updated else false.</returns>
        bool UpdateStore(StoreViewModel storeViewModel, out string errorMessage);

        /// <summary>
        /// Get all portals on CatalogId
        /// </summary>
        /// <param name="catalogId"></param>
        /// <param name="portalId"></param>
        /// <returns></returns>
        List<SelectListItem> GetPortalListByCatalogId(int catalogId, int portalId);


        /// <summary>
        /// Delete portal by store Id.
        /// </summary>
        /// <param name="portalId">Id of portal to delete portal.</param>
        /// <param name="isDeleteByStoreCode">if true then portal is deleted by storecode</param>
        /// <param name="errorMessage">errorMessage if error occured while deleting portal.</param>
        /// <returns>return true if deleted else false.</returns>
        bool DeleteStore(string portalId, out string errorMessage,bool isDeleteByStoreCode=false);

        // <summary>
        /// Creates a copy of an existing Store.
        /// </summary>
        /// <param name="storeViewModel">Model of the store</param>
        /// <returns>Returns true or false.</returns>
        bool CopyStore(StoreViewModel storeViewModel);

        /// <summary>
        /// Get other details to create/update store.
        /// </summary>
        /// <param name="storeViewModel">StoreViewModel</param>
        /// <returns>StoreViewModel</returns>
        StoreViewModel GetStoreInformation(StoreViewModel storeViewModel);

        /// <summary>
        /// Gets css list based on selected theme in store.
        /// </summary>
        /// <param name="cmsThemeId">To get css list.</param>
        /// <returns>List of select list item of css.</returns>
        List<SelectListItem> GetCSSList(int cmsThemeId);

        /// <summary>
        /// Check store code already exist or not.
        /// </summary>
        /// <param name="storeCode">storeCode.</param>
        /// <returns>Returns true if store code exist.</returns>
        bool IsStoreCodeExist(string storeCode);
        
        /// <summary>
        /// Create tab structure.
        /// </summary>
        /// <param name="portalId">PortalId.</param>
        /// <param name="profileId">ProfileId.</param>
        /// <returns>Returns created tab structure.</returns>
        TabViewListModel CreateTabStructure(int portalId, int? profileId);

        /// <summary>
        /// Create tab structure.
        /// </summary>
        /// <param name="portalId">PortalId.</param>
        /// <returns>Returns created tab structure.</returns>
        TabViewListModel CreateTabStructureForShippingOrigin(int portalId);

        /// <summary>
        /// Create tab structure.
        /// </summary>
        /// <param name="portalId">PortalId.</param>
        /// <returns>Returns created tab structure.</returns>
        TabViewListModel CreateTabStructureForSearch(int portalId);


        /// <summary>
        /// Set filters for portalid.
        /// </summary>
        /// <param name="filters">Filters for PortalId.</param>
        /// <param name="portalId">PortalId.</param>
        void SetFiltersForPortalId(FilterCollection filters, int portalId);

        /// <summary>
        /// Set filters for profileId.
        /// </summary>
        /// <param name="filters">Filters for ProfileId.</param>
        /// <param name="profileId">ProfileId.</param>
        /// <param name="portalId">PortalId.</param>
        void SetFiltersForProfileId(FilterCollection filters, int profileId, int portalId);

        /// <summary>
        ///  Get Profiles associated to store.
        /// </summary>
        /// <param name="portalId">PortalId</param>
        /// <returns>Returns Profiles associated to store.</returns>
        List<SelectListItem> GetPortalProfiles(int portalId);

        /// <summary>
        /// Get the list of portals.
        /// </summary>
        /// <param name="portalId">Portal id.</param>
        /// <returns>Returns list of portals.</returns>
        List<SelectListItem> GetPortalSelectList(int portalId);

        /// <summary>
        /// Get portal CatalogList
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PortalCatalogListViewModel GetPublishCatalogList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null);
        #region Inventory Management

        /// <summary>
        /// Gets list of associated warehouse as per portalId.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="filters">Filters to be applied on mapper list.</param>
        /// <returns>Returns PortalWarehouseViewModel.</returns>
        PortalWarehouseViewModel GetAssociatedWarehouseList(int portalId, int warehouseId, ExpandCollection expands = null, FilterCollection filters = null);

        /// <summary>
        /// Associate warehouses to respective store.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="warehouseId">Warehouse Id to be associated with Portal Id</param>
        /// <param name="alternateWarehouseIds">Alternate Warehouse Ids to be associated with Portal Id</param>
        /// <returns>True/False value according the status of associate operation.</returns>
        bool AssociateWarehouseToStore(int portalId, int warehouseId, string alternateWarehouseIds);

        #endregion

        /// <summary>
        /// Get all the domains from specified portal id
        /// </summary>
        /// <param name="portalId">portal id to get the associated domains</param>
        /// <param name="filters">filter list across Domains</param>
        /// <param name="sortCollection">sort collection for domains.</param>
        /// <param name="pageIndex">pageIndex for Domain record </param>
        /// <param name="recordPerPage">paging domain record per page</param>
        /// <returns></returns>
        DomainListViewModel GetDomains(int portalId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get list for Price.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Price list.</param>
        /// <param name="filters">Filters to be applied on Price list.</param>
        /// <param name="sorts">Sorting to be applied on Price list.</param>
        /// <param name="pageIndex">Start page index of Price list.</param>
        /// <param name="pageSize">Page size of Price list.</param>
        /// <returns>Returns Price list.</returns>
        PriceListViewModel GetPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        #region Portal Locale
        /// <summary>
        /// Get the list of all active Locales.
        /// </summary>
        /// <param name="filters">Filter collection</param>
        /// <param name="sortCollection">Sort collection</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>Locale List View Model</returns>
        LocaleListViewModel GetLocales(int portalId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Method to Update Locale in database.
        /// </summary>
        /// <param name="model">DefaultGlobalConfigViewModel model</param>
        /// <returns>Returns true/false</returns>
        bool UpdateLocale(DefaultGlobalConfigViewModel model, out string message);
        #endregion

        #region Portal Shipping
        /// <summary>
        /// Get shipping information by portal id.
        /// </summary>
        /// <param name="portalId">portalId to get information of specified id.</param>
        /// <param name="publishStateId">publishStateId to get information of application type.</param>
        /// <returns>returns portal shipping info.</returns>
        PortalShippingViewModel GetPortalShippingInformation(int portalId, int publishStateId);

        /// <summary>
        /// Updates the portal shipping data.
        /// </summary>
        /// <param name="portalShippingViewModel">Portal shipping model to be updated.</param>
        /// <returns>Returns updated portal shipping model.</returns>
        PortalShippingViewModel UpdatePortalShipping(PortalShippingViewModel portalShippingViewModel);

        /// <summary>
        /// To bind page dropdown for create/edit
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="portalId">portalId</param>
        /// <returns></returns>
        PortalShippingViewModel BindPageDropdown(PortalShippingViewModel model, int portalId);

        #endregion

        #region Tax Portal
        /// <summary>
        /// Get tax information by portal id.
        /// </summary>
        /// <param name="portalId">portalId to get information of specified id.</param>
        /// <returns>Returns portal tax info.</returns>
        TaxPortalViewModel GetTaxPortalInformation(int portalId);

        /// <summary>
        /// Updates the portal tax data.
        /// </summary>
        /// <param name="taxPortalViewModel">Portal shipping model to be updated.</param>
        /// <returns>Returns updated portal tax model.</returns>
        TaxPortalViewModel UpdateTaxPortal(TaxPortalViewModel taxPortalViewModel);
        #endregion

        #region Tax
        /// <summary>
        /// Get the list of tax class.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns TaxClass list ViewModel.</returns>
        TaxClassListViewModel GetTaxClassList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId, bool isAssociated);

        /// <summary>
        /// Associate / unassociate tax class.
        /// </summary>
        /// <param name="portalId">PortalId</param>
        /// <param name="taxClassIds">taxClassIds</param>
        /// <param name="isUnAssociated">isUnAssociated</param>
        /// <returns>return status as true or false.</returns>
        bool AssociateUnAssociateTaxClass(int portalId, string taxClassIds, bool isUnAssociated, out string message);

        /// <summary>
        /// Set portal default tax class.
        /// </summary>
        /// <param name="portalId">portalId</param>
        /// <param name="taxClassIds">taxClassIds</param>
        /// <returns>return status as true or false.</returns>
        bool SetPortalDefaultTax(int portalId, string taxClassIds, out string message);
        #endregion

        #region Payment Setting
        /// <summary>
        /// Get payment settings list.
        /// </summary>
        /// <param name="portalId">Portal id.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="isUnassociated">Is Unassociated</param>
        /// <returns>Returns payment settings list.</returns>
        PaymentSettingListViewModel GetPaymentSettingsList(int portalId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isUnassociated);

        /// <summary>
        /// Associate payment settings to portal.
        /// </summary>
        /// <param name="portalId">Portal id.</param>
        /// <param name="paymentSettingIds">Payment settings ids.</param>
        /// <param name="message">Error Message in case of Already Associate Payment Type.</param>
        /// <returns>Returns true if associated successfully else false.</returns>
        bool AssociatePaymentSettings(int portalId, string paymentSettingIds, out string message);

        /// <summary>
        /// Remove associated payment settings to portal.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="paymentSettingIds">Payment settings ids.</param>
        /// <returns>Returns true if association removed successfully else false.</returns>
        bool RemoveAssociatedPaymentSettings(int portalId, string paymentSettingIds, bool isUsedForOfflinePayment = false);
        
        /// <summary>
        /// //Get publisg catalog list with json result for autocomplete catalog
        /// </summary>
        /// <param name="catalogName"></param>
        /// <returns></returns>
        PortalCatalogListViewModel GetCatalogList(string catalogName);
        #endregion


        #region Portal Search Filter Setting
        /// <summary>
        /// Get Sort settings list.
        /// </summary>
        /// <param name="portalId">Portal id.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="isUnassociated">Is Unassociated</param>
        /// <returns>Returns payment settings list.</returns>
        PortalSortSettingListViewModel GetPortalSortSettingsList(int portalId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isUnassociated);

        /// <summary>
        /// Get Page settings list.
        /// </summary>
        /// <param name="portalId">Portal id.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Page</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="isUnassociated">Is Unassociated</param>
        /// <returns>Returns payment settings list.</returns>
        PortalPageSettingListViewModel GetPortalPageSettingsList(int portalId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, bool isUnassociated);

        /// <summary>
        /// Associate sort settings to portal.
        /// </summary>
        /// <param name="portalId">Portal id.</param>
        /// <param name="paymentSettingIds">Payment settings ids.</param>
        /// <param name="message">Error Message in case of Already Associate Payment Type.</param>
        /// <returns>Returns true if associated successfully else false.</returns>
        bool AssociateSortSettings(int portalId, string sortSettingIds, out string message);

        /// <summary>
        /// Associate sort settings to portal.
        /// </summary>
        /// <param name="portalId">Portal id.</param>
        /// <param name="paymentSettingIds">Payment settings ids.</param>
        /// <param name="message">Error Message in case of Already Associate Payment Type.</param>
        /// <returns>Returns true if associated successfully else false.</returns>
        bool AssociatePageSettings(int portalId, string pageSettingIds, out string message);

        /// <summary>
        /// Remove associated sort settings to portal.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="paymentSettingIds">Payment settings ids.</param>
        /// <returns>Returns true if association removed successfully else false.</returns>
        bool RemoveAssociatedSortSettings(int portalId, string portalSortSettingIds);

        /// <summary>
        /// Remove associated page settings to portal.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="paymentSettingIds">Payment settings ids.</param>
        /// <param name="errorMessage">Error Message.</param>
        /// <returns>Returns true if association removed successfully else false.</returns>
        bool RemoveAssociatedPageSettings(int portalId, string portalPageSettingIds, out string errorMessage);

        /// <summary>
        /// Update Portal Page Setting
        /// </summary>
        /// <param name="domainViewModel"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool UpdatePortalPageSetting(PortalPageSettingViewModel domainViewModel, out string errorMessage);
        #endregion


        #region Associate Portal Shipping
        /// <summary>
        /// Get associated shipping list for portal.
        /// </summary>
        /// <param name="filters">Filters for portal shipping.</param>
        /// <param name="sorts">Sorts for for portal shipping.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Portal Shipping.</returns>
        ShippingListViewModel GetAssociatedShippingList(int portalId, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get list of unassociated shipping for portal.
        /// </summary>
        /// <param name="filters">Filters for portal shipping.</param>
        /// <param name="sorts">Sorts for for portal shipping.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="recordPerPage">Records per page.</param>
        /// <returns>Returns list of unassociated shipping list.</returns>
        ShippingListViewModel GetUnAssociatedShippingList(int portalId, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Associate shipping to portal.
        /// </summary>
        /// <param name="portalId">portalId.</param>
        /// <param name="shippingIds">shippingIds to be associated.</param>
        /// <returns>Returns true if shipping associated successfully else return false.</returns>
        bool AssociateShipping(int portalId, string shippingIds);

        /// <summary>
        /// Remove associated shipping from portal.
        /// </summary>
        /// <param name="shippingId">ShippingIds to be removed.</param>
        /// <param name="portalId">portalId</param>
        /// <returns>Returns true if shipping unassociated successfully else return false.</returns>
        bool UnAssociateAssociatedShipping(string shippingId, int portalId);
        #endregion

        /// <summary>
        /// Get portal publish status.
        /// </summary>
        /// <param name="pimCatalogId">pimCatalogId</param>
        /// <param name="expands">Expands to be retrieved along with portal list.</param>
        /// <param name="filters">Filters to be applied on portal list.</param>
        /// <param name="sorts">Sorting to be applied on portal list.</param>
        /// <param name="pageIndex">Start page index of portal list.</param>
        /// <param name="pageSize">Page size of portal list.</param>
        /// <returns>Publish portal List View Model</returns>
        PublishPortalLogListViewModel GetPortalPublishStatus(int pimCatalogId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Analytics by portalid
        /// </summary>
        /// <param name="portalId">portal id.</param>
        /// <returns>viewmodel with analytics.</returns>
        AnalyticsViewModel GetAnalytics(int portalId);

        /// <summary>
        /// Save analtics data.
        /// </summary>
        /// <param name="vieModel">Model with analytics data</param>
        /// <returns>true or false</returns>
        bool SaveAnalytics(AnalyticsViewModel vieModel, out string errorMessage);
        
        /// <summary>
        /// Get robots.txt data.
        /// </summary>
        /// <param name="portalId">portal id.</param>
        /// <returns>Returns view model with data.</returns>
        RobotsTxtViewModel GetRobotsTxt(int portalId);

        /// <summary>
        /// Save robots.txt data.
        /// </summary>
        /// <param name="viewModel">Uses model with data.</param>
        /// <returns>Returns true if saved successfully else returns false.</returns>
        bool SaveRobotsTxt(RobotsTxtViewModel viewModel);

        /// <summary>
        /// Tests the Avalara Connection.
        /// </summary>
        /// <param name="model">Tax Portal View Model to be tested.</param>
        /// <returns>Returns true if tested successfully.</returns>
        string TestAvalaraConnection(TaxPortalViewModel model);


        /// <summary>
        /// Update portal payment settings.
        /// </summary>
        /// <param name="paymentSettingId">paymentSetting Id</param>
        /// <param name="portalId">portal Id</param>
        /// <param name="data">data</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool UpdatePortalPaymentSettings(int paymentSettingId, int portalId, string data);

        /// <summary>
        /// Get Portal Approver Details.
        /// </summary>
        /// <param name="portalId">Returns portal approval details by id</param>
        /// <param name="selectedApprovalTypeId">Approval Type Id</param>
        PortalApprovalViewModel GetPortalApproverDetailsById(int portalId, int selectedApprovalTypeId);

        /// <summary>
        /// Delete the approver level by id 
        /// </summary>
        /// <param name="userApproverId">User approver id.</param>
        /// <returns>Return true or false.</returns>
        bool DeletePortalApproverUserById(string userApproverId);

        /// <summary>
        /// Create and update store approvers.
        /// </summary>
        /// <param name="model">Model of Type Portal Approval View Model</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>Return true or false</returns>
        bool SaveUpdatePortalApprovalDetails(PortalApprovalViewModel model, out string errorMessage);

        /// <summary>
        /// Gets payment type list by portalId.
        /// </summary>
        /// <param name="portalId">portalId.</param>
        ///  <param name="paymentIds">paymentIds.</param>
        /// <returns>List of select list item of payment types.</returns>
        List<SelectListItem> GetPaymentTypeList(int portalId, string[] paymentIds = null);

        /// <summary>
        /// Get application Type 
        /// </summary>
        /// <returns>returns SelectListItem</returns>
        List<SelectListItem> GetApplicationType();

        /// <summary>
        /// Get application Type 
        /// </summary>
        /// <returns>returns SelectListItem</returns>
        bool UpdateShippingToPortal(int portalId, string shippingIds, string applicationType = "");

        /// <summary>
        /// Associate payment settings for invoice 
        /// </summary>
        /// <param name="portalId">portalId.</param>
        ///  <param name="paymentSettingIds">paymentSettingIds.</param>
        ///  <param name="message">message.</param>
        /// <returns>returns association status</returns>
        bool AssociatePaymentSettingsForInvoice(int portalId, string paymentSettingIds, out string message);

    }
}
