namespace Znode.Engine.Api.Client.Endpoints
{
    public class SearchBoostAndBuryRuleEndpoint : BaseEndpoint
    {

        //Get boost and bury rule list.
        public static string GetBoostAndBuryRules() => $"{ApiRoot}/searchboostandburyrule/list";

        //Create boost and bury rule.
        public static string CreateBoostAndBuryRule() => $"{ApiRoot}/searchboostandburyrule/createboostandburyrule";

        //Get boost and bury on the basis of searchCatalogRuleId.
        public static string GetBoostAndBuryRule(int searchCatalogRuleId) => $"{ApiRoot}/searchboostandburyrule/getboostandburyrule/{searchCatalogRuleId}";

        //Update boost and bury rule
        public static string UpdateBoostAndBuryRule() => $"{ApiRoot}/searchboostandburyrule/updateboostandburyrule";

        //delete catalog search rule.
        public static string Delete() => $"{ApiRoot}/searchboostandburyrule/delete";

        //Create boost and bury rule.
        public static string PausedSearchRule(bool isPause) => $"{ApiRoot}/searchboostandburyrule/pausedsearchrule/{isPause}";

        //Get searchable field list.
        public static string GetSearchableFieldList(int PublishCatalogId) => $"{ApiRoot}/searchboostandburyrule/searchablefieldlist/{PublishCatalogId}";

        //delete catalog search rule.
        public static string GetAutoSuggestion() => $"{ApiRoot}/searchboostandburyrule/getautosuggestion";

    }
}
