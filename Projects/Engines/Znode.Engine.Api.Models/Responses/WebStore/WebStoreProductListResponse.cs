using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreProductListResponse:BaseListResponse
    {
        public List<WebStoreProductModel> ProductsList { get; set; }
    }
}
