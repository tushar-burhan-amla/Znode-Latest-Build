using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreProductListModel : BaseListModel
    {
        public List<WebStoreProductModel> ProductList { get; set; }
    }
}
