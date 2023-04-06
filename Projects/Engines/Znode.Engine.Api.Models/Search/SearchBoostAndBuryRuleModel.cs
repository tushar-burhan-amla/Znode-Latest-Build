using System;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchBoostAndBuryRuleModel : BaseModel
    {
        public int PublishCatalogId { get; set; }
        public int SearchCatalogRuleId { get; set; }
        public int SearchItemRuleId { get; set; }
        public int SearchTriggerRuleId { get; set; }
        public string CatalogName { get; set; }
        public string RuleName { get; set; }
        public string Paused { get; set; }
        public string UserName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsGlobalRule { get; set; }
        public bool IsTriggerForAll { get; set; }
        public bool IsItemForAll { get; set; }
        public bool IsSearchIndexExists { get; set; }
        public List<SearchTriggerRuleModel> SearchTriggerRuleList { get; set; }
        public List<SearchItemRuleModel> SearchItemRuleList { get; set; }
        public List<FieldValueModel> SearchableFieldValueList { get; set; }

        public bool IsPause { get; set; }

    }
}
