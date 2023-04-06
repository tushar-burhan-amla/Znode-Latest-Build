using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface ISearchBoostAndBuryRuleCache
    {
        string GetBoostAndBuryRules(string routeUri, string routeTemplate);

        /// <summary>
        /// Get boost and bury rule on the basis of searchCatalogRuleId.
        /// </summary>
        /// <param name="searchCatalogRuleId">searchCatalogRuleId.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns boost and bury rule.</returns>
        string GetBoostAndBuryRule(int searchCatalogRuleId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get searchable field list.
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns searchable field  boost and bury rule.</returns>
        string GetSearchableFieldList(int publishCatalogId, string routeUri, string routeTemplate);

        /// <summary>
        ///Get Auto suggestion for boost and bury.
        /// </summary>
        /// <param name="boostAndBuryParameterModel">paramtere model for autosuggestion.</param>
        /// <returns>List of suggestion</returns>
        string GetAutoSuggestion(BoostAndBuryParameterModel parameterModel, string routeUri, string routeTemplate);
    }
}
