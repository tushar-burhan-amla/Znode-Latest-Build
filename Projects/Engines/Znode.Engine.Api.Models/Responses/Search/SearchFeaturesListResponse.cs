using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SearchFeaturesListResponse : BaseListResponse
    {
        public List<SearchFeatureModel> SearchFeaturesList { get; set; }
    }
}
