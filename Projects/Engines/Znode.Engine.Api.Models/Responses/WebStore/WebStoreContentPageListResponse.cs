using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreContentPageListResponse : BaseListResponse
    {
        public List<WebStoreContentPageModel> ContentPageList { get; set; }
    }
}
