namespace Znode.Engine.Api.Models
{
    public class SearchFeatureModel : BaseModel
    {
        public int SearchFeatureId { get; set; }
        public int? ParentSearchFeatureId { get; set; }
        public string FeatureName { get; set; }
        public string SearchFeatureValue { get; set; }
        public string FeatureCode { get; set; }
        public bool IsAdvancedFeature { get; set; }
        public string ControlType { get; set; }
        public string HelpDescription { get; set; }
    }
}
