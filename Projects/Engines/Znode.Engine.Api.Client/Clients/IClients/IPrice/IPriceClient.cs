using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IPriceClient : IBaseClient
    {
        /// <summary>
        /// Create Price.
        /// </summary>
        /// <param name="priceModel">Price Model.</param>
        /// <returns>Returns created Price Model.</returns>
        PriceModel CreatePrice(PriceModel priceModel);

        /// <summary>
        /// Get Price on the basis of Price list id.
        /// </summary>
        /// <param name="priceListId">Price List Id to get Price details.</param>
        /// <returns>Returns PriceModel.</returns>
        PriceModel GetPrice(int priceListId);

        /// <summary>
        /// Update Price data.
        /// </summary>
        /// <param name="priceModel">Price model to update.</param>
        /// <returns>Returns updated Price model.</returns>
        PriceModel UpdatePrice(PriceModel priceModel);

        /// <summary>
        /// Get list for Price.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Price list.</param>
        /// <param name="filters">Filters to be applied on Price list.</param>
        /// <param name="sorts">Sorting to be applied on Price list.</param>
        /// <returns>Returns Price list.</returns>
        PriceListModel GetPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get list for Price.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Price list.</param>
        /// <param name="filters">Filters to be applied on Price list.</param>
        /// <param name="sorts">Sorting to be applied on Price list.</param>
        /// <param name="pageIndex">Start page index of Price list.</param>
        /// <param name="pageSize">Page size of Price list.</param>
        /// <returns>Returns Price list.</returns>
        PriceListModel GetPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Delete price.
        /// </summary>
        /// <param name="priceListId">Price List Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeletePrice(ParameterModel priceListId);

        /// <summary>
        /// Copy Price based on priceListId
        /// </summary>
        /// <param name="priceModel">Price model to be Copied.</param>
        /// <returns>Return true or false</returns>
        bool CopyPrice(PriceModel priceModel);

        #region SKU Price.
        /// <summary>
        /// Get SKU Price list.
        /// </summary>
        /// <param name="expands">Expands for Associated Item.</param>
        /// <param name="filters">Filters for Associated Item.</param>
        /// <param name="sorts">Sorts for Associated Item.</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>PriceSKUListModel</returns>
        PriceSKUListModel GetSKUPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

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
        /// <returns>Returns updated SKU Price.</returns>
        PriceSKUModel UpdateSKUPrice(PriceSKUModel priceSKUModel);

        /// <summary>
        /// Delete existing SKU Price.
        /// </summary>
        /// <param name="model">Model containing Price List Id and multiple Price SKU Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteSKUPrice(SKUPriceDeleteModel model);

        /// <summary>
        /// Get price by sku
        /// </summary>
        /// <param name="filters">Filters for price.</param>
        /// <returns>PriceSKUModel</returns>
        PriceSKUModel GetPriceBySku(FilterCollection filters);

        /// <summary>
        /// Get price details by sku
        /// </summary>
        /// <param name="productPriceListSKUDataModel"></param>
        /// <returns></returns>
        PriceSKUModel GetProductPricingDetailsBySku(ProductPriceListSKUDataModel productPriceListSKUDataModel);

        /// <summary>
        /// Get Uom List.
        /// </summary>
        /// <param name="expands">Expands for Uom.</param>
        /// <param name="filters">Filters for Uom.</param>
        /// <param name="sorts">Sorts for Uom.</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>model</returns>
        UomListModel GetUomList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get paged SKU Price list.
        /// </summary>
        /// <param name="expands">Expands for Associated Item.</param>
        /// <param name="filters">Filters for Associated Item.</param>
        /// <param name="sorts">Sorts for Associated Item.</param>
        /// <param name="pageIndex">Start Page Index.</param>
        /// <param name="pageSize">Page Size.</param>
        /// <returns>Returns SKU and its prices on the basis of catalogId.</returns>
        PriceSKUListModel GetPagedSkuPrice(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
        #endregion

        #region Tier Price
        /// <summary>
        /// Get Tier Price list.
        /// </summary>
        /// <param name="expands">Expands for Tier Price.</param>
        /// <param name="filters">Filters for Tier Price.</param>
        /// <param name="sorts">Sorts for Tier Price.</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>PriceTierListModel</returns>
        PriceTierListModel GetTierPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Save tier price values.
        /// </summary>
        /// <param name="priceTierListModel">PriceTierListModel.</param>
        /// <returns>Returns true if tier price saved successfully else return false.</returns>
        bool AddTierPrice(PriceTierListModel priceTierListModel);

        /// <summary>
        /// Get Tier Price by PriceId.
        /// </summary>
        /// <param name="priceTierId">PriceTierId.</param>
        /// <returns>PriceTierModel</returns>
        PriceTierModel GetTierPrice(int priceTierId);

        /// <summary>
        /// Update Tier Price.
        /// </summary>
        /// <param name="priceTierModel">PriceTierModel to update existing Tier Price.</param>
        /// <returns>Returns updated Tier Price.</returns>
        PriceTierModel UpdateTierPrice(PriceTierModel priceTierModel);

        /// <summary>
        /// Delete existing Tier Price.
        /// </summary>
        /// <param name="priceTierId">PriceTierId to delete Tier Price.</param>
        /// <returns>true if deleted.</returns>
        bool DeleteTierPrice(int priceTierId);
        #endregion

        #region Associate Store

        /// <summary>
        /// Get associated store list for price.
        /// </summary>
        /// <param name="expands">Expands for Portal Price.</param>
        /// <param name="filters">Filters for Portal Price.</param>
        /// <param name="sorts">Sorts for for Portal Price.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Portal Price.</returns>
        PricePortalListModel GetAssociatedStoreList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get list of unassociated stores.
        /// </summary>
        /// <param name="expands">Expands for Portal.</param>
        /// <param name="filters">Filters for Portal.</param>
        /// <param name="sorts">Sorts for for Portal.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of unassociated store list.</returns>
        PortalListModel GetUnAssociatedStoreList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate price to store.
        /// </summary>
        /// <param name="pricePortalModelListModel">PricePortalListModel</param>
        /// <returns>Returns true if store associated successfully else return false.</returns>
        bool AssociateStore(PricePortalListModel pricePortalModelListModel);

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
        PricePortalModel GetAssociatedStorePrecedence(int priceListPortalId, ExpandCollection expands);

        /// <summary>
        /// Update associated stores precedence value.
        /// </summary>
        /// <param name="pricePortalModel">PricePortalModel model.</param>
        /// <returns>Returns updated PricePortalModel.</returns>
        PricePortalModel UpdateAssociatedStorePrecedence(PricePortalModel pricePortalModel);
        #endregion

        #region Associate Profile

        /// <summary>
        /// Get associated profile list for price.
        /// </summary>
        /// <param name="expands">Expands for Profile Price.</param>
        /// <param name="filters">Filters for Profile Price.</param>
        /// <param name="sorts">Sorts for for Profile Price.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Profile Price.</returns>
        PriceProfileListModel GetAssociatedProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get list of unassociated Profiles.
        /// </summary>
        /// <param name="expands">Expands for Profile.</param>
        /// <param name="filters">Filters for Profile.</param>
        /// <param name="sorts">Sorts for for Profile.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of unassociated profile list.</returns>
        ProfileListModel GetUnAssociatedProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate price to profile.
        /// </summary>
        /// <param name="priceProfileModelListModel">PriceProfileListModel</param>
        /// <returns>Returns true if store associated successfully else return false.</returns>
        bool AssociateProfile(PriceProfileListModel priceProfileModelListModel);

        /// <summary>
        /// Remove associated profiles from price.
        /// </summary>
        /// <param name="model">RemoveAssociatedProfilesModel.</param>
        /// <returns>Returns true if profiles unassociated successfully else return false.</returns>
        bool RemoveAssociatedProfiles(RemoveAssociatedProfilesModel model);

        /// <summary>
        /// Get associated profiles precedence value.
        /// </summary>
        /// <param name="priceListProfileId">Price list Profile id to get the data.</param>
        /// <param name="expands">Expands for getting Profile name.</param>
        /// <returns>Returns PriceProfileModel.</returns>
        PriceProfileModel GetAssociatedProfilePrecedence(int priceListProfileId, ExpandCollection expands);

        /// <summary>
        /// Update associated Profiles precedence value.
        /// </summary>
        /// <param name="priceProfileModel">PriceProfileModel model.</param>
        /// <returns>Returns updated PriceProfileModel.</returns>
        PriceProfileModel UpdateAssociatedProfilePrecedence(PriceProfileModel priceProfileModel);
        #endregion

        #region Associate Customer
        /// <summary>
        /// Get associated Customer list for price.
        /// </summary>
        /// <param name="expands">Expands for Customer Price.</param>
        /// <param name="filters">Filters for Customer Price.</param>
        /// <param name="sorts">Sorts for for Customer Price.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Customer Price.</returns>
        PriceUserListModel GetAssociatedCustomerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get list of unassociated Customer.
        /// </summary>
        /// <param name="expands">Expands for Customer.</param>
        /// <param name="filters">Filters for Customer.</param>
        /// <param name="sorts">Sorts for for Customer.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of unassociated Customer list.</returns>
        PriceUserListModel GetUnAssociatedCustomerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate price to Customer.
        /// </summary>
        /// <param name="priceAccountModelListModel">List<PriceAccountModel></param>
        /// <returns>Returns true if Customer associated successfully else return false.</returns>
        bool AssociateCustomer(PriceUserListModel priceAccountModelListModel);

        /// <summary>
        /// Delete Customer.
        /// </summary>
        /// <param name="customerIds">customerIds.</param>
        /// <returns>Returns true if AssociateCustomer deleted successfully else return false.</returns>
        bool DeleteAssociatedCustomer(ParameterModel customerIds);

        /// <summary>
        /// Get associated customers precedence value.
        /// </summary>
        /// <param name="priceListUserId">Price list user id to get the data.</param>
        /// <param name="expands">Expands for getting customer name.</param>
        /// <returns>Returns PriceUserModel.</returns>
        PriceUserModel GetAssociatedCustomerPrecedence(int priceListUserId, ExpandCollection expands);

        /// <summary>
        /// Update associated customers precedence value.
        /// </summary>
        /// <param name="priceUserModel">PriceAccountModel model.</param>
        /// <returns>Returns updated PriceAccountModel.</returns>
        PriceUserModel UpdateAssociatedCustomerPrecedence(PriceUserModel priceUserModel);
        #endregion

        #region Associate Account
        /// <summary>
        /// Get associated Account list for price.
        /// </summary>
        /// <param name="expands">Expands for Account Price.</param>
        /// <param name="filters">Filters for Account Price.</param>
        /// <param name="sorts">Sorts for for Account Price.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Account Price.</returns>
        PriceAccountListModel GetAssociatedAccountList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get list of unassociated Account.
        /// </summary>
        /// <param name="expands">Expands for Account.</param>
        /// <param name="filters">Filters for Account.</param>
        /// <param name="sorts">Sorts for for Account.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of unassociated Account list.</returns>
        PriceAccountListModel GetUnAssociatedAccountList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate price to Account.
        /// </summary>
        /// <param name="priceAccountModelListModel">List<PriceAccountModel></param>
        /// <returns>Returns true if Account associated successfully else return false.</returns>
        bool AssociateAccount(PriceAccountListModel priceAccountModelListModel);

        /// <summary>
        /// Remove associated accounts from price.
        /// </summary>
        /// <param name="model">RemoveAssociatedAccountsModel.</param>
        /// <returns>Returns true if accounts unassociated successfully else return false.</returns>
        bool RemoveAssociatedAccounts(RemoveAssociatedAccountsModel model);

        /// <summary>
        /// Get associated account precedence value.
        /// </summary>
        /// <param name="priceListUserId">Price list user id to get the data.</param>
        /// <param name="expands">Expands for getting account name.</param>
        /// <returns>Returns PriceAccountModel.</returns>
        PriceAccountModel GetAssociatedAccountPrecedence(int priceListUserId, ExpandCollection expands);

        /// <summary>
        /// Update associated account precedence value.
        /// </summary>
        /// <param name="priceAccountModel">PriceAccountModel model.</param>
        /// <returns>Returns updated PriceAccountModel.</returns>
        PriceAccountModel UpdateAssociatedAccountPrecedence(PriceAccountModel priceAccountModel);
        #endregion

        #region Price Management
        /// <summary>
        /// Get unassociated price list.
        /// </summary>
        /// <param name="expands">Expands for Price List.</param>
        /// <param name="filters">Filters for Price List.</param>
        /// <param name="sorts">Sorts for Price List.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns unassociated price list.</returns>
        PriceListModel GetUnAssociatedPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

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
        /// Get associated price list precedence value for Store/Profile.
        /// </summary>
        /// <param name="pricePortalModel">pricePortalModel contains price list id and portal id /profile id to get precedence.</param>
        /// <returns>Returns PricePortalModel.</returns>
        PricePortalModel GetAssociatedPriceListPrecedence(PricePortalModel pricePortalModel);

        /// <summary>
        /// Update associated price list precedence value Store/Profile.
        /// </summary>
        /// <param name="pricePortalModel">PricePortalModel.</param>
        /// <returns>Returns updated PricePortalModel.</returns>
        PricePortalModel UpdateAssociatedPriceListPrecedence(PricePortalModel pricePortalModel);
        #endregion

        /// <summary>
        /// Get pricing data to export
        /// </summary>
        /// <param name="priceListIds">ID's of priceList</param>
        /// <returns>List of Export Price Model.</returns>
        List<ExportPriceModel> GetExportPriceData(string priceListIds);
    }
}
