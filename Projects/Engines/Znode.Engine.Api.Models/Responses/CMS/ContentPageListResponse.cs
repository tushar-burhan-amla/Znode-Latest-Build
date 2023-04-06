using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ContentPageListResponse : BaseListResponse
    {
        public List<ContentPageModel> ContentPageList { get; set; }
    }
}
