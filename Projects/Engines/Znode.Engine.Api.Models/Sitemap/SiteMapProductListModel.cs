using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SiteMapProductListModel : BaseListModel
    {
        public List<SiteMapProductModel> ProductList { get; set; }
    }
}
