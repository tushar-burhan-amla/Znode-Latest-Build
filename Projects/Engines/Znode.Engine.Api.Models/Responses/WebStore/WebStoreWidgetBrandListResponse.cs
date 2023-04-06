using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreWidgetBrandListResponse : BaseListResponse
    {
        //Brand List Widget Response.
        public List<WebStoreWidgetBrandModel> Brands { get; set; }
    }
}
