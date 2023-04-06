using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ISearchBoostAndBuryRuleService
    {
        /// <summary>
        /// Get search catalog rule list.
        /// </summary>
        /// <param name="expands">Expands collections</param>
        /// <param name="filters">Filters collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="page">Page Number</param>
        /// <returns>Returns list of search rule according to catalog.</returns>
        SearchBoostAndBuryRuleListModel GetBoostAndBuryRules(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create boost and bury rule.
        /// </summary>
        /// <param name="searchBoostAndBuryRuleModel">SearchBoostAndBuryRuleModel</param>
        /// <returns>SearchBoostAndBuryRuleModel</returns>
        SearchBoostAndBuryRuleModel CreateBoostAndBuryRule(SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel);

        /// <summary>
        /// Get boost and bury rule on the basis of searchCatalogRuleId.
        /// </summary>
        /// <param name="searchCatalogRuleId">searchCatalogRuleId.</param>
        /// <returns>Returns Search Boost And Bury Rule Model.</returns>
        SearchBoostAndBuryRuleModel GetBoostAndBuryRule(int searchCatalogRuleId);

        /// <summary>
        /// Update boost and bury rule data.
        /// </summary>
        /// <param name="searchBoostAndBuryRuleModel">searchBoostAndBuryRuleModel to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateBoostAndBuryRule(SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel);

        /// <summary>
        /// Paused an existing catalog search rule for a while.
        /// </summary>
        /// <param name="searchCatalogRuleId">Search Rule Id.</param>
        /// <returns>Returns true if paused successfully else return false.</returns>
        bool PausedSearchRule(ParameterModel searchCatalogRuleId, bool isPause);

        /// <summary>
        /// Delete Search catalog rule.
        /// </summary>
        /// <param name="searchCatalogRuleId">Search Rule Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool Delete(ParameterModel searchCatalogRuleId);

        /// <summary>
        /// Get searchable field list.
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <returns>Returns list of search field.</returns>
        SearchBoostAndBuryRuleModel GetSearchableFieldList(int publishCatalogId);

        /// <summary>
        ///Get Auto suggestion for boost and bury.
        /// </summary>
        /// <param name="boostAndBuryParameterModel">paramtere model for autosuggestion.</param>
        /// <returns>List of suggestions</returns>
        List<string> GetAutoSuggestion(BoostAndBuryParameterModel parameterModel);
    }
}
