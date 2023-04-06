using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IProviderEngineAgent
    {
        #region TaxRule Type
        /// <summary>
        /// Get the list of tax rule type.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns ProviderEngine list ViewModel.</returns>
        ProviderEngineListViewModel GetTaxRuleTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get a tax rule type.
        /// </summary>
        /// <param name="taxRuleTypeId">ID of taxRuleType to get details of perticular TaxRuleType.</param>
        /// <returns>Returns ProviderEngine ViewModel.</returns>
        ProviderEngineViewModel GetTaxRuleType(int taxRuleTypeId);

        /// <summary>
        /// Create new tax rule type.
        /// </summary>
        /// <param name="providerEngineModel">ProviderEngine ViewModel.</param>
        /// <returns>Returns true if ProviderEngine created else returns false.</returns>
        ProviderEngineViewModel CreateTaxRuleType(ProviderEngineViewModel providerEngineModel);

        /// <summary>
        /// Update a Tax RuleType.
        /// </summary>
        /// <param name="providerEngineModel">ProviderEngine ViewModel.</param>
        /// <returns>Returns updated ProviderEngineViewModel.</returns>
        ProviderEngineViewModel UpdateTaxRuleType(ProviderEngineViewModel providerEngineTypeModel);

        /// <summary>
        /// Delete a Tax RuleType.
        /// </summary>
        /// <param name="taxRuleTypeId">ID of tax rule type to delete.</param>
        /// <param name="message">message of success/Failure to delete Tax rule type.</param>
        /// <returns>Returns true if TaxRuleType deleted else returns false.</returns>
        bool DeleteTaxRuleType(string taxRuleTypeId, out string message);

        /// <summary>
        /// Get all Tax Rule Types which are not present in database.
        /// </summary>
        /// <returns>Returns Tax RuleType list ViewModel which are not in database.</returns>
        List<SelectListItem> GetAllTaxRuleTypesNotInDatabase();

        /// <summary>
        /// Get the tax rule type on the basis of class name.
        /// </summary>
        /// <param name="name">Name of tax rule type class.</param>
        /// <returns>Returns ProviderEngine ViewModel.</returns>
        ProviderEngineViewModel GetTaxRuleTypeByClassName(string name);

        /// <summary>
        /// Bulk enable disable tax rule types
        /// </summary>
        /// <param name="taxRuleTypeId">Ids of taxrule type to enable/disable</param>
        /// <param name="isEnable">true if you want to enable and false if you want disable tax rule type.</param>
        /// <param name="message">message of success/Failure to enable/disable shipping type.</param>
        /// <returns>true/ false status for success or not.</returns>
        bool BulkEnableDisableTaxRuleTypes(string taxRuleTypeId, bool isEnable, out string message);
        #endregion

        #region Shipping Type
        /// <summary>
        /// Get the list of shipping type.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns ProviderEngine list ViewModel.</returns>
        ProviderEngineListViewModel GetShippingTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get a shipping type.
        /// </summary>
        /// <param name="shippingTypeId">ID of shippingType to get details of perticular shippingType.</param>
        /// <returns>Returns ProviderEngine ViewModel.</returns>
        ProviderEngineViewModel GetShippingType(int shippingTypeId);

        /// <summary>
        /// Get all Shipping Types which are not present in database.
        /// </summary>
        /// <returns>Returns ShippingType list ViewModel which are not in database.</returns>
        List<SelectListItem> GetAllShippingTypesNotInDatabase();

        /// <summary>
        /// Create new shipping type.
        /// </summary>
        /// <param name="shippingTypeViewModel">ProviderEngine ViewModel.</param>
        /// <returns>Returns true if ProviderEngine created else returns false.</returns>
        ProviderEngineViewModel CreateShippingType(ProviderEngineViewModel shippingTypeViewModel);

        /// <summary>
        /// Update a Shipping Type.
        /// </summary>
        /// <param name="providerEngineModel">ProviderEngine ViewModel.</param>
        /// <returns>Returns updated ProviderEngineViewModel.</returns>
        ProviderEngineViewModel UpdateShippingType(ProviderEngineViewModel shippingTypeViewModel);

        /// <summary>
        /// Delete a Shipping Type.
        /// </summary>
        /// <param name="shippingTypeId">ID of Shipping type to delete.</param>
        /// <param name="message">message of success/Failure to delete shipping type type.</param>
        /// <returns>Returns true if ShippingType deleted else returns false.</returns>
        bool DeleteShippingType(string shippingTypeId, out string message);

        /// <summary>
        /// Get the shipping type on the basis of class name.
        /// </summary>
        /// <param name="name">Name of shipping type class.</param>
        /// <returns>Returns ProviderEngine ViewModel.</returns>
        ProviderEngineViewModel GetShippingTypeByClassName(string name);

        /// <summary>
        /// Bulk enable disable shipping types
        /// </summary>
        /// <param name="shippingTypeId">Ids of shipping type to enable/disable</param>
        /// <param name="isEnable">true if you want to enable and false if you want disable shipping type.</param>
        /// <param name="message">message of success/Failure to enable/disable shipping type.</param>
        /// <returns>true/ false status for success or not.</returns>
        bool BulkEnableDisableShippingTypes(string shippingTypeId, bool isEnable, out string message);
        #endregion

        #region Promotion Type
        /// <summary>
        /// Get the list of promotion type.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns ProviderEngine list ViewModel.</returns>
        ProviderEngineListViewModel GetPromotionTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get a promotion type.
        /// </summary>
        /// <param name="promotionTypeId">ID of promotionType to get details of perticular promotionType.</param>
        /// <returns>Returns ProviderEngine ViewModel.</returns>
        ProviderEngineViewModel GetPromotionType(int promotionTypeId);

        /// <summary>
        /// Get all Promotion Types which are not present in database.
        /// </summary>
        /// <returns>Returns PromotionType list ViewModel which are not in database.</returns>
        List<SelectListItem> GetAllPromotionTypesNotInDatabase();

        /// <summary>
        /// Create new promotion type.
        /// </summary>
        /// <param name="promotionTypeViewModel">ProviderEngine ViewModel.</param>
        /// <returns>Returns true if ProviderEngine created else returns false.</returns>
        ProviderEngineViewModel CreatePromotionType(ProviderEngineViewModel promotionTypeViewModel);

        /// <summary>
        /// Update a Promotion Type.
        /// </summary>
        /// <param name="promotionTypeViewModel">ProviderEngine ViewModel.</param>
        /// <returns>Returns updated ProviderEngine view model.</returns>
        ProviderEngineViewModel UpdatePromotionType(ProviderEngineViewModel promotionTypeViewModel);

        /// <summary>
        /// Delete a Promotion Type.
        /// </summary>
        /// <param name="promotionTypeId">ID of Promotion type to delete.</param>
        /// <param name="message">message of success/Failure to delete promotion type.</param>
        /// <returns>Returns true if PromotionType deleted else returns false.</returns>
        bool DeletePromotionType(string promotionTypeId, out string message);

        /// <summary>
        /// Get the promotion type on the basis of class name.
        /// </summary>
        /// <param name="name">Name of promotion type class.</param>
        /// <returns>Returns ProviderEngine ViewModel.</returns>
        ProviderEngineViewModel GetPromotionTypeByClassName(string name);

        /// <summary>
        /// Bulk enable disable promotion types
        /// </summary>
        /// <param name="promotionTypeId">Ids of promotion type to enable/disable</param>
        /// <param name="isEnable">true if you want to enable and false if you want disable promotion type.</param>
        /// <param name="message">message of success/Failure to enable/disable promotion type.</param>
        /// <returns>true/ false status for success or not.</returns>
        bool BulkEnableDisablePromotionTypes(string promotionTypeId, bool isEnable, out string message);
        #endregion
    }
}
