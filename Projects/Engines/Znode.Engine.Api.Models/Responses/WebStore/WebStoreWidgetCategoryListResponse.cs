using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreWidgetCategoryListResponse : BaseListResponse
    {
        //Category List Widget Response.
        public List<WebStoreWidgetCategoryModel> Categories { get; set; }
    }
}
