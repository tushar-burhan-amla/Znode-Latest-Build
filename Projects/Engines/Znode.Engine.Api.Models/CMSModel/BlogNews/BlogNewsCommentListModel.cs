using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class BlogNewsCommentListModel : BaseListModel
    {
        public List<BlogNewsCommentModel> BlogNewsCommentList { get; set; }
    }
}
