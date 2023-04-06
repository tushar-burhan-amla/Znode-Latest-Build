
namespace Znode.Engine.Api.Models
{
    public class ProductDetailModel : BaseModel
    {
        public int OmsSavedcartLineItemId { get; set; }
        public decimal? Price { get; set; }
        public decimal ShippingCost { get; set; }
        public string SKU { get; set; }
        public decimal? InitialPrice { get; set; }
        public decimal InitialShippingCost { get; set; }
        public bool IsPriceEdit { get; set; }
    }
}
