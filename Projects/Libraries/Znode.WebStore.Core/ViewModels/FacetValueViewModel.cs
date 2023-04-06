namespace Znode.Engine.WebStore.ViewModels
{
    public class FacetValueViewModel : BaseViewModel
    {
        public string AttributeCode { get; set; }
        public string Label { get; set; }
        public string AttributeValue { get; set; }
        public long FacetCount { set; get; }
        public string RefineByUrl { get; set; }
        public string RangeMax { get; set; }
        public string RangeMin { get; set; }
        public string RangeEnd { get; set; }
        public string RangeStart { get; set; }
    }
}