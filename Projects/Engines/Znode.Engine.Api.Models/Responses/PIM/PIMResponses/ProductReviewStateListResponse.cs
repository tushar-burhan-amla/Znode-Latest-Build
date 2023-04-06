using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ProductReviewStateListResponse : BaseListResponse
    {
        public List<ProductReviewStateModel> ProductReviewStates { get; set; }
    }
}
