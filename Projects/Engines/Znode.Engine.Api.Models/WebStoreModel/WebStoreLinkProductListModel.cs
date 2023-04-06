using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreLinkProductListModel : BaseListModel
    {
        public List<WebStoreLinkProductModel> LinkProductList { get; set; }
    }
}
