using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ISearchBoostAndBuryClient : IBaseClient
    {
        /// <summary>
        /// Gets boost and bury rule list.
        /// </summary>
        /// <param name="expands">Expands for attribute list.</param>
        /// <param name="filters">Filters for attribute list.</param>
        /// <param name="sorts">Sorts for attribute list.</param>
        /// <param name="page">Page Index of attribute list.</param>
        /// <param name="recordPerPage">Page size of attribute list.</param>
        /// <returns>list of rules for boost and bury.</returns>
        SearchBoostAndBuryRuleListModel GetBoostAndBuryRules(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? page, int? recordPerPage);

        /// <summary>
        /// Create boost and bury rule.
        /// </summary>
        /// <param name="searchBoostAndBuryRuleModel">SearchBoostAndBuryRuleModel</param>
        /// <returns>SearchBoostAndBuryRuleModel</returns>
        SearchBoostAndBuryRuleModel CreateBoostAndBuryRule(SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel);

        /// <summary>
        /// To get boost and bury rule by searchCatalogRuleId
        /// </summary>
        /// <param name="searchCatalogRuleId">searchCatalogRuleId to get boost and bury details.</param>
        /// <returns>Returns SearchBoostAndBuryRuleModel.</returns>
        SearchBoostAndBuryRuleModel GetBoostAndBuryRule(int searchCatalogRuleId);

        /// <summary>
        /// To update boost and bury rules
        /// </summary>
        /// <param name="searchBoostAndBuryRuleModel">SearchBoostAndBuryRuleModel model to update.</param>
        /// <returns>Returns updated WebStore SearchBoostAndBuryRule Model.</returns>
        SearchBoostAndBuryRuleModel UpdateBoostAndBuryRule(SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel);

        /// <summary>
        /// Delete Catalog search rule.
        /// </summary>
        /// <param name="parameterModel">catalog search rule id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool Delete(ParameterModel parameterModel);

        /// <summary>
        /// Paused an existing catalog search rule for a while.
        /// </summary>
        /// <param name="parameterModel">Search Rule Id.</param>
        /// <param name="isActive">flag for rule whether its pause or restart.</param>
        /// <returns>Returns true if paused successfully else return false.</returns>
        bool PausedSearchRule(ParameterModel parameterModel, bool isPause);

        /// <summary>
        /// Get searchable field list.
        /// </summary>
        /// <param name="PublishCatalogId">PublishCatalogId</param>
        /// <returns>Searchable field list.</returns>
        SearchBoostAndBuryRuleModel GetSearchableFieldList(int PublishCatalogId);

        /// <summary>
        ///Get Auto suggestion for boost and bury.
        /// </summary>
        /// <param name="boostAndBuryParameterModel">paramtere model for autosuggestion.</param>
        /// <returns>List of suggestions</returns>
        List<string> GetAutoSuggestion(BoostAndBuryParameterModel boostAndBuryParameterModel);
    }
}
