using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ITaxRuleTypeService
    {
        /// <summary>
        /// Get the list of all tax rule types.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection to generate orderby clause.</param>
        /// <param name="page">Collection  of paging parameters.</param>
        /// <returns>List of TaxRuleType Model.</returns>
        TaxRuleTypeListModel GetTaxRuleTypeList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get a Tax Class.
        /// </summary>
        /// <param name="taxRuleTypeId">ID of taxRuleType to get details of perticular TaxClass.</param>
        /// <returns>Returns TaxRuleType Model.</returns>
        TaxRuleTypeModel GetTaxRuleType(int taxRuleTypeId);

        /// <summary>
        /// Creates a new Tax Class.
        /// </summary>
        /// <param name="taxRuleTypeModel">TaxRuleType Model to create new tax rule type.</param>
        /// <returns>Returns newly created TaxClass Model.</returns>
        TaxRuleTypeModel CreateTaxRuleType(TaxRuleTypeModel taxRuleTypeModel);

        /// <summary>
        /// Update a Tax Class.
        /// </summary>
        /// <param name="taxRuleTypeModel">TaxRuleType Model to update tax rule type.</param>
        /// <returns>Returns true if TaxRuleType updated else returns false.</returns>
        bool UpdateTaxRuleType(TaxRuleTypeModel taxRuleTypeModel);

        /// <summary>
        /// Delete a Tax Class.
        /// </summary>
        /// <param name="entityIds">ID of tax rule type to delete.</param>
        /// <returns>Returns true if TaxRuleType deleted else returns false.</returns>
        bool DeleteTaxRuleType(ParameterModel entityIds);

        /// <summary>
        /// Get all Tax Rule Types which are not present in database.
        /// </summary>
        /// <returns>Return List Tax Rule Type.</returns>
        TaxRuleTypeListModel GetAllTaxRuleTypesNotInDatabase();

        /// <summary>
        /// Bulk enable disable tax rule types
        /// </summary>
        /// <param name="entityIds">Ids of taxrule type to enable/disable</param>
        /// <param name="isEnable">true if you want to enable and false if you want disable tax rule type.</param>
        /// <returns>true/ false status for success or not.</returns>
        bool EnableDisableTaxRuleType(ParameterModel entityIds, bool isEnable);
    }
}
