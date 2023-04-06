using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class BlogNewsCommentListResponse : BaseListResponse
    {
        public List<BlogNewsCommentModel> BlogNewsCommentList { get; set; }
    }
}
