using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProductFeedListModel: BaseListModel
    {
        public List<ProductFeedModel> ProductFeeds { get; set; }
    }
}
