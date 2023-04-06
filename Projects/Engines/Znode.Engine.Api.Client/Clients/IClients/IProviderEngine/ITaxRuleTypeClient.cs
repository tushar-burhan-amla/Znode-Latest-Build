using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ITaxRuleTypeClient : IBaseClient
    {
        /// <summary>
        /// Get the list of tax rule type.
        /// </summary>
        /// <param name="filters">Filter collection to generate where clause.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">current index of page.</param>
        /// <param name="pageSize">Record per page.</param>
        /// <returns>Returns Tax RuleType list model.</returns>
        TaxRuleTypeListModel GetTaxRuleTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get a tax rule type.
        /// </summary>
        /// <param name="taxRuleTypeId">ID of taxRuleType to get details of perticular TaxRuleType.</param>
        /// <returns>Returns Tax rule type model.</returns>
        TaxRuleTypeModel GetTaxRuleType(int taxRuleTypeId);

        /// <summary>
        /// Create new tax rule type.
        /// </summary>
        /// <param name="taxRuleTypeModel">TaxRuleType Model.</param>
        /// <returns>Returns newly created TaxRuleType Model.</returns>
        TaxRuleTypeModel CreateTaxRuleType(TaxRuleTypeModel taxRuleTypeModel);

        /// <summary>
        /// Update a Tax RuleType.
        /// </summary>
        /// <param name="taxRuleTypeModel">TaxRuleType Model.</param>
        /// <returns>Returns Tax Rule Type Model as response.</returns>
        TaxRuleTypeModel UpdateTaxRuleType(TaxRuleTypeModel taxRuleTypeModel);

        /// <summary>
        /// Delete a Tax RuleType.
        /// </summary>
        /// <param name="taxRuleTypeIds">ID of tax rule type to delete.</param>
        /// <returns>Returns true if TaxRuleType deleted else returns false.</returns>
        bool DeleteTaxRuleType(ParameterModel taxRuleTypeIds);

        /// <summary>
        /// Get all Tax Rule Types which are not present in database.
        /// </summary>
        /// <returns>Returns Tax RuleType list ViewModel which are not in database.</returns>
        TaxRuleTypeListModel GetAllTaxRuleTypesNotInDatabase();

        /// <summary>
        /// Bulk enable disable tax rule types
        /// </summary>
        /// <param name="taxRuleTypeIds">Ids of taxrule type to enable/disable</param>
        /// <param name="isEnable">true if you want to enable and false if you want disable tax rule type.</param>
        /// <returns>true/ false status for success or not.</returns>
        bool BulkEnableDisableTaxRuleTypes(ParameterModel taxRuleTypeIds, bool isEnable);
    }
}
