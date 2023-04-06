namespace Znode.Engine.Api.Models
{
    public class SearchFacetValueModel : BaseModel
    {
        public string AttributeValue { get; set; }
        public long FacetCount { set; get; }
        public string Label { get; set; }
        public string RangeMax { get; set; }
        public string RangeMin { get; set; }
        public string RangeEnd { get; set; }
        public string RangeStart { get; set; }
        public int? DisplayOrder { get; set; }
    }
}