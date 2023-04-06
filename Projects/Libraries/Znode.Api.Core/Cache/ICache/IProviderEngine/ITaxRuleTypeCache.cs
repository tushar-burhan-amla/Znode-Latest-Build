namespace Znode.Engine.Api.Cache
{
    public interface ITaxRuleTypeCache
    {
        /// <summary>
        /// Get the list of all tax rule types.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>list of all tax rule type in string format.</returns>
        string GetTaxRuleTypeList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get a Tax RuleType.
        /// </summary>
        /// <param name="taxRuleTypeId">taxRuleTypeId to get TaxClass.</param>
        /// <param name="routeUri">route URL.</param>
        /// <param name="routeTemplate">route Template.</param>
        /// <returns>Returns TaxRuleType Model in string format.</returns>
        string GetTaxRuleType(int taxRuleTypeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get all Tax Rule Types which are not present in database.
        /// </summary>
        /// <param name="routeUri">route URL.</param>
        /// <param name="routeTemplate">route Template.</param>
        /// <returns>Return List Tax Rule Type.</returns>
        string GetAllTaxRuleTypesNotInDatabase(string routeUri, string routeTemplate);
    }
}
