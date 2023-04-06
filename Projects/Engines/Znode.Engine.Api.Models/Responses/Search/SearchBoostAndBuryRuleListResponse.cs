using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchBoostAndBuryRuleListResponse : BaseListResponse
    {
        public List<SearchBoostAndBuryRuleModel> SearchBoostAndBuryRuleList { get; set; }
        public int PublishCatalogId { get; set; }
        public string CatalogName { get; set; }
    }
}
