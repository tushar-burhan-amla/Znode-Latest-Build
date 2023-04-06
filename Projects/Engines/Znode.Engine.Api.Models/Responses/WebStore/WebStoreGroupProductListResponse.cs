using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreGroupProductListResponse : BaseResponse
    {
        public List<WebStoreGroupProductModel> GroupProducts { get; set; }
    }
}
