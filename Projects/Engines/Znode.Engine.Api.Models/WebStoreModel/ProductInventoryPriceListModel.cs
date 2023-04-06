using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProductInventoryPriceListModel : BaseListModel
    {
        public List<ProductInventoryPriceModel> ProductList { get; set; }
    }
}
