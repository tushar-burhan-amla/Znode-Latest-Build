using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreWidgetSearchModel : BaseModel
    {
        public List<SearchFacetModel> Facets { get; set; }
        public List<SearchProductModel> Products { get; set; }
        public long TotalProductCount { get; set; }
    }
}
