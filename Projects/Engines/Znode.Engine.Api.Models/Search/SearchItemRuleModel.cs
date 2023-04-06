namespace Znode.Engine.Api.Models
{
    public class SearchItemRuleModel : BaseModel
    {
        public int SearchItemRuleId { get; set; }
        public int SearchCatalogRuleId { get; set; }
        public string SearchItemKeyword { get; set; }
        public string SearchItemCondition { get; set; }
        public string SearchItemValue { get; set; }
        public decimal? SearchItemBoostValue { get; set; }
        public bool IsItemForAll { get; set; }
    }
}
