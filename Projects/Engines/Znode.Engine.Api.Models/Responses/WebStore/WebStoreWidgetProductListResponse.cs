using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreWidgetProductListResponse : BaseListResponse
    {
        //Product List Widget Response.
        public List<WebStoreWidgetProductModel> Products { get; set; }
        public string DisplayName { get; set; }
    }
}
