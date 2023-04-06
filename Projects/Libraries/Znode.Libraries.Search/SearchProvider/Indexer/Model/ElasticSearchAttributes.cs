namespace Znode.Libraries.Search
{
    public class ElasticSearchAttributes
    {
        public string AttributeCode { get; set; }
        public int? BoostValue { get; set; }
        public bool IsFacets { get; set; }
        public bool IsUseInSearch { get; set; }
    }
}
