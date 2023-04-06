using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreLinkProductListResponse : BaseListResponse
    {
        public List<WebStoreLinkProductModel> LinkProductsList { get; set; }
    }
}
