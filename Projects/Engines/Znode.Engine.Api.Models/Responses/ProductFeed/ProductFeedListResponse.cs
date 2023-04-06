using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ProductFeedListResponse : BaseListResponse
    {
        public List<ProductFeedModel> ProductFeeds { get; set; }
    }
}
