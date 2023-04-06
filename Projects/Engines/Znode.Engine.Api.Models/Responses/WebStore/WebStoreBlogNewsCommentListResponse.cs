using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreBlogNewsCommentListResponse : BaseListResponse
    {
        public List<WebStoreBlogNewsCommentModel> BlogNewsCommentList { get; set; }
    }
}
