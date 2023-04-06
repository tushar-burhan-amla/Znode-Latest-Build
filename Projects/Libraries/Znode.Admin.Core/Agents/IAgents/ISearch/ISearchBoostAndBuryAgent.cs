using System.Collections.Generic;
using Znode.Engine.Admin.ViewModels;
using System;

namespace Znode.Engine.Admin.Agents
{
    public interface ISearchBoostAndBuryAgent
    {
        /// <summary>
        /// To get boost and bury list.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <param name="catalogId">Catalog Id</param>
        /// <param name="catalogName">CatalogName</param>
        /// <returns>SearchBoostAndBuryRuleListViewModel</returns>
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because catalogId will be fetched from the filters" +
        " Please use overload of this method which does not have catalogId and catalogName as parameters ")]
        SearchBoostAndBuryRuleListViewModel GetBoostAndBuryRules(FilterCollectionDataModel model, int catalogId, string catalogName);

        /// <summary>
        /// To get boost and bury list.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <returns>SearchBoostAndBuryRuleListViewModel</returns>
        SearchBoostAndBuryRuleListViewModel GetBoostAndBuryRules(FilterCollectionDataModel model);

        /// <summary>
        /// To get boost and bury rule by searchCatalogRuleId
        /// </summary>
        /// <param name="searchCatalogRuleId">int searchCatalogRuleId</param>
        /// <returns>returns SearchBoostAndBuryRule model</returns>
        SearchBoostAndBuryRuleViewModel GetBoostAndBuryRule(int searchCatalogRuleId);

        /// <summary>
        /// To Save boost and bury rule.
        /// </summary>
        /// <param name="model">SearchBoostAndBuryRuleViewModel model</param>
        /// <returns>returns SearchBoostAndBuryRuleViewModel</returns>
        SearchBoostAndBuryRuleViewModel CreateBoostAndBuryRule(SearchBoostAndBuryRuleViewModel model);

        /// <summary>
        /// To update boost and bury rules
        /// </summary>
        /// <param name="model">CaseRequestViewModel model</param>
        /// <returns>returns CaseRequestViewModel</returns>
        SearchBoostAndBuryRuleViewModel UpdateBoostAndBuryRule(SearchBoostAndBuryRuleViewModel model);

        /// <summary>
        /// Delete Catalog search rule.
        /// </summary>
        /// <param name="searchRuleId">catalog search rule id.</param>
        /// <param name="errorMessage">error messge for if any for delete the rule</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteCatalogSearchRule(string searchRuleId, out string errorMessage);

        /// <summary>
        /// Paused an existing catalog search rule for a while.
        /// </summary>
        /// <param name="searchCatalogRuleId">Search Rule Id.</param>
        /// <param name="isActive">flag for rule whther its pause or restart.</param>
        /// <returns>Returns true if paused successfully else return false.</returns>
        bool PauseCatalogSearchRule(string searchRuleId, bool isPause, out string errorMessage);

        /// <summary>
        /// To bind page dropdown for create/edit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        SearchBoostAndBuryRuleViewModel BindPageDropdown(SearchBoostAndBuryRuleViewModel model);

        /// <summary>
        /// Checks wheater rule name already exists.
        /// </summary>
        /// <param name="ruleName">Rule name to check.</param>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="searchCatalogRuleId">search Catalog Rule Id.</param>
        /// <returns>Returns true if rule name already exists else false.</returns>
        bool IsRuleNameExist(string ruleName, int publishCatalogId, int searchCatalogRuleId);

        /// <summary>
        ///Get Auto suggestion for boost and bury.
        /// </summary>
        /// <param name="query">Search term</param>
        /// <param name="fieldName">field name.</param>
        /// <param name="publishCatalogId">Catalog id.</param>
        /// <returns>List of autosuggestion</returns>
        List<string> GetAutoSuggestion(string query, string fieldName, int publishCatalogId);
    }
}
