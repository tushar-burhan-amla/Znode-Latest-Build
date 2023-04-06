
namespace Znode.Engine.klaviyo.Models
{
    public class OrderLineItemDetailsModel
    {
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Image { get; set; }
    }
}
