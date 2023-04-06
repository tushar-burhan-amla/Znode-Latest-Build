using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class RecentViewProductResponse : BaseListResponse
    {
        public List<RecentViewProductModel> RecentViewProductModelCollection { get; set; }
    }
}
