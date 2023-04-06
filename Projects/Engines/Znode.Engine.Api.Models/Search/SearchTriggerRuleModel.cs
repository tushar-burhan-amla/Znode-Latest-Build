namespace Znode.Engine.Api.Models
{
    public class SearchTriggerRuleModel : BaseModel
    {
        public int SearchCatalogRuleId { get; set; }
        public int SearchTriggerRuleId { get; set; }
        public string SearchTriggerKeyword { get; set; }
        public string SearchTriggerCondition { get; set; }
        public string SearchTriggerValue { get; set; }

    }
}
