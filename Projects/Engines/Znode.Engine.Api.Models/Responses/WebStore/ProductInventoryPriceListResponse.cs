using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ProductInventoryPriceListResponse : BaseListResponse
    {
        public List<ProductInventoryPriceModel> ProductList { get; set; }
    }
}
