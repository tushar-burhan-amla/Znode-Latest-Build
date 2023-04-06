namespace Znode.Engine.Admin.ViewModels
{
    public class SearchFeatureViewModel
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
