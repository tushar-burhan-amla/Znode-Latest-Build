using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class BlogNewsListModel : BaseListModel
    {
        public List<BlogNewsModel> BlogNewsList { get; set; }
    }
}
