using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreBundleProductListResponse : BaseListResponse
    {
        public List<WebStoreBundleProductModel> BundleProducts { get; set; }
    }
}
