using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IPriceAgent
    {
        #region Price
        /// <summary>
        ///  Get Currency list by currency id.
        /// </summary>
        /// <param name="currencyId">Currency Id.</param>
        /// <returns>Returns list of currency type.</returns>
        List<SelectListItem> GetCurrencyList(int currencyId);

        /// <summary>
        ///  Get Currency list by culture id.
        /// </summary>
        /// <param name="currencyId">Culture Id.</param>
        /// <returns>Returns list of culture type.</returns>
        List<SelectListItem> GetCultureList(int cultureId, int currencyId);

        /// <summary>
        /// Create Price.
        /// </summary>
        /// <param name="priceViewModel">Price View Model.</param>
        /// <returns>Returns created model.</returns>
        PriceViewModel Create(PriceViewModel priceViewModel);

        /// <summary>
        /// Get Price list by Price list id.
        /// </summary>
        /// <param name="priceListId">Price List Id</param>
        /// <returns>Returns PriceViewModel.</returns>
        PriceViewModel GetPrice(int priceListId);

        /// <summary>
        /// Update Price data.
        /// </summary>
        /// <param name="priceViewModel">Price View Model to update.</param>
        /// <returns>Returns updated Price model.</returns>
        PriceViewModel Update(PriceViewModel priceViewModel);

        /// <summary>
        /// Get list for Price.
        /// </summary>
        /// <param name="filters">Filters to be applied on Price list.</param>
        /// <param name="sorts">Sorting to be applied on Price list.</param>
        /// <returns>Returns Price list.</returns>
        PriceListViewModel GetPriceList(FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get list for Price.
        /// </summary>
        /// <param name="filters">Filters to be applied on Price list.</param>
        /// <param name="sorts">Sorting to be applied on Price list.</param>
        /// <param name="pageIndex">Start page index of Price list.</param>
        /// <param name="pageSize">Page size of Price list.</param>
        /// <returns>Returns Price list.</returns>
        PriceListViewModel GetPriceList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Delete Price.
        /// </summary>
        /// <param name="priceListId">Price List Id.</param>
        /// <param name="errorMessage">Erro message if any to show.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeletePrice(string priceListId, out string errorMessage);

        // <summary>
        /// Creates a copy of an existing Price.
        /// </summary>
        /// <param name="priceViewModel">Model of the Price</param>
        /// <returns>Returns true or false.</returns>
        bool CopyPrice(PriceViewModel priceViewModel, out string errorMessage);

        /// <summary>
        /// Get All Active Price List
        /// </summary>
        /// <returns></returns>
        List<SelectListItem> GetAllActivePriceList();
        #endregion

        #region SKU Price.
        /// <summary>
        /// Add SKU Price.
        /// </summary>
        /// <param name="model">PriceSKUDataViewModel to create SKU Price.</param>
        /// <returns>Added SKU Price view model.</returns>
        PriceSKUDataViewModel AddSKUPrice(PriceSKUDataViewModel model);

        /// <summary>
        /// Get SKUPrice list from database.
        /// </summary>
        /// <param name="PriceListId">Price List Id.</param>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>Returns PriceSKUViewListModel</returns>
        PriceSKUListViewModel GetSKUPriceList(int PriceListId, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get SKU Price by PriceId.
        /// </summary>
        /// <param name="priceId">Get SKU Price on the basis of PriceId.</param>
        /// <returns>Returns PriceSKUViewModel.</returns>
        PriceSKUViewModel GetSKUPrice(int priceId);

        /// <summary>
        /// Update SKU Price.
        /// </summary>
        /// <param name="model">PriceSKUDataViewModel.</param>
        /// <returns>Returns PriceSKUViewModel.</returns>
        PriceSKUDataViewModel UpdateSKUPrice(PriceSKUDataViewModel model);

        /// <summary>
        /// Delete SKU Price on the basis of PriceId.
        /// </summary>
        /// <param name="priceId">Price Ids.</param>
        /// <param name="priceListId">Price List Id.</param>
        /// <param name="message">Erro message if any to show.</param>
        /// <param name="pimProductId">Pim product id.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteSKUPrice(string priceId, int priceListId, out string message, int pimProductId = 0);

        /// <summary>
        /// Get Unit of Measurement.
        /// </summary>
        /// <returns>select list items.</returns>
        List<SelectListItem> UOM();

        /// <summary>
        /// Get price by sku.
        /// </summary>
        /// <param name="sku">Get SKU Price on the basis of sku.</param>
        /// <param name="productType">Product Type</param>
        /// <param name="pimProductId">pimProductId</param>
        /// <returns>Returns PriceSKUViewModel.</returns>
        PriceSKUViewModel GetPriceBySku(string pimProductId, string sku, string productType);

        /// <summary>
        /// Get pricing details by product price model
        /// </summary>
        /// <param name="productPriceListSKUDataViewModel"></param>
        /// <returns></returns>
        PriceSKUViewModel GetProductPricingDetailsBySku(ProductPriceListSKUDataViewModel productPriceListSKUDataViewModel);
        #endregion

        #region Tier Price
        /// <summary>
        /// Save Tier Price values.
        /// </summary>
        /// <param name="model">PriceSKUDataViewModel to save Tier Price.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Returns true if tier price saved successfully else return false.</returns>
        bool AddTierPrice(PriceSKUDataViewModel model, out string message);

        /// <summary>
        /// Get Tier Price list from database.
        /// </summary>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>Returns PriceSKUViewListModel</returns>
        PriceTierListViewModel GetTierPriceList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get Tier Price by PriceId.
        /// </summary>
        /// <param name="priceTierId">Get Tier Price on the basis of PriceTierId.</param>
        /// <returns>Returns PriceTierViewModel.</returns>
        PriceTierViewModel GetTierPrice(int priceTierId);

        /// <summary>
        /// Update Tier Price.
        /// </summary>
        /// <param name="model">PriceSKUDataViewModel.</param>
        /// <returns>Returns PriceTierViewModel.</returns>
        PriceSKUDataViewModel UpdateTierPrice(PriceSKUDataViewModel model);

        /// <summary>
        /// Delete Tier Price on the basis of PriceTierId.
        /// </summary>
        /// <param name="priceTierId">Price Tier Id.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteTierPrice(int priceTierId);

        /// <summary>
        /// Set filters for SKU and Price List.
        /// </summary>
        /// <param name="filters">FilterCollection filters.</param>
        /// <param name="priceListId">Price List Id.</param>
        /// <param name="sku">SKU.</param>
        void SetFiltersForSKUAndPriceList(FilterCollection filters, int priceListId, string sku);
        #endregion

        #region Associate Store

        /// <summary>
        /// Get associated store list for price.
        /// </summary>
        /// <param name="filters">Filters for Portal Price.</param>
        /// <param name="sorts">Sorts for for Portal Price.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Portal Price.</returns>
        PricePortalListViewModel GetAssociatedStoreList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Sets filter for price list id.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="priceListId">PriceListId.</param>
        void SetFilters(FilterCollection filters, int priceListId);

        /// <summary>
        /// Get list of unassociated stores.
        /// </summary>
        /// <param name="filters">Filters for Portal.</param>
        /// <param name="sorts">Sorts for for Portal.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="recordPerPage">Records per page.</param>
        /// <returns>Returns list of unassociated store list.</returns>
        StoreListViewModel GetUnAssociatedStoreList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Associate price to store.
        /// </summary>
        /// <param name="priceListId">Id of price list.</param>
        /// <param name="storeIds">Store Ids to be associated.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Returns true if store associated successfully else return false.</returns>
        bool AssociateStore(int priceListId, string storeIds, out string message);

        /// <summary>
        /// Remove associated stores from price.
        /// </summary>
        /// <param name="priceListPortalIds">PRice List Portal Ids.</param>
        /// <param name="priceListId">Price List Id.</param>
        /// <returns>Returns true if store unassociated successfully else return false.</returns>
        bool RemoveAssociatedStores(string priceListPortalIds, int priceListId);

        /// <summary>
        /// Get associated stores precedence value.
        /// </summary>
        /// <param name="priceListPortalId">Price list portal id.</param>
        /// <returns>Returns PricePortalViewModel.</returns>
        PricePortalViewModel GetAssociatedStorePrecedence(int priceListPortalId);

        /// <summary>
        /// Update associated stores precedence value.
        /// </summary>
        /// <param name="pricePortalModel">PricePortalViewModel model.</param>
        /// <returns>Returns updated PricePortalViewModel.</returns>
        PricePortalViewModel UpdateAssociatedStorePrecedence(PricePortalViewModel pricePortalViewModel);
        #endregion

        #region Associate Profile

        /// <summary>
        /// Get associated profile list for price.
        /// </summary>
        /// <param name="filters">Filters for Profile Price.</param>
        /// <param name="sorts">Sorts for for Profile Price.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Profile Price.</returns>
        PriceProfileListViewModel GetAssociatedProfileList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get list of unassociated profiles.
        /// </summary>
        /// <param name="filters">Filters for Profile.</param>
        /// <param name="sorts">Sorts for for Profile.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="recordPerPage">Records per page.</param>
        /// <returns>Returns list of unassociated Profile list.</returns>
        ProfileListViewModel GetUnAssociatedProfileList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Associate price to profile.
        /// </summary>
        /// <param name="priceListId">Id of price list.</param>
        /// <param name="storeIds">Profile Ids to be associated.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Returns true if profile associated successfully else return false.</returns>
        bool AssociateProfile(int priceListId, string profileIds, out string message);

        /// <summary>
        /// Remove associated profiles from price.
        /// </summary>
        /// <param name="priceListPortalIds">Price List Profile Ids.</param>
        /// <param name="priceListId">Price List Id.</param>
        /// <returns>Returns true if store unassociated successfully else return false.</returns>
        bool RemoveAssociatedProfiles(string priceListProfileIds, int priceListId);

        /// <summary>
        /// Get associated profile precedence value.
        /// </summary>
        /// <param name="priceListProfileId">Price list profile id.</param>
        /// <returns>Returns PriceProfileViewModel.</returns>
        PriceProfileViewModel GetAssociatedProfilePrecedence(int priceListProfileId);

        /// <summary>
        /// Update associated profile precedence value.
        /// </summary>
        /// <param name="priceProfileViewModel">PriceProfileViewModel model.</param>
        /// <returns>Returns updated PriceAccountViewModel.</returns>
        PriceProfileViewModel UpdateAssociatedProfilePrecedence(PriceProfileViewModel priceProfileViewModel);
        #endregion

        #region Associate Customer
        /// <summary>
        /// Get associated customer list for price.
        /// </summary>
        /// <param name="filters">Filters for Customer Price.</param>
        /// <param name="sorts">Sorts for for Customer Price.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Customer Price.</returns>
        PriceUserListViewModel GetAssociatedCustomerList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get list of unassociated customer.
        /// </summary>
        /// <param name="filters">Filters for Customer.</param>
        /// <param name="sorts">Sorts for for Customer.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="recordPerPage">Records per page.</param>
        /// <returns>Returns list of unassociated customer list.</returns>
        PriceUserListViewModel GetUnAssociatedCustomerList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Associate price to customer.
        /// </summary>
        /// <param name="priceListId">Id of price list.</param>
        /// <param name="customerIds">Customer Ids to be associated.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Returns true if Customer associated successfully else return false.</returns>
        bool AssociateCustomer(int priceListId, string customerIds, out string message);

        /// <summary>
        /// Delete customer Price on the basis of customerIds.
        /// </summary>
        /// <param name="customerIds">customerIds.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteAssociatedCustomer(string customerIds, out string message);

        /// <summary>
        /// Get associated customers precedence value.
        /// </summary>
        /// <param name="priceListUserId">Price list user id.</param>
        /// <returns>Returns PriceUserViewModel.</returns>
        PriceUserViewModel GetAssociatedCustomerPrecedence(int priceListUserId);

        /// <summary>
        /// Returns the column List.
        /// </summary>
        /// <returns></returns>
        List<BaseDropDownList> GetCustomColumnList();

        /// <summary>
        /// Update associated customers precedence value.
        /// </summary>
        /// <param name="pricePortalModel">PriceAccountViewModel model.</param>
        /// <returns>Returns updated PriceUserViewModel.</returns>
        PriceUserViewModel UpdateAssociatedCustomerPrecedence(PriceUserViewModel priceAccountViewModel);
        #endregion

        #region Associate Account
        /// <summary>
        /// Get associated account list for price.
        /// </summary>
        /// <param name="filters">Filters for Account Price.</param>
        /// <param name="sorts">Sorts for for Account Price.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Account Price.</returns>
        PriceAccountListViewModel GetAssociatedAccountList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get list of unassociated account.
        /// </summary>
        /// <param name="filters">Filters for Account.</param>
        /// <param name="sorts">Sorts for for Account.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="recordPerPage">Records per page.</param>
        /// <returns>Returns list of unassociated account list.</returns>
        PriceAccountListViewModel GetUnAssociatedAccountList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Associate price to account.
        /// </summary>
        /// <param name="priceListId">Id of price list.</param>
        /// <param name="accountIds">Account Ids to be associated.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Returns true if Account associated successfully else return false.</returns>
        bool AssociateAccount(int priceListId, string accountIds, out string message);

        /// <summary>
        /// Remove associated accounts from price.
        /// </summary>
        /// <param name="priceListPortalIds">Price List Account Ids.</param>
        /// <param name="priceListId">Price List Id.</param>
        /// <returns>Returns true if account unassociated successfully else return false.</returns>
        bool RemoveAssociatedAccounts(string priceListAccountIds, int priceListId);

        /// <summary>
        /// Get associated accounts precedence value.
        /// </summary>
        /// <param name="priceListUserId">Price list user id.</param>
        /// <returns>Returns PriceAccountViewModel.</returns>
        PriceAccountViewModel GetAssociatedAccountPrecedence(int priceListUserId);

        /// <summary>
        /// Update associated accounts precedence value.
        /// </summary>
        /// <param name="pricePortalModel">PriceAccountViewModel model.</param>
        /// <returns>Returns updated PriceAccountViewModel.</returns>
        PriceAccountViewModel UpdateAssociatedAccountPrecedence(PriceAccountViewModel priceAccountViewModel);
        #endregion

        #region Price Management

        /// <summary>
        /// Get unassociated price list.
        /// </summary>
        /// <param name="expands">Expands for Price List.</param>
        /// <param name="filters">Filters for Price List.</param>
        /// <param name="sorts">Sorts for Price List.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="recordPerPage">Size of page.</param>
        /// <returns>Returns unassociated price list.</returns>
        PriceListViewModel GetUnAssociatedPriceList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Associate pricelist to store.
        /// </summary>
        /// <param name="storeId">Store Id to which pricelist to be associated.</param>
        /// <param name="priceListIds">priceListIds to be associated.</param>
        /// <returns>Returns true if pricelist associated successfully else return false.</returns>
        bool AssociatePriceListToStore(int storeId, string priceListId);

        /// <summary>
        /// Associate pricelist to profile.
        /// </summary>
        /// <param name="profileId">profileId to which pricelist to be associated.</param>
        /// <param name="priceListIds">priceListIds to be associated.</param>
        /// <param name="portalId">portalId.</param>       
        /// <returns>Returns true if pricelist associated successfully else return false.</returns>
        bool AssociatePriceListToProfile(int profileId, string priceListId, int portalId);

        /// <summary>
        /// Remove associated price list from Store.
        /// </summary>
        /// <param name="model">Model containing priceListPortalIds.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool RemoveAssociatedPriceListToStore(string priceListPortalId);

        /// <summary>
        /// Remove associated price list from Profile.
        /// </summary>
        /// <param name="priceListProfileIds">priceListProfileIds.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool RemoveAssociatedPriceListToProfile(string priceListProfileIds);

        /// <summary>
        /// Get associated price list precedence value for Store/Profile.
        /// </summary>
        /// <param name="priceListProfileId">priceListProfileId to get precedence for Profile.</param>
        /// <param name="priceListId">priceListId to get precedence for Store.</param>
        /// <param name="portalId">portalId to get precedence for Store.</param>
        /// <param name="listName">price list name.</param>
        /// <returns>Returns PricePortalViewModel.</returns>
        PricePortalViewModel GetAssociatedPriceListPrecedence(int priceListProfileId, int priceListId, int portalId, string listName);

        /// <summary>
        /// Update associated price list precedence value for Store/Profile.
        /// </summary>
        /// <param name="pricePortalViewModel">PricePortalViewModel model.</param>
        /// <returns>Returns true/false status.</returns>
        bool UpdateAssociatedPriceListPrecedence(string data);
        #endregion

        #region Import/Export
        /// <summary>
        /// Get the list of price data which is going to import.
        /// </summary>
        /// <param name="file">Excel file containing price data to import.</param>
        /// <returns>List of ImportPriceViewModel.</returns>
        List<ImportPriceViewModel> PreviewImportData(HttpPostedFileBase file);

        /// <summary>
        /// Get pricing data to export
        /// </summary>
        /// <param name="priceListIds">ID's of priceList</param>
        /// <returns>List of Export Price View Model.</returns>
        List<ExportPriceViewModel> ExportPriceData(string priceListIds);
        #endregion

        /// <summary>
        /// Set Import Invertory Details.
        /// </summary>
        /// <param name="priceSKUDataViewModel">PriceViewModel</param>
        void SetImportPricingDetails(PriceViewModel priceSKUDataViewModel);
       
    }
}
