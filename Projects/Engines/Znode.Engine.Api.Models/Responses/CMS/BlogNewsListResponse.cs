using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class BlogNewsListResponse : BaseListResponse
    {
        public List<BlogNewsModel> BlogNewsList { get; set; }
    }
}
