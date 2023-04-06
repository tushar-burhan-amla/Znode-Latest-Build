using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreBundleProductListModel : BaseListModel
    {
        public List<WebStoreBundleProductModel> BundleProducts { get; set; }
    }
}
