using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SiteMapProductListResponse : BaseListResponse
    {
        public List<SiteMapProductModel> ProductList { get; set; }
    }
}
