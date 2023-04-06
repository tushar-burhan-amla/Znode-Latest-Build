using Znode.Engine.Api.Models;
namespace Znode.Engine.Api.Cache
{
    public interface IPriceCache
    {
        #region Price
        /// <summary>
        /// Get Price on the basis of Price list id.
        /// </summary>
        /// <param name="priceListId">Price list id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Price.</returns>
        string GetPrice(int priceListId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Price list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Price list.</returns>
        string GetPriceList(string routeUri, string routeTemplate); 
        #endregion

        #region SKU Price
        /// <summary>
        /// Get SKU Price from Cache By PriceId.
        /// </summary>
        /// <param name="priceId">PriceId.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns SKU Price on the basis of PriceId.</returns>
        string GetSKUPrice(int priceId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get SKU Price list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of SKU Price.</returns>
        string GetSKUPriceList(string routeUri, string routeTemplate);

        /// <summary>
        ///  Get Unit of Measurement List.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Uom.</returns>
        string GetUomList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get SKU Price list By CatalogId
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of SKU Price on the basis of CatalogId.</returns>
        string GetPagedPriceSKU(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Price from Cache By sku.
        /// </summary>
        /// <param name="sku">sku.</param>
        /// <param name="productType">ProductType</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns SKU Price on the basis of PriceId.</returns>
        string GetPriceBySku(string routeUri, string routeTemplate);

        /// <summary>
        /// Get pricing details by product price model.
        /// </summary>
        /// <param name="productPriceListSKUDataModel"></param>
        /// <returns></returns>
        string GetProductPricingDetailsBySku(ProductPriceListSKUDataModel productPriceListSKUDataModel);

        #endregion

        #region Tier Price
        /// <summary>
        /// Get Tier Price from Cache By PriceTierId.
        /// </summary>
        /// <param name="priceTierId">priceTierId.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns SKU Price on the basis of PriceId.</returns>
        string GetTierPrice(int priceTierId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Tier Price list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Tier Price.</returns>
        string GetTierPriceList(string routeUri, string routeTemplate);
        #endregion

        #region Associate Store
        /// <summary>
        /// Get associated store list for price.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Portal Price.</returns>
        string GetAssociatedStoreList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of unassociated stores.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of unassociated store list.</returns>
        string GetUnAssociatedStoreList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get associated stores precedence value.
        /// </summary>
        /// <param name="priceListPortalId">Price list portal id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns PricePortalModel.</returns>
        string GetAssociatedStorePrecedence(int priceListPortalId, string routeUri, string routeTemplate);
        #endregion

        #region Associate Profile
        /// <summary>
        /// Get associated profile list for price.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Profile Price.</returns>
        string GetAssociatedProfileList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of unassociated profiles.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of unassociated profile list.</returns>
        string GetUnAssociatedProfileList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get associated profile precedence value.
        /// </summary>
        /// <param name="priceListProfileId">Price list profile id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns PriceProfileModel.</returns>
        string GetAssociatedProfilePrecedence(int priceListProfileId, string routeUri, string routeTemplate);
        #endregion

        #region Associate Customer
        /// <summary>
        /// Get associated customer list for price.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Customer Price.</returns>
        string GetAssociatedCustomerList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of unassociated customer.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of unassociated customer list.</returns>
        string GetUnAssociatedCustomerList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get associated customers precedence value.
        /// </summary>
        /// <param name="priceListUserId">Price list user id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns PriceAccountModel.</returns>
        string GetAssociatedCustomerPrecedence(int priceListUserId, string routeUri, string routeTemplate);
        #endregion

        #region Associate Account
        /// <summary>
        /// Get associated account list for price.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of Account Price.</returns>
        string GetAssociatedAccountList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of unassociated account.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of unassociated account list.</returns>
        string GetUnAssociatedAccountList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get associated account precedence value.
        /// </summary>
        /// <param name="priceListUserId">Price list user id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns PriceAccountModel.</returns>
        string GetAssociatedAccountPrecedence(int priceListUserId, string routeUri, string routeTemplate);
        #endregion

        #region Price Management
        /// <summary>
        /// Get unassociated price list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of unassociated price list.</returns>
        string GetUnAssociatedPriceList(string routeUri, string routeTemplate);
        #endregion
    }
}
