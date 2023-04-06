using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchBoostAndBuryRuleListModel : BaseListModel
    {
        public List<SearchBoostAndBuryRuleModel> SearchBoostAndBuryRuleList { get; set; }
        public int PortalId { get; set; }
        public int PublishCatalogId { get; set; }
        public string CatalogName { get; set; }
    }
}
