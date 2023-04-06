using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreBlogNewsListModel : BaseListModel
    {
        public List<WebStoreBlogNewsModel> BlogNewsList { get; set; }
    }
}
