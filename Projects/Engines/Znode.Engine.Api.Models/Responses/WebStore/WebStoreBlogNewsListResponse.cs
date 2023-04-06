using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreBlogNewsListResponse : BaseListResponse
    {
        public List<WebStoreBlogNewsModel> BlogNewsList { get; set; }
    }
}

