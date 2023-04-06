namespace Znode.Libraries.Search
{
    public class ElasticSearchFeature
    {
        public int SearchFeatureId { get; set; }
        public int? ParentSearchFeatureId { get; set; }
        public string FeatureName { get; set; }
        public string SearchFeatureValue { get; set; }
        public string FeatureCode { get; set; }
    }
}
