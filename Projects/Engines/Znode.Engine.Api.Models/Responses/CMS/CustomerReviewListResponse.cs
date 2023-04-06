using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CustomerReviewListResponse : BaseListResponse
    {
        public List<CustomerReviewModel> CustomerReviewList { get; set; }
    }
}
