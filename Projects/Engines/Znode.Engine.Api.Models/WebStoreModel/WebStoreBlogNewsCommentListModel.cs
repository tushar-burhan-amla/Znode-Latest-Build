using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreBlogNewsCommentListModel : BaseListModel
    {
        public List<WebStoreBlogNewsCommentModel> BlogNewsCommentList { get; set; }
    }
}

