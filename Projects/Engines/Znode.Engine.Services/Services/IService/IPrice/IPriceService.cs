using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPriceService
    {
        #region Price
        /// <summary>
        /// Create Price.
        /// </summary>
        /// <param name="priceModel">Price Model.</param>
        /// <returns>Returns created Price Model.</returns>
        PriceModel CreatePrice(PriceModel priceModel);

        /// <summary>
        /// Get Price   on the basis of Price list id.
        /// </summary>
        /// <param name="priceListId">PriceListId.</param>
        /// <returns>Returns Price Model.</returns>
        PriceModel GetPrice(int priceListId);

        /// <summary>
        /// Update Price data.
        /// </summary>
        /// <param name="priceModel">Price model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdatePrice(PriceModel priceModel);

        /// <summary>
        /// Get list for Price.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Price list.</param>
        /// <param name="filters">Filters to be applied on Price list.</param>
        /// <param name="sorts">Sorting to be applied on Price list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of Price.</returns>
        PriceListModel GetPriceList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete price.
        /// </summary>
        /// <param name="priceListIds">Price List Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeletePrice(ParameterModel priceListIds);

        /// <summary>
        /// Copy a price.
        /// </summary>
        /// <param name="model">Price model to be Copied</param>
        /// <returns>Return true or false.</returns>
        bool CopyPrice(PriceModel model);
        #endregion

        #region SKU Price.
        /// <summary>
        /// Get SKU Price list.
        /// </summary>
        /// <param name="expands">Expands for Associated Item.</param>
        /// <param name="filters">Filters for Associated Item.</param>
        /// <param name="sorts">Sorts for Associated Item.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>PriceSKUListModel</returns>
        PriceSKUListModel GetSKUPriceList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get SKU Price by PriceId.
        /// </summary>
        /// <param name="priceId">PriceId.</param>
        /// <returns>PriceSKUModel</returns>
        PriceSKUModel GetSKUPrice(int priceId);

        /// <summary>
        /// Add SKU Price. 
        /// </summary>
        /// <param name="priceSKUModel">PriceSKUModel to add SKU Price.</param>
        /// <returns>Added SKU Price.</returns>
        PriceSKUModel AddSKUPrice(PriceSKUModel priceSKUModel);

        /// <summary>
        /// Update SKU Price.
        /// </summary>
        /// <param name="priceSKUModel">PriceSKUModel to update existing SKU Price.</param>
        /// <returns>Returns true if updated.</returns>
        bool UpdateSKUPrice(PriceSKUModel priceSKUModel);

        /// <summary>
        /// Delete existing SKU Price.
        /// </summary>
        /// <param name="model">Model containing Price List Id and multiple Price SKU Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteSKUPrice(SKUPriceDeleteModel model);

        /// <summary>
        /// Get Price by sku.
        /// </summary>
        /// <param name="filters">Filters for price.</param>
        /// <returns>PriceSKUModel</returns>
        PriceSKUModel GetPriceBySku(FilterCollection filters);

        /// <summary>
        /// Get pricing details by product price model
        /// </summary>
        /// <param name="productPriceListSKUDataModel"></param>
        /// <returns></returns>
        PriceSKUModel GetProductPricingDetailsBySku(ProductPriceListSKUDataModel productPriceListSKUDataModel);

        /// <summary>
        /// Get Unit of Measurement List.
        /// </summary>
        /// <param name="expands">Expands for Unit of Measurement List.</param>
        /// <param name="filters">Filters for Unit of Measurement List.</param>
        /// <param name="sorts">Sorts for Unit of Measurement List.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>list</returns>
        UomListModel GetUomList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get SKU Price list by CatalogId.
        /// </summary>
        /// <param name="priceSKUListModel"></param>
        /// <returns>Returns SKU and its Prices on the basis of catalog Id</returns>
        PriceSKUListModel GetPagedPriceSKU(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, DataTable productIdTable, string isInStock = "-1");
        #endregion

        #region Tier Price.
        /// <summary>
        /// Get Tier Price list.
        /// </summary>
        /// <param name="expands">Expands for Tier Price.</param>
        /// <param name="filters">Filters for Tier Price.</param>
        /// <param name="sorts">Sorts for Tier Price.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>PriceTierListModel</returns>
        PriceTierListModel GetTierPriceList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Save Tier Price. 
        /// </summary>
        /// <param name="priceTierModel">PriceTierListModel to save Tier Price.</param>
        /// <returns>Returns true if tier price saved successfully else return false.</returns>
        bool AddTierPrice(PriceTierListModel priceTierListModel);

        /// <summary>
        /// Get Tier Price by priceTierId.
        /// </summary>
        /// <param name="priceTierId">priceTierId.</param>
        /// <returns>PriceTierModel</returns>
        PriceTierModel GetTierPrice(int priceTierId);

        /// <summary>
        /// Update Tier Price.
        /// </summary>
        /// <param name="priceTierModel">PriceTierModel to update existing Tier Price.</param>
        /// <returns>Returns true if updated.</returns>
        bool UpdateTierPrice(PriceTierModel priceTierModel);

        /// <summary>
        /// Delete existing Tier Price.
        /// </summary>
        /// <param name="priceTierId">PriceId to delete Tier Price.</param>
        /// <returns>true if deleted.</returns>
        bool DeleteTierPrice(int priceTierId);
        #endregion

        #region Associate Store

        /// <summary>
        ///  Get associated store list for price.
        /// </summary>
        /// <param name="expands">Expands for Portal Price.</param>
        /// <param name="filters">Filters for Portal Price.</param>
        /// <param name="sorts">Sorts for for Portal Price.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of Portal Price.</returns>
        PricePortalListModel GetAssociatedStoreList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of unassociated stores.
        /// </summary>
        /// <param name="filters">Filters for Portal.</param>
        /// <param name="sorts">Sorts for for Portal.</param> <param name="page"></param>
        /// <returns>Returns list of unassociated store list.</returns>
        PortalListModel GetUnAssociatedStoreList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate store to price.
        /// </summary>
        /// <param name="pricePortalModel">PricePortalListModel</param>
        /// <returns>Returns true if store associated successfully else return false.</returns>
        bool AssociateStore(PricePortalListModel pricePortalModel);

        /// <summary>
        /// Remove associated stores from price.
        /// </summary>
        /// <param name="model">RemoveAssociatedStoresModel.</param>
        /// <returns>Returns true if stores unassociated successfully else return false.</returns>
        bool RemoveAssociatedStores(RemoveAssociatedStoresModel model);

        /// <summary>
        /// Get associated stores precedence value.
        /// </summary>
        /// <param name="priceListPortalId">Price list portal id.</param>
        /// <param name="expands">Expands for getting store name.</param>
        /// <returns>Returns PricePortalModel.</returns>
        PricePortalModel GetAssociatedStorePrecedence(int priceListPortalId, NameValueCollection expands);

        /// <summary>
        /// Update associated stores precedence value.
        /// </summary>
        /// <param name="pricePortalModel">PricePortalModel model.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateAssociatedStorePrecedence(PricePortalModel pricePortalModel);
        #endregion

        #region Associate Profile

        /// <summary>
        ///  Get associated profile list for price.
        /// </summary>
        /// <param name="expands">Expands for Profile Price.</param>
        /// <param name="filters">Filters for Profile Price.</param>
        /// <param name="sorts">Sorts for for Profile Price.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of Profile Price.</returns>
        PriceProfileListModel GetAssociatedProfileList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of unassociated profiles.
        /// </summary>
        /// <param name="filters">Filters for Profile.</param>
        /// <param name="sorts">Sorts for for Profile.</param> <param name="page"></param>
        /// <returns>Returns list of unassociated profile list.</returns>
        ProfileListModel GetUnAssociatedProfileList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate profile to price.
        /// </summary>
        /// <param name="priceProfileModel">PriceProfileListModel.</param>
        /// <returns>Returns true if profile associated successfully else return false.</returns>
        bool AssociateProfile(PriceProfileListModel priceProfileModel);

        /// <summary>
        /// Remove associated profiles from price.
        /// </summary>
        /// <param name="model">RemoveAssociatedProfilesModel.</param>
        /// <returns>Returns true if profiles unassociated successfully else return false.</returns>
        bool RemoveAssociatedProfiles(RemoveAssociatedProfilesModel model);

        /// <summary>
        /// Get associated profile precedence value.
        /// </summary>
        /// <param name="priceListProfileId">Price list profile id.</param>
        /// <param name="expands">Expands for getting profile name.</param>
        /// <returns>Returns PriceProfileModel.</returns>
        PriceProfileModel GetAssociatedProfilePrecedence(int priceListProfileId, NameValueCollection expands);

        /// <summary>
        /// Update associated profile precedence value.
        /// </summary>
        /// <param name="priceProfileModel">PriceProfileModel model.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateAssociatedProfilePrecedence(PriceProfileModel priceProfileModel);
        #endregion

        #region Associate Customer
        /// <summary>
        ///  Get associated customer list for price.
        /// </summary>
        /// <param name="expands">Expands for Customer Price.</param>
        /// <param name="filters">Filters for Customer Price.</param>
        /// <param name="sorts">Sorts for for Customer Price.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of Customer Price.</returns>
        PriceUserListModel GetAssociatedCustomerList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of unassociated customer.
        /// </summary>
        /// <param name="filters">Filters for Customer.</param>
        /// <param name="sorts">Sorts for for Customer.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of unassociated Customer list.</returns>
        PriceUserListModel GetUnAssociatedCustomerList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate customer to price.
        /// </summary>
        /// <param name="priceCustomerModel">List<PriceAccountModel>.</param>
        /// <returns>Returns true if customer associated successfully else return false.</returns>
        bool AssociateCustomer(PriceUserListModel priceCustomerModel);

        /// <summary>
        /// Delete Associated customer.
        /// </summary>
        /// <param name="customerIds">customerIds.</param>
        /// <returns>Returns true if associated customer deleted successfully else return false.</returns>
        bool DeleteAssociatedCustomer(ParameterModel customerIds);

        /// <summary>
        /// Get associated customers precedence value.
        /// </summary>
        /// <param name="priceListUserId">Price list user id.</param>
        /// <param name="expands">Expands for getting customer name.</param>
        /// <returns>Returns PriceUserModel.</returns>
        PriceUserModel GetAssociatedCustomerPrecedence(int priceListUserId, NameValueCollection expands);

        /// <summary>
        /// Update associated customers precedence value.
        /// </summary>
        /// <param name="priceAccountModel">PriceAccountModel model.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateAssociatedCustomerPrecedence(PriceUserModel priceAccountModel);
        #endregion

        #region Associate Account
        /// <summary>
        ///  Get associated account list for price.
        /// </summary>
        /// <param name="expands">Expands for Account Price.</param>
        /// <param name="filters">Filters for Account Price.</param>
        /// <param name="sorts">Sorts for for Account Price.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of Account associated to Price.</returns>
        PriceAccountListModel GetAssociatedAccountList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of unassociated account to price.
        /// </summary>
        /// <param name="filters">Filters for Account.</param>
        /// <param name="sorts">Sorts for for Account.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of unassociated Account list.</returns>
        PriceAccountListModel GetUnAssociatedAccountList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate account to price list.
        /// </summary>
        /// <param name="priceAccountModel">List<PriceAccountModel>.</param>
        /// <returns>Returns true if account associated successfully else return false.</returns>
        bool AssociateAccount(PriceAccountListModel priceAccountModel);

        /// <summary>
        /// Remove associated accounts from price.
        /// </summary>
        /// <param name="model">RemoveAssociatedAccountsModel.</param>
        /// <returns>Returns true if accounts unassociated successfully else return false.</returns>
        bool RemoveAssociatedAccounts(RemoveAssociatedAccountsModel model);

        /// <summary>
        /// Get associated account precedence value.
        /// </summary>
        /// <param name="priceListUserId">Price list user id.</param>
        /// <param name="expands">Expands for getting account name.</param>
        /// <returns>Returns PriceAccountModel.</returns>
        PriceAccountModel GetAssociatedAccountPrecedence(int priceListUserId, NameValueCollection expands);

        /// <summary>
        /// Update associated account precedence value.
        /// </summary>
        /// <param name="priceAccountModel">PriceAccountModel model.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateAssociatedAccountPrecedence(PriceAccountModel priceAccountModel);
        #endregion

        #region Price Management
        /// <summary>
        /// Get unassociated price list.
        /// </summary>
        /// <param name="expands">Expands for Price List.</param>
        /// <param name="filters">Filters for Price List.</param>
        /// <param name="sorts">Sorts for Price List.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns unassociated price list.</returns>
        PriceListModel GetUnAssociatedPriceList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Remove associated price list from Store.
        /// </summary>
        /// <param name="model">Model containing price list ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool RemoveAssociatedPriceListToStore(ParameterModel model);

        /// <summary>
        /// Remove associated price list from Profile.
        /// </summary>
        /// <param name="priceListProfileIds">priceListProfileIds.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool RemoveAssociatedPriceListToProfile(ParameterModel priceListProfileIds);

        /// <summary>
        /// Get associated price list precedence value for store/profile.
        /// </summary>
        /// <param name="pricePortalModel">pricePortalModel contains priceListId and PortalId/ProfileId to get precedence value.</param>
        /// <returns>Returns PricePortalModel.</returns>
        PricePortalModel GetAssociatedPriceListPrecedence(PricePortalModel pricePortalModel);

        /// <summary>
        /// Update associated price list precedence value for store/profile.
        /// </summary>
        /// <param name="pricePortalModel">PricePortalModel model.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateAssociatedPriceListPrecedence(PricePortalModel pricePortalModel);
        #endregion

        #region Export
        /// <summary>
        /// Get pricing data to export.
        /// </summary>
        /// <param name="priceListIds">ID's of priceList.</param>
        /// <returns>List of Export Price Model.</returns>
        List<ExportPriceModel> GetExportPriceData(string priceListIds);
        #endregion
    }
}
