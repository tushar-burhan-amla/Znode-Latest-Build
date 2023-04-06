using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProductInventoryDetailModel : BaseModel
    {
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public decimal? AllLocationQuantity { get; set; }
        public List<InventorySKUModel> Inventory { get; set; }
    }
}
