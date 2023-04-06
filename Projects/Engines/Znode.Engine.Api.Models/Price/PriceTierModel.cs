namespace Znode.Engine.Api.Models
{
    public class PriceTierModel : BaseModel
    {
        public int PriceTierId { get; set; }
        public int PriceListId { get; set; }
        public string SKU { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? MinQuantity { get; set; }
        public decimal? MaxQuantity { get; set; }
    }
}
