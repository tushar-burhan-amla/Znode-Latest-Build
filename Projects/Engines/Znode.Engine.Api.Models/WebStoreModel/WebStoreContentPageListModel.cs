using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreContentPageListModel : BaseListModel
    {
        public List<WebStoreContentPageModel> ContentPageList { get; set; }
    }
}
