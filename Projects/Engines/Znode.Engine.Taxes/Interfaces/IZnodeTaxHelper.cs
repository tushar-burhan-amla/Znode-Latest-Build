using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Taxes
{
    public interface IZnodeTaxHelper
    {
        /// <summary>
        /// Get the value for Taxes In Product Price.
        /// </summary>
        bool IsIncludePortalLevelTax(string storeFeature);

        /// <summary>
        /// Get inclusive tax rules.
        /// </summary>
        List<TaxRuleModel> GetInclusiveTaxRules();

        /// <summary>
        /// Get country list.
        /// </summary>
        List<CityModel> GetTaxRuleByCountryFIPSCityStateCodePostalCode(AddressModel address, TaxRuleModel taxRule);

        /// <summary>
        /// Ger user Id from current request headers.
        /// </summary>
        int GetLoginUserId();

        /// <summary>
        /// Get profile model on basis of user id from cache.
        /// </summary>
        ProfileModel GetProfileCache();

        List<TaxRuleModel> GetTaxRuleListByPortalId(AddressModel shippingAddress, int portalId, int? profileId, int? userId = 0, int? orderId = 0);

        /// <summary>
        /// Get portal shipping from address.
        /// </summary>
        AddressModel GetPortalShippingAddress(int portalId);

        /// <summary>
        /// Get tax rule list.
        /// </summary>
        List<TaxRuleModel> GetTaxRuleList(string countryCode, string stateCode, int? portalId, int? profileId = 0, int? userId = 0, int? orderId = 0);

        /// <summary>
        /// Get shipping origin address on the basis of portal id.
        /// </summary>
        AddressModel GetPortalShippingOriginAddress(int portalId);

        /// <summary>
        /// Get tax on the basis of portal id.
        /// </summary>
        TaxPortalModel GetTaxPortalData(int portalId);

        /// <summary>
        /// Get ware house default address on the basis of portal id.
        /// </summary>
        AddressModel GetPortalWareHouseAddress(int portalId);

        /// <summary>
        /// Get tax transaction number.
        /// </summary>
        string GetTaxTransactionNumber(int? orderId);

        

        /// <summary>
        /// to check user is guest user or registered
        /// </summary>
        bool IsGuestUser(int userId);

        /// <summary>
        /// Get the currency.
        /// </summary>
        string GetCulture(int currentStore);
    }
}
